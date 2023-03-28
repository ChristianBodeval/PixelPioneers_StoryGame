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
    protected Coroutine shakeCoroutine;
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

        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine); // Stops blink coroutine
        blinkCoroutine = StartCoroutine(BlinkOnDmgTaken(freezeDurationOnDmgTaken));

        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(SpriteShake(0.2f, freezeDurationOnDmgTaken));
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

    protected IEnumerator SpriteShake(float magnitude, float duration)
    {
        int iterations = 8;
        float timeBetweenShakes = duration / iterations;
        float magnitudeReductionOverDuration = magnitude / iterations;
        GameObject spriteObject = GetComponentInChildren<SpriteRenderer>().gameObject;
        duration += Time.time;

        // Shake to either left or right first, at random
        Vector3 dir = Random.Range(0, 2) == 0 ? Vector3.left : Vector3.right; // Ternary operator '?' is just a short if statement - outputs left or right vector

        while (duration > Time.time)
        {
            spriteObject.transform.localPosition = dir * magnitude; // Displace sprite
            dir = dir * -1f; // Swap direction
            magnitude -= magnitudeReductionOverDuration; // Reduce shake over the duration
            yield return new WaitForSeconds(timeBetweenShakes);
        }

        spriteObject.transform.localPosition = Vector3.zero;
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
        GetComponentInChildren<SpriteRenderer>().gameObject.transform.localPosition = Vector3.zero; // Resets sprite position
    }

    protected virtual void Update()
    {
        if (this.currentHealth <= 0 && deathCoroutine == null)
        {
            if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
            if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
            deathCoroutine = StartCoroutine(Die());
        }
    }
}
