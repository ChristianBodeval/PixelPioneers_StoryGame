using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Charger_Attack : Enemy_Attack
{
    [SerializeField] private float attackTelegraphTime;
    [SerializeField] private float attackDMG;

    [Header("Charge Attack")]
    [SerializeField] private GameObject dangerIndicator;
    [SerializeField] private float chargeDmg;
    public float chargeRange = 7f;
    [SerializeField] private float chargeUpTime;
    [SerializeField] private float chargeHitBox;
    [SerializeField] private float chargeSpeed;
    [SerializeField] private float chargeUpdateInterval;
    [SerializeField] private float chargeCD;
    [SerializeField] private LineRenderer lr;
    private bool canCharge = true;
    private Coroutine chargeCoroutine;
    private Collider2D col;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponentInChildren<Animator>();
        animator.SetBool("AttackRDY", true); // Make sure we can attack
        col = GetComponent<Collider2D>();
        lr = GetComponent<LineRenderer>();
    }

    private void FixedUpdate()
    {
        StartCharge(player);
        InAttackRange(player); // Player variable is inherited from IEnemyAttack
        StopCharge();
    }

    private void OnEnable()
    {
        if (animator != null) { animator.SetBool("AttackRDY", true); } // Resets variable when respawning
    }

    private void OnDisable()
    {
        if (chargeCoroutine != null) StopCoroutine(chargeCoroutine);
        canCharge = true;
        dangerIndicator.SetActive(false);
    }

    //TODO Unoedvendig, hvis den gør det samme som parentklassen
    public override void Attack()
    {
        StartCoroutine(AttackCD(attackCD)); // Starts cooldown for the attack
        StartCoroutine(TelegraphAttack());
    }

    private IEnumerator TelegraphAttack()
    {
        yield return new WaitForSeconds(attackTelegraphTime);
        if (Vector3.Distance(player.transform.position, transform.position) <= attackRange) player.GetComponent<PlayerHealth>().TakeDamage(attackDMG); // Deal damage
    }

    public void Charge()
    {
        StartCoroutine(ChargeCD());
        if (chargeCoroutine != null) StopCoroutine(chargeCoroutine);
        chargeCoroutine = StartCoroutine(ChargeCoroutine());
    }

    private IEnumerator ChargeCoroutine()
    {
        GetComponent<Crowd_Control>().isStunImmune = true;
        animator.SetBool("CanMove", false);
        Physics2D.IgnoreLayerCollision(12, 3);
        Physics2D.IgnoreLayerCollision(12, 7);
        dangerIndicator.SetActive(true);

        Color lowAlphaRed = new Color(1f, 0f, 0f, 0f);
        Color highAlphaRed = new Color(1f, 0f, 0f, 0.7f);
        float totalTicks = 10f;
        float yieldDuration = (chargeUpTime - 0.1f) / totalTicks;
        float t = 0f;
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Vector3 targetPos = transform.position + 2f * chargeRange * direction;

        // Slow increase of alpha and size of line
        while (t < 1f)
        {
            t += 1f / totalTicks;

            // Alpha of line renderer
            lr.startColor = Color.Lerp(lowAlphaRed, highAlphaRed, t);
            lr.endColor = Color.Lerp(lowAlphaRed, highAlphaRed, t);

            // Set start & end pos of line renderer
            direction = (player.transform.position - transform.position).normalized;
            targetPos = transform.position + direction * (2f * chargeRange);
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, Vector3.Lerp(transform.position, targetPos, t));

            // Set width
            lr.widthMultiplier = 0.1f + t * 0.02f;

            if (!IsInLineOfSight(player, animator)) 
            { 
                animator.SetBool("CanMove", true);
                //Enable collisions
                Physics2D.IgnoreLayerCollision(12, 3, false);
                Physics2D.IgnoreLayerCollision(12, 7, false);
                lr.startColor = lowAlphaRed;
                lr.endColor = lowAlphaRed;
                yield break; 
            }

            yield return new WaitForSeconds(yieldDuration);
        }

        t = 0f; // Resets value of t

        // Lerp color to white
        while (t < 1f)
        {
            t += 0.15f;

            lr.startColor = Color.Lerp(highAlphaRed, Color.white, t);
            lr.endColor = Color.Lerp(highAlphaRed, Color.white, t);

            yield return new WaitForSeconds(0.01f);
        }

        // Disable icon, collision and set bool to true
        dangerIndicator.SetActive(false);
        animator.SetBool("IsCharging", true);

        float distance = Vector2.Distance(transform.position, targetPos);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, targetPos);

        t = 0; // Reset value of t
        bool isPlayerHit = false;

        while (distance > 1.5f && !Physics2D.CircleCast(transform.position, 0.4f, direction, 0.4f, LayerMask.GetMask("Pit")))
        {
            if (Physics2D.CircleCast(transform.position, 0.4f, direction, 0.4f, LayerMask.GetMask("Obstacles")))
            {
                animator.SetBool("CanMove", true);
                GetComponent<Crowd_Control>().Stun(1.5f);
                yield break;
            }

            // Updates direction and movement
            t += 0.25f;
            direction = (targetPos - transform.position).normalized;
            rb.velocity = direction * Mathf.SmoothStep(1f, chargeSpeed, t);

            // Alpha of line renderer
            lr.startColor = Color.Lerp(Color.white, lowAlphaRed, t);
            lr.endColor = Color.Lerp(Color.white, lowAlphaRed, t);

            RaycastHit2D playerHit = Physics2D.CircleCast(transform.position, chargeHitBox, direction, LayerMask.GetMask("Player")); // Check for player around enemy

            if (playerHit.collider.CompareTag("Player") && !isPlayerHit && playerHit.collider != null)
            {
                playerHit.transform.gameObject.GetComponent<PlayerHealth>().TakeDamage(chargeDmg); // Deal damage
                isPlayerHit = true;
            }

            yield return new WaitForSeconds(chargeUpdateInterval); // Time between steps

            distance = Vector2.Distance(transform.position, targetPos); // Updates distance for the next while loop iteration
        }

        //Enable collisions
        Physics2D.IgnoreLayerCollision(12, 3, false);
        Physics2D.IgnoreLayerCollision(12, 7, false);

        GetComponent<Crowd_Control>().isStunImmune = false;
        GetComponent<Crowd_Control>().Stun(0.2f);
        animator.SetBool("IsCharging", false);
    }

    private IEnumerator ChargeCD()
    {
        canCharge = false;

        yield return new WaitForSeconds(chargeCD);

        canCharge = true;
    }

    // Starts the charge ability if player is in los and range + has ChargeCD rdy
    private void StartCharge(GameObject player)
    {
        Debug.Log($"{Vector2.Distance(player.transform.position, transform.position) <= (chargeRange / 4) * 3} && {IsInLineOfSight(player, animator)} && {canCharge}");
        if (Vector2.Distance(player.transform.position, transform.position) <= (chargeRange / 4) * 3 && IsInLineOfSight(player, animator) && canCharge)
        {
            animator.Play("Charge");
        }
    }

    // Changes 'InAttackRange' bool to if enemy is in attack range
    private void InAttackRange(GameObject player)
    {
        if (Vector2.Distance(player.transform.position, transform.position) <= attackRange && IsInLineOfSight(player, animator))
        { 
            animator.SetBool("InAttackRange", true); 
        }
        else
        {
            animator.SetBool("InAttackRange", false);
        }
    }

    public void StopCharge()
    {
        if (animator.GetBool("IsStunned") || GetComponent<Health>().currentHealth <= 0f)
        {
            if (chargeCoroutine != null) StopCoroutine(chargeCoroutine);
            animator.SetBool("IsCharging", false);
            if (GetComponent<Health>().currentHealth > 0f) animator.SetBool("CanMove", true);
            dangerIndicator.SetActive(false);

            lr.startColor = new Color(0f, 0f, 0f, 0f);
            lr.endColor = new Color(0f, 0f, 0f, 0f);
        }
    }
}
