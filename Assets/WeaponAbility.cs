using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponAbility : MonoBehaviour
{
    [Header("Music")]
    [Range(0, 1)] public float musicVolume = 1f;
    public AudioClip bossTrack;
    private bool isPlaying = false;

    [Header("SFX")]
    [Range(0, 1)] public float sfxVolume = 1f;
    [SerializeField] protected AudioClip sprintSFX;
    [SerializeField] protected AudioClip fireSFX;
    [SerializeField] protected AudioClip gungnirThrow;

    [Header("UI")]
    public GameObject bossHealthBar;

    [Header("Abilities")]
    [SerializeField] private LayerMask obstacles;
    [SerializeField] private float abilityCD;
    [SerializeField] private float castTime;
    [SerializeField] private float abilityRange;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDamage;
    [SerializeField] private float dashHitBox;
    [SerializeField] private float gungnirSideSpearAngle;
    [SerializeField] private float gungnirHitBox;
    public GameObject mjoelnir;
    [SerializeField] private GameObject firePrefab;
    [SerializeField] private GameObject gungnirPrefab;
    [SerializeField] private GameObject gungnirSidePrefab;
    [SerializeField] private List<LineRenderer> lr = new List<LineRenderer>();
    private GameObject player;
    private Vector3 targetPos;
    private bool hasDash = false;
    private bool hasGungnir = false;
    private bool isAbilityReady = true;

    public GameObject weaponPickUp;

    private List<GameObject> cleanUpOnDeathList = new List<GameObject>();

    private Animator animator;

    private void Awake()
    {
        if (GameObject.Find("HermesFightIntroDialogue") != null) GameObject.Find("HermesFightIntroDialogue").GetComponent<Dialogue>().StartDialogue();
    }

    private void Start()
    {
        player = GameObject.Find("Player");
        animator = GetComponentInChildren<Animator>();

        isPlaying = false;
        if (bossHealthBar != null) bossHealthBar.SetActive(true);
        SetAbilities();
    }

    private void FixedUpdate()
    {
        if (!isPlaying) { isPlaying = true; MusicManager.singleton.PlayMusic(bossTrack, musicVolume); }

        if (isAbilityReady && !animator.GetBool("IsBusy")) CastAbility();
    }

    private void SetAbilities()
    {
        switch (ExtractNumberFromName(SceneManager.GetActiveScene().name))
        {
            case 1:
                hasDash = true;
                break;
            case 2:
                cleanUpOnDeathList.Add(Instantiate(mjoelnir, new Vector3(transform.position.x, transform.position.y, 0f), transform.rotation));
                break;
            case 3:
                hasGungnir = true;
                break;
            case 4:
                hasDash = true;
                cleanUpOnDeathList.Add(Instantiate(mjoelnir, new Vector3(transform.position.x, transform.position.y, 0f), transform.rotation));
                hasGungnir = true;
                break;
            default:
                break;
        }
    }

    private void CastAbility()
    {
        if (!hasDash && !hasGungnir) return;

        animator.SetBool("IsBusy", true);

        switch (Random.Range(0,2))
        {
            case 0:
                if (hasDash)
                {
                    StartCoroutine(CastDash());
                }
                break;
            case 1:
                if (hasGungnir)
                {
                    StartCoroutine(CastGungnir());
                }
                break;
            default:
                animator.SetBool("IsBusy", false);
                break;
        }
    }

    private IEnumerator CastDash()
    {
        yield return DrawLine(dashHitBox);

        animator.Play("Dash");
        StartCoroutine(DashToNewPos(targetPos));
    }

    private IEnumerator DashToNewPos(Vector3 newPos)
    {
        // Variables
        Vector3 startPos = transform.position;
        Vector3 dir = (newPos - startPos).normalized;
        float distance = Vector2.Distance(startPos, newPos);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        float lastFire = Time.time;
        bool hasPlayed = false;
        bool isStuck = false;

        SFXManager.singleton.PlaySound(sprintSFX, transform.position, sfxVolume, transform);

        float t = 0;

        // Move to new position
        while (distance > 1.5f && !isStuck)
        {
            // Move through obstacles if any are encountered
            while (Physics2D.CircleCast(transform.position, 0.6f, dir, 0.6f, obstacles))
            {
                isStuck = true;
                break;
            }

            // Updates direction and movement
            dir = (newPos - transform.position).normalized;
            rb.velocity = dir * dashSpeed;

            // Spawn fire
            if (lastFire + 0.04f < Time.time) { Instantiate(firePrefab, transform.position, transform.rotation); lastFire = Time.time; }

            // Play sound
            if (!hasPlayed) { hasPlayed = true; SFXManager.singleton.PlaySound(fireSFX, transform.position, sfxVolume); }

            RaycastHit2D hit = Physics2D.CircleCast(transform.position, dashHitBox, Vector2.zero, dashHitBox, LayerMask.GetMask("Player"));
            if (hit) hit.transform.GetComponent<PlayerHealth>().TakeDamage(dashDamage);

            yield return new WaitForSeconds(0.02f); // Time between steps

            // Updates distance for the next while loop iteration
            distance = Vector2.Distance(transform.position, newPos);
        }

        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(0.2f);

        EndAbility();
    }

    private IEnumerator CastGungnir()
    {
        StartCoroutine(DrawLine(gungnirHitBox, 100f, gungnirSideSpearAngle, 2));
        StartCoroutine(DrawLine(gungnirHitBox, 100f, -gungnirSideSpearAngle, 1));
        yield return DrawLine(gungnirHitBox, 100f, 0f, 0);

        animator.Play("Gungnir");
        StartCoroutine(ThrowGungnir());
    }

    private IEnumerator ThrowGungnir()
    {
        Vector2 dir = (targetPos - transform.position).normalized;

        GameObject gungnir = Instantiate(gungnirPrefab, transform.position, transform.rotation);
        gungnir.GetComponent<Hermes_Gungnir>().SetDirection(dir);

        // Instantiate and throw the additional spears
        GameObject spear1 = Instantiate(gungnirSidePrefab, transform.position, transform.rotation);
        spear1.GetComponent<Hermes_Gungnir_Side>().SetDirection(RotateVector(dir, gungnirSideSpearAngle));

        GameObject spear2 = Instantiate(gungnirSidePrefab, transform.position, transform.rotation);
        spear2.GetComponent<Hermes_Gungnir_Side>().SetDirection(RotateVector(dir, -gungnirSideSpearAngle));

        yield return new WaitForSeconds(0.2f);

        EndAbility();
    }

    private IEnumerator DrawLine(float targetWidth, float rangeMultiplier = 1f, float angleOffset = 0f, int lineIndex = 0)
    {
        animator.Play("ChargeUp");

        Color lowAlphaRed = new Color(1f, 0f, 0f, 0f);
        Color highAlphaRed = new Color(1f, 0f, 0f, 0.7f);
        float totalTicks = 60f;
        float yieldDuration = (castTime - 0.1f) / totalTicks;
        float t = 0f;
        Vector3 direction = RotateVector((player.transform.position - transform.position).normalized, angleOffset);
        targetPos = player.transform.position + rangeMultiplier * abilityRange * direction;

        // Slow increase of alpha and size of line
        while (t < 1f)
        {

            Vector2 dir = (player.transform.position - transform.position).normalized; // Look to player

            if (dir.x > 0.24f) // Right
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
            else if (dir.x < -0.24f) // Left
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
            }

            t += 1f / totalTicks;

            // Alpha of line renderer
            lr[lineIndex].startColor = Color.Lerp(lowAlphaRed, highAlphaRed, t);
            lr[lineIndex].endColor = Color.Lerp(lowAlphaRed, highAlphaRed, t);

            // Set start & end pos of line renderer
            direction = RotateVector((player.transform.position - transform.position).normalized, angleOffset);
            targetPos = transform.position + direction * (abilityRange * rangeMultiplier);
            lr[lineIndex].SetPosition(0, transform.position);
            lr[lineIndex].SetPosition(1, Vector3.Lerp(transform.position, targetPos, t));

            // Set width
            lr[lineIndex].widthMultiplier = t * targetWidth;

            yield return new WaitForSeconds(yieldDuration);
        }

        t = 0f; // Resets value of t

        // Lerp color to white
        while (t < 1f)
        {
            t += 0.15f;

            lr[lineIndex].startColor = Color.Lerp(highAlphaRed, Color.white, t);
            lr[lineIndex].endColor = Color.Lerp(highAlphaRed, Color.white, t);

            yield return new WaitForSeconds(0.01f);
        }

        lr[lineIndex].startColor = new Color(0f, 0f, 0f, 0f);
        lr[lineIndex].endColor = new Color(0f, 0f, 0f, 0f);
    }

    private Vector2 RotateVector(Vector2 direction, float angleOffset)
    {
        float angleRad = angleOffset * Mathf.Deg2Rad; // Convert angle to radians
        float cosAngle = Mathf.Cos(angleRad);
        float sinAngle = Mathf.Sin(angleRad);

        float x = direction.x * cosAngle - direction.y * sinAngle;
        float y = direction.x * sinAngle + direction.y * cosAngle;

        return new Vector2(x, y);
    }

    private void EndAbility()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        animator.SetBool("IsBusy", false);
        animator.Play("Idle");

        StartCoroutine(WeaponCooldown());
    }

    private int ExtractNumberFromName(string name)
    {
        Match match = Regex.Match(name, @"[1-9]");
        if (match.Success)
        {
            string numberString = match.Value;
            int number;
            if (int.TryParse(numberString, out number))
            {
                return number;
            }
        }
        Debug.LogWarning("Failed to extract number from name: " + name);
        return -1; // Return a default value or handle the error as needed
    }

    private IEnumerator WeaponCooldown()
    {
        isAbilityReady = false;

        yield return new WaitForSeconds(abilityCD);

        isAbilityReady = true;
    }

    private void OnDisable()
    {
        foreach (var obj in cleanUpOnDeathList)
        {
            Destroy(obj);
        }
    }
}
