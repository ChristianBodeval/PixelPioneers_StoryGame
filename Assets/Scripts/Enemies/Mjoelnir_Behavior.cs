using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Mjoelnir_Behavior : MonoBehaviour
{
    [Header("General for Pathfinding")]
    [SerializeField] private Transform parentTransform;
    [SerializeField] private float speed = 3f;
    private float attackRange = 1.2f;
    private GameObject player;
    private Rigidbody2D rb;

    [Header("A*")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float updateInterval = 0.1f;
    [SerializeField] private float nextWayPointDistance = 2f;
    private Path path;
    private int currentWayPoint = 0;
    private Seeker seeker;

    [Header("Custom Behavior")]
    [SerializeField] private float specialCooldown;
    [SerializeField] private float castTime = 0.3f;
    private bool abilityRDY = false;
    private Coroutine abilityCDFunction;
    private bool canSpin = true;

    [Header("Hammer Movement")]
    [SerializeField] private float spinDMG;
    [SerializeField] private float spinRadius;
    [SerializeField] private float spinSpeed;
    [SerializeField] private float spriteRotationSpeed;
    [SerializeField] private GameObject mjoelnirSprite;
    [SerializeField] private float onCollisionFreezeDuration;
    [SerializeField] private float collisionFreezeCD; // Cannot be held still forever
    private float timeOfPlayerHit;
    private bool onFreezeCD = false;
    private Vector3 freezeLocation;

    [Header("Charge")]
    [SerializeField] private float chargeDMG;
    [SerializeField] private float chargeSpeed;
    public AnimationCurve accelerationCurve;
    [SerializeField] private float baseHitboxSize;
    [SerializeField] private float hitboxWidthMultiplier = 0.1f;
    [SerializeField] private float buttonChargeUpRate = 10;
    [SerializeField] private float chargeUpdateInterval = 0.05f;
    [SerializeField] private float maxCharge;
    [SerializeField] private GameObject rangeIndicator;
    [SerializeField] private GameObject chargeParticles;
    private bool isCharging = false;
    private bool isChargingCharge = false;
    private float charge = 0f;

    [Header("Area Of Effect")]
    [SerializeField] private float aoeDMG;
    [SerializeField] private float aoeRadius = 3f;
    [SerializeField] private GameObject aoeAnim;
    [SerializeField] private GameObject aoeIndicator;

    private void Start()
    {
        mjoelnirSprite = GetComponentInChildren<SpriteRenderer>().gameObject;
        obstacleLayer = LayerMask.GetMask("Obstacles");
        player = GameObject.FindWithTag("Player");
        seeker = GetComponent<Seeker>();
        rb = GetComponentInParent<Rigidbody2D>();
        StartCoroutine(AbilityCD(2f)); // Dont have special ready right away

        InvokeRepeating("UpdatePath", 0f, updateInterval); // Updates pathfinding regularly
    }

    private void Update()
    {
        Spin();
    }

    // Spins the hammer around
    private void Spin()
    {
        if (!canSpin && onFreezeCD && !isCharging) // Guard clause
        {
            transform.position = freezeLocation;
            return;
        }
        else if (!canSpin)
        {
            return;
        }

        transform.position = parentTransform.position + (transform.right + transform.up) * spinRadius;
        mjoelnirSprite.transform.Rotate(0f, 0f, spriteRotationSpeed);
    }

    private void FixedUpdate()
    {       
        // Move closer if not in range
        if (TargetNotAttackable() && !isChargingCharge)
        {
            PathFollow();
        }
        else
        {
            rb.velocity = new Vector3(0f,0f,0f);
        }

        // Spin hammer around player
        if (canSpin)
        {
            transform.RotateAround(parentTransform.position, new Vector3(0f, 0f, 1f) * spinRadius, spinSpeed);
            freezeLocation = transform.position;
        }

        if (abilityRDY) UseSpecial();
    }

    private void Move(Vector2 dir)
    {
        if (!isCharging)
        {
            rb.velocity = speed * dir; // Movement
        }
        else if (isChargingCharge)
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(parentTransform.position, player.transform.position, OnPathComplete);
        }
    }

    private void PathFollow()
    {
        // Guard clause
        if (path == null || currentWayPoint >= path.vectorPath.Count) { return; } // Is not there yet and has a path

        // Move towards player
        Vector2 direction = (path.vectorPath[currentWayPoint] - parentTransform.position).normalized;
        Move(direction);

        // Swaps early to new waypoint
        float distance = Vector2.Distance(parentTransform.position, path.vectorPath[currentWayPoint]);

        // Next waypoint in the array
        if (distance < nextWayPointDistance)
        {
            currentWayPoint++; 
        }
    }

    private bool TargetNotAttackable()
    {
        float dis = Vector2.Distance(parentTransform.position, player.transform.position);

        // Return true if terrain is in the way
        if (Physics2D.Raycast(parentTransform.position, player.transform.position - parentTransform.position, attackRange, obstacleLayer))
        {
            return true;
        }
        return dis > attackRange; // Return true if we are in range and not in attackrange
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWayPoint = 1;
        }
    }

    private IEnumerator FreezeSpin()
    {
        onFreezeCD = true;
        canSpin = false;

        yield return new WaitForSeconds(onCollisionFreezeDuration);

        canSpin = true;

        yield return new WaitForSeconds(collisionFreezeCD);

        onFreezeCD = false;
    }

    private bool IsPlayerReadyForHit()
    {
        if (canSpin)
        {
            bool isReady = timeOfPlayerHit + onCollisionFreezeDuration * 2 > Time.time; // Is still on not rdy to be hit again
            if (isReady) timeOfPlayerHit = 0f;
            return isReady;
        }
        return false;
    }

    private void EnableHammer()
    {
        isCharging = false;
        canSpin = true;    // Allow the hammer to spin again
        GetComponent<CircleCollider2D>().enabled = true;    // Player can move again
    }

    private void DisableHammer()
    {
        isCharging = true;
        canSpin = false;    // Stop hammer's spin
        GetComponent<CircleCollider2D>().enabled = false;   // Cannot hit enemies with hammer sprite
    }

    private void UseSpecial()
    {
        // Random between charge and aoe ability
        /*switch (Random.Range(0,2))
        {
            case 0: // Charge ability, is it in range
                if (Physics2D.Raycast(parentTransform.position, player.transform.position - parentTransform.position, maxCharge - 1f, obstacleLayer))
                {
                    StartCoroutine(AbilityCD(specialCooldown));
                    StartCoroutine(ChargeAbility());
                }
                break;

            case 1: // Aoe abilitym, is it in range
                if (Physics2D.Raycast(parentTransform.position, player.transform.position - parentTransform.position, aoeRadius - 1f, obstacleLayer))
                {
                    StartCoroutine(AbilityCD(specialCooldown));
                    StartCoroutine(AOEAbility());
                }
                break;

            default:
                break;
        }
        */

        if (Physics2D.Raycast(transform.position, player.transform.position - transform.position, maxCharge - (maxCharge / 4), obstacleLayer))
        {
            StartCoroutine(AbilityCD(specialCooldown));
            StartCoroutine(ChargeAbility());
        }
    }

    private IEnumerator ChargeAbility()
    {
        DisableHammer();

        isChargingCharge = true;
        // Position the hammer on parent
        parentTransform.position = transform.position;
        Vector3 direction = Vector2.zero;
        SpriteRenderer sr = rangeIndicator.GetComponent<SpriteRenderer>();
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f); // Not visible
        rangeIndicator.SetActive(true);
        rangeIndicator.transform.localScale = new Vector2(1f, 1f);
        charge = 0f; // Resets value
        float a = 0f;


        // Slow increase of alpha and size of cirle
        while (a < 1f)
        {
            charge += maxCharge / 50;
            a += 1f / 50;

            // Alpha
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, accelerationCurve.Evaluate(a) * 0.5f);
            

            // Charge range indicator - change its size and rotation
            rangeIndicator.transform.localScale = new Vector3(charge, baseHitboxSize + (charge * hitboxWidthMultiplier) - 0.3f, 1f); // Sets the length - chargeHitbox * 2 - 0.2f is the diameter of the indicator -0.2f is such that the player feels cheated of a hit less often
            direction = (player.transform.position - transform.position).normalized;
            rangeIndicator.transform.position = transform.position + direction * (charge / 2); // Move indicator
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rangeIndicator.gameObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // Mjoelnir sprite
            mjoelnirSprite.transform.rotation = Quaternion.AngleAxis(angle + -135f, Vector3.forward);   // Point hammer away from player

            yield return new WaitForSeconds(castTime/1.4f / 50f);
        }

        yield return new WaitForSeconds(0.05f);

        // Reset variables
        rangeIndicator.SetActive(false);

        // Start charge
        isChargingCharge = false;
        isCharging = true;
        StartCoroutine(PointHammerForwards(direction)); // Faces hammer forward while player is charging forwards
        StartCoroutine(Charge(direction, maxCharge));
    }

    // CHARGE function
    private IEnumerator Charge(Vector2 dir, float chargedAmount)
    {
        
        float startTime = Time.time; // Used for acceleration curve

        Vector3 targetPos = (Vector2)parentTransform.position + dir * chargedAmount;
        float distance = Vector2.Distance((Vector2)parentTransform.position, targetPos);

        // Enable particles
        chargeParticles.SetActive(true);

        // Variables
        bool isPlayerHit = false;
        float slowDown = 1f;
        float speed = 1f; // Default of 1 such that it has no effect if we are beyond acceleration
        float elapsed = 0f;

        while (distance > 1f && !Physics2D.CircleCast(parentTransform.position, 0.3f, dir, 0.3f, obstacleLayer))
        {
            dir = (targetPos - parentTransform.position).normalized;

            if (elapsed < 0.2f)
            {
                elapsed += Time.time - startTime;
                speed = accelerationCurve.Evaluate(elapsed * 5);
            }

            if (distance < 2f) slowDown = (targetPos - parentTransform.position).magnitude / 2f;

            rb.velocity = chargeSpeed * speed * dir * slowDown; // Move towards targetpos

            // Hitting enemies and the consequences

            if (!isPlayerHit)
            {
                isPlayerHit = CheckForPlayer(baseHitboxSize + (chargedAmount * hitboxWidthMultiplier), dir);
                if (isPlayerHit)
                {
                    player.GetComponent<PlayerHealth>().TakeDamage(chargeDMG);
                    rb.velocity = Vector2.zero;
                    break; // Break out of the while loop
                }
            }

            yield return new WaitForSeconds(chargeUpdateInterval); // Time between steps

            distance = Vector2.Distance(parentTransform.position, targetPos); // Update distance for next loop iteration
        }

        parentTransform.position = transform.position;
        transform.position = parentTransform.position;

        // Disable particles
        chargeParticles.SetActive(false);
        isCharging = false;

        /*
        // Enable movement and spin
        
        EnableHammer(); // Hammer can hit enemies again
        */
        StartCoroutine(AOEAbility());
    }

    private IEnumerator PointHammerForwards(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;                // Angle for pointing to player
        mjoelnirSprite.transform.rotation = Quaternion.AngleAxis(angle + -135f, Vector3.forward);   // Point hammer away from player

        while (isCharging)
        {
            // Points hammer and player in direction
            transform.position = parentTransform.position; // Position hammer in front of player

            yield return null;
        }
    }

    // AoE ability
    private IEnumerator AOEAbility()
    {
        DisableHammer();
        rb.velocity = Vector2.zero;
        parentTransform.position = transform.position;
        transform.position = parentTransform.position;

        // Instantiate circle
        SpriteRenderer sr = aoeIndicator.GetComponent<SpriteRenderer>();
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f); // Not visible
        aoeIndicator.SetActive(true);
        aoeIndicator.transform.localScale = new Vector2(1f, 1f);
        float a = 0f;


        // Slow increase of alpha and size of cirle
        while (a < 1f)
        {
            a += 0.1f;
            aoeIndicator.transform.localScale = new Vector2(a * aoeRadius * 2f, a * aoeRadius * 2f); // Scale size over time
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, accelerationCurve.Evaluate(a) * 0.5f);
            yield return new WaitForSeconds(castTime / 10);
        }

        aoeIndicator.SetActive(false);

        // Toggle aoe anim
        Instantiate(aoeAnim, transform);

        // Deal damage
        bool isPlayerHit = CheckForPlayer(aoeRadius - 0.2f, Vector2.right);
        if (isPlayerHit) player.GetComponent<PlayerHealth>().TakeDamage(aoeDMG);

        yield return new WaitForSeconds(castTime);

        EnableHammer();
        StartCoroutine(LerpToSpinRadius());
    }

    private bool CheckForPlayer(float radius, Vector2 direction)
    {
        return Physics2D.CircleCast(transform.position, radius, direction, radius, LayerMask.GetMask("Player"));
    }

    private IEnumerator LerpToSpinRadius()
    {
        float temp = spinRadius;
        spinRadius = 0.2f;

        while (spinRadius < temp)
        {
            //if (isCharging) { spinRadius = temp; yield break; } // Break if we are starting a new ability
            spinRadius += Time.deltaTime;
            yield return null;
        }

        spinRadius = temp;
    }

    // Puts ability on cd and is reset after a duration
    private IEnumerator AbilityCD(float cooldown)
    {
        abilityRDY = false;

        yield return new WaitForSeconds(cooldown);

        abilityRDY = true;
    }

    // Damage player when colliding
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player") && IsPlayerReadyForHit())
        {
            timeOfPlayerHit = Time.time;
            col.gameObject.GetComponent<PlayerHealth>().TakeDamage(spinDMG);
            if (!onFreezeCD) StartCoroutine(FreezeSpin());
        }
    }
}