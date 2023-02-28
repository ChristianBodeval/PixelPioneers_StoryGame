using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthObject : MonoBehaviour
{
    public float health { get; private set; }
    public float maxHealth { get; private set; }

    private float blinkDuration = 0.1f;
    private static Material blinkMaterial = (Material)Resources.Load("Materials/White", typeof(Material));

    // Constructor
    public HealthObject(float health, float maxHealth)
    {
        this.health = health;
        this.maxHealth = maxHealth;
    }

    public void SetHealth(float health)
    {
        this.health = health;
    }

    public void SetMaxHealth(float maxHealth)
    {
        this.maxHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        this.health -= damage;

        StartCoroutine(BlinkOnDmgTaken(blinkDuration));
        DisplayNumber(damage, Color.red);
    }

    public void HealDamage(float heal)
    {
        this.health += heal;
        DisplayNumber(heal, Color.green);
    }

    private IEnumerator BlinkOnDmgTaken(float wait)
    {
        SpriteRenderer sr = this.gameObject.GetComponentInChildren<SpriteRenderer>();
        Material temp = sr.material;
        sr.material = HealthObject.blinkMaterial;

        yield return new WaitForSeconds(wait);

        sr.material = temp;
    }

    private void DisplayNumber(float number, Color color)
    {
        //**
    }
}
