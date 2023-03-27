using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float currentHealth;
    public float maxHealth;
    [SerializeField] private Material blinkMaterial;
    [SerializeField] private Material baseMaterial;
    [SerializeField] private Material deathMaterial;
    [SerializeField] private float deathAnimDuration;
    private Coroutine deathCoroutine;
    protected Coroutine blinkCoroutine;
    protected static readonly float freezeDurationOnDmgTaken = 0.15f;

    private SpriteRenderer sr;

    private void Start()
    {
        sr = gameObject.GetComponentInChildren<SpriteRenderer>();
    }

    // Constructor
    public Health(float health, float maxHealth)
    {
        this.currentHealth = health;
        this.maxHealth = maxHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        this.currentHealth -= damage;

        // Freeze frame enemies
        if (gameObject.CompareTag("Enemy")) GetComponent<Crowd_Control>().FreezeFrame(freezeDurationOnDmgTaken);

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }

        blinkCoroutine = StartCoroutine(BlinkOnDmgTaken(freezeDurationOnDmgTaken));
        //PrintDmgToScreen(damage, Color.red);
    }

    public virtual void HealDamage(float heal)
    {
        if (currentHealth + heal > maxHealth)
        {
            currentHealth = maxHealth;
            return;
        }

        currentHealth += heal;
        PrintDmgToScreen(heal, Color.green);
    }

    // Changes material and color for a duration
    public IEnumerator BlinkOnDmgTaken(float duration = 0.15f)
    {
        if (deathCoroutine != null) yield break;

        Material blinkMat = Instantiate(blinkMaterial);
        sr.material = blinkMat;
        sr.material.color = Color.red;

        yield return new WaitForSeconds(duration);

        sr.material = baseMaterial;
        sr.material.color = Color.white;

        blinkCoroutine = null;
    }

    protected IEnumerator Die()
    {
        if (gameObject.CompareTag("Enemy"))
        {
            HealthPickUp.pickUpPool.AddHealthPickUp(transform.position, maxHealth / 4); // Spawn health pickup

            gameObject.GetComponent<Crowd_Control>().Stun();
            gameObject.GetComponent<Collider2D>().enabled = false;
            Material deathMat = Instantiate(deathMaterial);
            sr.material = deathMat;
            sr.material.color = Color.red;

            float timeStep = deathAnimDuration / 4;
            float t = 1f;
            float tStep = 1 / timeStep;

            while (sr.material.GetFloat("_FadeTime") > 0f)
            {
                t -= timeStep;
                sr.material.SetFloat("_FadeTime", t);
                yield return new WaitForSeconds(timeStep);
            }

            GameObject.Find("EnemyFactory").GetComponent<SpawnSystem>().RemoveFromWaitDeathList(gameObject);
            Pool.pool.ReturnToPool(gameObject);
        }
    }

    private void PrintDmgToScreen(float number, Color color)
    {
        //**
    }

    private void OnEnable()
    {
        this.currentHealth = maxHealth;
    }

    private void OnDisable()
    {
        Mjölnir.cannotHitList.Remove(gameObject); // Remove this enemy from the list
        if (deathCoroutine != null) StopCoroutine(deathCoroutine);
        deathCoroutine = null;
        sr.material = baseMaterial;
        sr.color = Color.white;
        gameObject.GetComponent<Collider2D>().enabled = true;
    }

    protected virtual void Update()
    {
        if (this.currentHealth <= 0 && deathCoroutine == null)
        {
            if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
            deathCoroutine = StartCoroutine(Die());
        }
    }
}
