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
    [SerializeField] private float chargeRange;
    [SerializeField] private float chargeUpTime;
    [SerializeField] private float chargeHitBox;
    [SerializeField] private float chargeSpeed;
    [SerializeField] private float chargeUpdateInterval;
    [SerializeField] private float chargeCD;
    private bool canCharge = true;
    [HideInInspector] public bool chargingCharge; // 10/10 naming
    private Coroutine chargeCoroutine;
    private Collider2D col;
    private LayerMask obstacleLayer;

    private void FixedUpdate()
    {
        StartCharge(player);
        InAttackRange(player); // Player variable is inherited from IEnemyAttack
        StopCharge();
    }

    private void Awake()
    {
        obstacleLayer = LayerMask.GetMask("Obstacles");
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponentInChildren<Animator>();
        animator.SetBool("AttackRDY", true); // Make sure we can attack
        col = GetComponent<Collider2D>();
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
        dangerIndicator.SetActive(true);

        float countdown = Time.time + chargeUpTime;

        while (countdown > Time.time)   // Gives player a headsup
        {
            chargingCharge = true; // Enemy doesn't slide around before charging
            // ** Implement visual element
            yield return null; 
        }

        dangerIndicator.SetActive(false);
        chargingCharge = false; // Deactivate collisions

        // Turn off collisions between charger and enemies + player
        Physics2D.IgnoreLayerCollision(12, 3);
        Physics2D.IgnoreLayerCollision(12, 7);

        Vector3 dir = (player.transform.position - transform.position).normalized; // Direction of player
        Vector3 targetPos = transform.position + 2f * chargeRange * dir;
        float distance = Vector2.Distance(transform.position, targetPos);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        float t = 0;
        bool isPlayerHit = false;

        while (distance > 1.5f && !Physics2D.Raycast(transform.position, dir, 0.5f, obstacleLayer))
        {
            // Updates direction and movement
            t += 0.3f;
            dir = (targetPos - transform.position).normalized;
            //rb.velocity = dir * (chargeSpeed + chargeRange / Mathf.Clamp(distance, 1f, chargeRange)); // Old version
            //rb.velocity = dir * Mathf.SmoothDamp(1f, chargeSpeed, ref velocity, 0.2f, chargeSpeed);
            rb.velocity = dir * Mathf.SmoothStep(1f, chargeSpeed, t);

            RaycastHit2D playerHit = Physics2D.CircleCast(transform.position, chargeHitBox, dir, LayerMask.GetMask("Player")); // Check for player around enemy

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
        if (Vector2.Distance(player.transform.position, transform.position) <= chargeRange && IsInLineOfSight(player, animator) && canCharge)
        {
            animator.SetBool("IsCharging", true);
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
        if (animator.GetBool("IsStunned"))
        {
            if (chargeCoroutine != null) StopCoroutine(chargeCoroutine);
            animator.SetBool("IsCharging", false);
            dangerIndicator.SetActive(false);
        }
    }
}
