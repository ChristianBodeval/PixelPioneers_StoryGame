using UnityEngine;
using UnityEngine.UI;

public class BossHealth : Health
{
    public Image HP;
    public Image DamagedBar;

    private const float DAMAGED_HEALTH_SHRINK_TIMER_MAX = 0.5f;
    private float damagedHealthShrinkTimer;

    private void Start()
    {
        DamagedBar.fillAmount = HP.fillAmount;
        sr = gameObject.GetComponentInChildren<SpriteRenderer>();
    }

    public BossHealth(float startingHealth, float maxHealth) : base(startingHealth, maxHealth)
    {
        this.currentHealth = startingHealth;
        this.maxHealth = maxHealth;
    }

    protected override void Update()
    {
        HP.fillAmount = currentHealth / 100f;

        damagedHealthShrinkTimer -= Time.deltaTime;
        if (damagedHealthShrinkTimer < 0)
        {
            if (HP.fillAmount <= DamagedBar.fillAmount)
            {
                float shrinkSpeed = 0.1f;
                DamagedBar.fillAmount -= shrinkSpeed * Time.deltaTime;
            }
        }
        if (this.currentHealth <= 0 && deathCoroutine == null)
        {
            if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
            if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
            deathCoroutine = StartCoroutine(Die());
        }
    }

    public override void TakeDamage(float damage)
    {
        damagedHealthShrinkTimer = DAMAGED_HEALTH_SHRINK_TIMER_MAX;
        if (this.isActiveAndEnabled)
        {
            this.currentHealth -= damage;

            // Hermes moves when haven taken damage
            if (gameObject.CompareTag("Boss"))
            {
                GetComponent<Hermes_Pathing>().MoveOnHitTaken();

                if (Random.Range(0, 2) == 0)
                {
                    // Create blood and pickup
                    HealthPickUp.pickUpPool.AddHealthPickUp(transform.position, 1f); // Spawn health pickup
                    GameObject blood = Pool.pool.DrawFromBloodPool();
                    blood.transform.position = transform.position;
                    blood.transform.Rotate(new Vector3(0f, 0f, Random.Range(0, 4) * 90f)); // Random rotation
                    float size = Random.Range(2f, 3f);
                    blood.transform.localScale = new Vector3(size, size, 1f);
                }
            }

            // Freeze frame enemies
            if (gameObject.CompareTag("Enemy")) GetComponent<Crowd_Control>().FreezeFrame(freezeDurationOnDmgTaken);

            // Spawn blood
            GameObject bloodSplatter = Pool.pool.DrawFromBloodSpatterPool();
            bloodSplatter.transform.position = transform.position;

            if (blinkCoroutine != null) StopCoroutine(blinkCoroutine); // Stops blink coroutine
            blinkCoroutine = StartCoroutine(BlinkOnDmgTaken(freezeDurationOnDmgTaken));

            if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
            shakeCoroutine = StartCoroutine(SpriteShake(0.12f, freezeDurationOnDmgTaken));
            //PrintDmgToScreen(damage, Color.red);

            DamageTakenEvent.Invoke();
        }
    }
}