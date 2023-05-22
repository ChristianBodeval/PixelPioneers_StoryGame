using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bruiser_Attack : Enemy_Attack
{
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
    private bool isWaveRDYCloseQuaters = false; // Used to get a quicker attack if we are very close to the player
    private Coroutine waveCDCoroutine;
    public float waveRange { get; private set; }
    private Vector3 startLocation;
    private Vector3 dir = Vector3.zero;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponentInChildren<Animator>();
        ResetVariables();
        waveRange = segmentLength * segmentAmounts;
    }

    private void OnEnable()
    {
        if (animator != null) ResetVariables();
        isWaveRDY = true;
    }

    private void FixedUpdate()
    {
        if (WaveUsable())
        {
            if (waveCDCoroutine != null) StopCoroutine(waveCDCoroutine);
            waveCDCoroutine = StartCoroutine(WaveCooldown(0.5f));
            animator.Play("ChargeUp");
        }
    }

    public bool WaveUsable()
    {
        bool isRDYCloseQuaters = (Vector2.Distance(player.transform.position, transform.position) < 1f && isWaveRDYCloseQuaters) ? true : false;
        if (isRDYCloseQuaters) return true;

        // In range, los and and has CD
        return Vector2.Distance(player.transform.position, transform.position) <= waveRange / 2 && IsInLineOfSight(player, animator) && isWaveRDY;
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
        StartCoroutine(segment.GetComponent<SegmentScript>().LerpAlphaIn(angle, waveDamage, gameObject));
        segment.transform.position = startLocation + dir * (currentSegment * segmentLength - (segmentLength / 2) );
        segment.transform.localScale = new Vector3(segmentLength, segmentWidth * (1 + segmentGrowthRate * currentSegment), 1f);
        var ps = segment.GetComponentInChildren<ParticleSystem>().shape; // Shape of particle emitter
        ps.scale = new Vector3(segmentLength, segmentWidth * (1 + segmentGrowthRate * currentSegment), 1f); // Set scale of emitter

        StartCoroutine(SegmentDelay());
    }

    private IEnumerator SegmentDelay()
    {
        if (currentSegment > segmentAmounts) { ResetVariables(); yield break; } // Guard clause
        yield return new WaitForSeconds(segmentSpawnDelay);
        currentSegment++;
        ThrowWave();
    }

    private IEnumerator WaveCooldown(float multiplier = 1f)
    {
        isWaveRDY = false;
        isWaveRDYCloseQuaters = false;

        yield return new WaitForSeconds((waveCooldown / 4) * multiplier);

        isWaveRDYCloseQuaters = true;

        yield return new WaitForSeconds((waveCooldown / 4 ) * 3 * multiplier);

        isWaveRDY = true;
    }

    private void ResetVariables()
    {
        currentSegment = 1;
        if (animator != null) { animator.SetBool("AttackRDY", true); animator.SetBool("CanMove", true); } // Resets variable when respawning
    }
}