using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hermes_Attack : Enemy_Attack
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
    [HideInInspector] public static float waveRange { get; private set; }
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
        if (animator != null) ResetVariables();
    }

    private void FixedUpdate()
    {
        if (IsWaveUsable())
        {
            if (waveCDCoroutine != null) StopCoroutine(waveCDCoroutine);
            waveCDCoroutine = StartCoroutine(WaveCooldown(0.5f));
            animator.Play("Hermes_Attack_ChargeUp");
        }
    }
    
    public bool IsWaveUsable()
    {
        // Is something else being done by Hermes
        if (animator.GetBool("IsBusy") || animator.GetBool("IsStunned") || !animator.GetBool("AttackRDY") || animator.GetCurrentAnimatorStateInfo(0).IsTag("Immobile") || animator.GetCurrentAnimatorStateInfo(0).IsTag("Special")) return false;

        // In range and los
        return Vector2.Distance(player.transform.position, transform.position) <= waveRange && IsInLineOfSight(player, animator);
    }

    public void ThrowWave()
    {
        // Start cooldown
        if (waveCDCoroutine != null) StopCoroutine(waveCDCoroutine);
        waveCDCoroutine = StartCoroutine(WaveCooldown());

        if (currentSegment > segmentAmounts) { currentSegment = 1; return; } // Guard clause

        if (currentSegment == 1)
        {
            startLocation = transform.position;
            dir = (player.transform.position - transform.position).normalized;
        }

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        GameObject segment = Instantiate(segmentPrefab, transform.position, Quaternion.AngleAxis(angle, Vector3.forward));
        StartCoroutine(segment.GetComponent<SegmentScript>().LerpAlphaIn(angle, waveDamage, gameObject));
        segment.transform.position = startLocation + dir * (currentSegment * segmentLength - (segmentLength / 2) );
        segment.transform.localScale = new Vector3(segmentLength, segmentWidth * (1 + segmentGrowthRate * currentSegment), 1f);
        var ps = segment.GetComponentInChildren<ParticleSystem>().shape; // Shape of particle emitter
        ps.scale = new Vector3(segmentLength, segmentWidth * (1 + segmentGrowthRate * currentSegment), 1f); // Set scale of emitter

        StartCoroutine(SegmentDelay());
    }

    private IEnumerator SegmentDelay()
    {
        if (currentSegment > segmentAmounts) { currentSegment = 1; yield break; } // Guard clause
        yield return new WaitForSeconds(attackTelegraphTime);
        currentSegment++;
        ThrowWave();
    }

    private IEnumerator WaveCooldown(float multiplier = 1f)
    {
        animator.SetBool("AttackRDY", false);

        yield return new WaitForSeconds(waveCooldown * multiplier);

        animator.SetBool("AttackRDY", true);
    }

    private void ResetVariables()
    {
        currentSegment = 1;
        if (animator != null)
        {
            animator.SetBool("AttackRDY", true);
            animator.SetBool("CanMove", true);
            animator.SetBool("IsChargedUp", false);
            animator.SetBool("IsStunned", false);
            animator.SetBool("IsBusy", false);
        }
    }
}