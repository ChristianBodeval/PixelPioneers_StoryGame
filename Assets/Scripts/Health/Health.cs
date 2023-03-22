using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float currentHealth;
    public float maxHealth;
    [SerializeField] private Material blinkMaterial;
    [SerializeField] private Material baseMaterial;
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
        if (gameObject.CompareTag("Enemy")) StartCoroutine(GetComponent<Crowd_Control>().FreezeFrame(freezeDurationOnDmgTaken));

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
        Material blinkMat = Instantiate(blinkMaterial);
        sr.material = blinkMat;
        sr.material.color = Color.red;

        yield return new WaitForSeconds(duration);

        sr.material = baseMaterial;
        sr.material.color = Color.white;

        blinkCoroutine = null;
    }


    public void Die()
    {
        if (gameObject.CompareTag("Enemy") && isActiveAndEnabled)
        {
            HealthPickUp.pickUpPool.AddHealthPickUp(transform.position, maxHealth / 4); // Spawn health pickup
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
        sr.material = baseMaterial;
        sr.color = Color.white;
        Mjölnir.cannotHitList.Remove(gameObject); // Remove this enemy from the list
    }

    protected virtual void Update()
    {
        if (this.currentHealth <= 0)
        {
            Die();
        }
    }
}
