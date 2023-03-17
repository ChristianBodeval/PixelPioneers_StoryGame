using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float currentHealth;
    public float maxHealth; 

    private float blinkDuration = 0.1f;
    private static Material blinkMaterial = (Material)Resources.Load("Materials/White", typeof(Material));

    // Constructor
    public Health(float health, float maxHealth)
    {
        this.currentHealth = health;
        this.maxHealth = maxHealth;
    }

    public void SetHealth(float health)
    {
        this.currentHealth = health;
    }

    public void SetMaxHealth(float maxHealth)
    {
        this.maxHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        this.currentHealth -= damage;

        StartCoroutine(BlinkOnDmgTaken(blinkDuration));
        DisplayNumber(damage, Color.red);
    }

    public void HealDamage(float heal)
    {
        this.currentHealth += heal;
        DisplayNumber(heal, Color.green);
    }

    private IEnumerator BlinkOnDmgTaken(float wait)
    {
        SpriteRenderer sr = this.gameObject.GetComponentInChildren<SpriteRenderer>();
        Material temp = sr.material;
        sr.material = Health.blinkMaterial;

        yield return new WaitForSeconds(wait);

        sr.material = temp;
    }
    public void Die()
    {
        Destroy(this.gameObject);
        Debug.Log(this.gameObject.name +  "HAVE DIED HAHA!"); 

    }

    private void DisplayNumber(float number, Color color)
    {
        //**
    }
    private void Start()
    {
        this.currentHealth = maxHealth;
    }
    protected virtual void Update()
    {
        if (this.currentHealth <= 0)
        {
            Die();
        }
    }
}
