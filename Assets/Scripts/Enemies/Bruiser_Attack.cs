using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bruiser_Attack : Enemy_Attack
{
    [SerializeField] private float attackDMG;
    [SerializeField] private float attackTelegraphTime;

    [Header("Wave Ability")]
    [SerializeField] private GameObject segmentPrefab;
    [SerializeField] private float segmentLength;
    [SerializeField] private float segmentWidth;
    [SerializeField] private float segmentGrowthRate;
    [SerializeField] private float segmentAmounts;
    [SerializeField] private float segmentSpawnDelay;
    [SerializeField] private float waveDamage;
    [SerializeField] private float waveCooldown;
    public float chargeDelay;
    public float postFireDelay;
    private int currentSegment = 1;
    private bool isWaveRDY = true;
    private Coroutine waveCDCoroutine;
    private float waveRange;
    private float waveRangeBuffer = 4f;
    private Vector3 startLocation;
    private Vector3 dir = Vector3.zero;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponentInChildren<Animator>();
        animator.SetBool("AttackRDY", true); // Make sure we can attack
        waveRange = segmentLength * segmentAmounts;
    }

    private void OnEnable()
    {
        ResetVariables();
        isWaveRDY = true;
    }

    private void FixedUpdate()
    {
        IsInAttackRange(player); // Player variable is inherited from IEnemyAttack

        if (WaveUsable() && !(animator.GetBool("AttackRDY") && animator.GetBool("InAttackRange")) )
        {
            if (waveCDCoroutine != null) StopCoroutine(waveCDCoroutine);
            waveCDCoroutine = StartCoroutine(WaveCooldown(0.5f));
            animator.Play("ChargeUp");
        }
    }

    public override void Attack() // Called from animator
    {
        StartCoroutine(AttackCD(attackCD)); // Starts cooldown for the attack
        StartCoroutine(TelegraphAttack());
    }

    private IEnumerator TelegraphAttack()
    {
        yield return new WaitForSeconds(attackTelegraphTime);
        if (Vector3.Distance(player.transform.position, transform.position) <= attackRange) player.GetComponent<PlayerHealth>().TakeDamage(attackDMG); // Deal damage
    }

    private bool IsInAttackRange(GameObject player)
    {
        bool inRange = Vector2.Distance(player.transform.position, transform.position) <= attackRange && IsInLineOfSight(player, animator); // In attack range & los
        animator.SetBool("InAttackRange", inRange);
        return inRange;
    }

    
    public bool WaveUsable(bool isWaveBeingCharged = false)
    {
        // In range, los and not stunned
        if (isWaveBeingCharged) return Vector2.Distance(player.transform.position, transform.position) <= waveRange + waveRangeBuffer && IsInLineOfSight(player, animator) && !animator.GetBool("IsStunned");

        // In In range, los, not stunned and ability rdy
        return Vector2.Distance(player.transform.position, transform.position) <= waveRange && IsInLineOfSight(player, animator) && !animator.GetBool("IsStunned") && isWaveRDY;
    }

    public void ThrowWave()
    {
        // Start cooldown
        if (waveCDCoroutine != null) StopCoroutine(waveCDCoroutine);
        waveCDCoroutine = StartCoroutine(WaveCooldown());

        if (currentSegment > segmentAmounts) { ResetVariables(); return; } // Guard clause

        if (currentSegment == 1)
        {
            startLocation = transform.position;
            dir = (player.transform.position - transform.position).normalized;
        }

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        GameObject segment = Instantiate(segmentPrefab, transform.position, Quaternion.AngleAxis(angle, Vector3.forward));
        segment.transform.position = startLocation + dir * (currentSegment * segmentLength - (segmentLength / 2) );
        segment.transform.localScale = new Vector3(segmentLength, segmentWidth * (1 + segmentGrowthRate * currentSegment), 1f);
        var ps = segment.GetComponentInChildren<ParticleSystem>().shape; // Shape of particle emitter
        ps.scale = new Vector3(segmentLength, segmentWidth * (1 + segmentGrowthRate * currentSegment), 1f); // Set scale of emitter

        StartCoroutine(segment.GetComponent<SegmentScript>().LerpAlphaIn(angle, waveDamage)); 

        StartCoroutine(SegmentDelay());
    }

    private IEnumerator SegmentDelay()
    {
        if (currentSegment > segmentAmounts) { ResetVariables(); yield break; } // Guard clause
        yield return new WaitForSeconds(attackTelegraphTime);
        currentSegment++;
        ThrowWave();
    }

    private IEnumerator WaveCooldown(float multiplier = 1f)
    {
        isWaveRDY = false;

        yield return new WaitForSeconds(waveCooldown * multiplier);

        isWaveRDY = true;
    }

    private void ResetVariables()
    {
        currentSegment = 1;
        if (animator != null) { animator.SetBool("AttackRDY", true); } // Resets variable when respawning
    }
}