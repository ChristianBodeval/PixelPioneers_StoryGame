using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : Health
{
    private const float DAMAGED_HEALTH_SHRINK_TIMER_MAX = 0.5f;
    public Slider HP;
    public Slider DamagedBar;
    private float damagedHealthShrinkTimer;
    private bool invulnerable = false;
    private Coroutine invulnerableCoroutine;
    public CameraShake cameraShake;
    public Gradient gradient;
    public Image HPFill;

    // Constructor
    public PlayerHealth(float startingHealth, float maxHealth) : base(startingHealth, maxHealth)
    {
        this.currentHealth = startingHealth;
        this.maxHealth = maxHealth;
    }

    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        DamagedBar.value = HP.value;
    }

    public override void TakeDamage(float damage)
    {
        damagedHealthShrinkTimer = DAMAGED_HEALTH_SHRINK_TIMER_MAX;
        if (!invulnerable)
        {
            cameraShake.TakesDamage();
            this.currentHealth -= damage;
            if (invulnerableCoroutine != null) StopCoroutine(invulnerableCoroutine);
            invulnerableCoroutine = StartCoroutine(MomentaryInvulnerability());
        }

        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
        blinkCoroutine = StartCoroutine(BlinkOnDmgTaken(freezeDurationOnDmgTaken));
        
    }

    // Changes material and color for a duration
    public override IEnumerator BlinkOnDmgTaken(float duration = 0.15f)
    {
        if (deathCoroutine != null) yield break;

        Material blinkMat = Instantiate(blinkMaterial);
        sr.material = blinkMat;
        sr.material.color = Color.white;

        yield return new WaitForSeconds(duration);

        sr.material = baseMaterial;
        sr.material.color = Color.white;

        blinkCoroutine = null;
    }

    public override void HealDamage(float healAmount)
    {
        this.currentHealth += healAmount;
        if (this.currentHealth > maxHealth)
        {
            this.currentHealth = maxHealth;
        }
        DamagedBar.value = HP.value;
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
        HPFill.color = gradient.Evaluate(HP.normalizedValue);
        if (this.currentHealth <= 0)
        {
            Die();
        }
        damagedHealthShrinkTimer -= Time.deltaTime;
        if (damagedHealthShrinkTimer < 0)
        {
            if (HP.value <= DamagedBar.value) {
                float shrinkSpeed = 10f;
                DamagedBar.value-= shrinkSpeed * Time.deltaTime;
            }
        }
    }
}
