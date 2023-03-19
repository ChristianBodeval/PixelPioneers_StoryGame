using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : Health
{
    public Slider HP;
    private bool invulnerable = false;
    private Coroutine invulnerableCoroutine;

    // Constructor
    public PlayerHealth(float startingHealth, float maxHealth) : base(startingHealth, maxHealth)
    {
        this.currentHealth = startingHealth;
        this.maxHealth = maxHealth;
    }

    public override void TakeDamage(float damage)
    {
        if (!invulnerable)
        {
            this.currentHealth -= damage;
            if (invulnerableCoroutine != null) StopCoroutine(invulnerableCoroutine);
            invulnerableCoroutine = StartCoroutine(MomentaryInvulnerability());
        }

        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
        blinkCoroutine = StartCoroutine(BlinkOnDmgTaken(freezeDurationOnDmgTaken));
    }

    public void AddInvulnerability()
    {
        invulnerable = true;
    }

    public void RemoveInvulnerability()
    {
        invulnerable = false;
    }

    private IEnumerator MomentaryInvulnerability()
    {
        invulnerable = true;

        yield return new WaitForSeconds(0.05f);

        invulnerable = false;
    }

    protected override void Update()
    {
        HP.value = currentHealth;
        if (this.currentHealth <= 0)
        {
            Die();
        }
    }
}
