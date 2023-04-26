using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Mjoelnir : MonoBehaviour, IUpgradeable
{
    [Header("Hammer Movement")]
    [SerializeField] private float spinDMG;
    [SerializeField] private float spinRadius;
    [SerializeField] private float spinSpeed;
    [SerializeField] private float spriteRotationSpeed;
    [SerializeField] private GameObject mjoelnirSprite;
    [SerializeField] private float onCollisionFreezeDuration;
    [SerializeField] private float collisionFreezeCD; // Cannot be held still forever
    [SerializeField] private LayerMask enemyLayers;
    [HideInInspector] public static Dictionary<GameObject, float> cannotHitList = new Dictionary<GameObject, float>();
    private bool onFreezeCD = false;
    private Vector3 freezeLocation;
    private GameObject player;
    
    [Header("Charge Upgrade")]
    public bool hasChargeUpgrade = true;
    [SerializeField] private float chargeDMG;
    [SerializeField] private float chargeSpeed;
    [SerializeField] private float slowAmountWhileCharging;
    public AnimationCurve accelerationCurve;
    [SerializeField] private float baseHitboxSize;
    [SerializeField] private float hitboxWidthMultiplier = 0.1f;
    public float chargeCD;
    [SerializeField] private float buttonChargeUpRate = 10;
    [SerializeField] private float chargeUpdateInterval = 0.05f;
    [SerializeField] private float minCharge;
    [SerializeField] private float maxCharge;
    [SerializeField] private float stunDuration = 1f;
    [SerializeField] private GameObject rangeIndicator;
    private bool isCharging = false;
    private bool isCharghingCharge = false;
    private Coroutine initCharge;
    private float charge = 0f;
    public WeaponCDs weaponCDVisual;

    [Header("Area Of Effect Upgrade")]
    public bool hasAreaOfEffectUpgrade = false;
    [SerializeField] private float aoeDMG;
    [SerializeField] private float castTime = 0.3f;
    [SerializeField] private float aoeRadius = 3f;
    [SerializeField] private float areaOfEffectCD;
    [SerializeField] private GameObject aoeIndicator;

    private LayerMask obstacleLayer;

    private bool abilityRDY = true;
    private Coroutine abilityCDFunction;
    private bool canSpin = true;


    private void Start()
    {
        mjoelnirSprite = GetComponentInChildren<SpriteRenderer>().gameObject;
        player = GameObject.FindGameObjectWithTag("Player");

        obstacleLayer = LayerMask.GetMask("Obstacles");
    }

    private void Update()
    {
        Spin();

        ChargeAbility();

        AOEAbility();
    }

    private void FixedUpdate()
    {
        if (canSpin)
        {
            transform.RotateAround(player.transform.position, new Vector3(0f, 0f, 1f), spinSpeed);
            freezeLocation = transform.position;
        }
    }

    // Spins the hammer around the player
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

        transform.position = player.transform.position + (transform.right + transform.up) * spinRadius;
        mjoelnirSprite.transform.Rotate(0f, 0f, spriteRotationSpeed);
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

    private void AddToSpinCDList(GameObject e)
    {
        if (!cannotHitList.ContainsKey(e)) cannotHitList.Add(e, Time.time + onCollisionFreezeDuration * 2f);
    }

    public void RemoveFromSpinCDList(GameObject e)
    {
        cannotHitList.Remove(e);
    }

    private bool IsInList(GameObject e)
    {
        if (cannotHitList.ContainsKey(e) && canSpin)
        {
            bool isInList = cannotHitList[e] + onCollisionFreezeDuration * 2 > Time.time; // Is still on not rdy to be hit again
            if (!isInList) cannotHitList.Remove(e); // If its rdy we remove from list
            return isInList;
        }
        return false; // Not in list anymore
    } 

    private void EnableHammer()
    {
        isCharging = false;
        canSpin = true;    // Allow the hammer to spin again
        GetComponent<CircleCollider2D>().enabled = true;    // Player can move again
    }

    private void DisableHammer()
    {
        canSpin = false;    // Stop hammer's spin
        GetComponent<CircleCollider2D>().enabled = false;   // Cannot hit enemies with hammer sprite
    }

    private void ChargeAbility()
    {
        if (!hasChargeUpgrade) { return; }                  // Checks if we have the upgrade

        // Hold button
        if ((Input.GetButton("Fire2") || isCharghingCharge) && abilityRDY)         // 'K' button held charges the ability, note you also need to have the cd ready
        {
            DisableHammer();
            if (initCharge != null) StopCoroutine(initCharge);
            initCharge =  StartCoroutine(InitialCharging());
            player.GetComponent<PlayerAction>().StartSlow(slowAmountWhileCharging); // Root the player while casting
            player.GetComponent<PlayerAction>().CannotDash();

            // Position the hammer on player
            transform.position = player.transform.position;
            mjoelnirSprite.transform.Rotate(0f, 0f, spriteRotationSpeed);

            charge += Time.deltaTime * buttonChargeUpRate;  // Charge value
            if (charge > maxCharge) // Cannot exceed max value
            {
                charge = maxCharge;
            }
            else // While we are increasing charge value we zoom out
            {
                float startValue = 5f;
                float endValue = 8f;
                float t = charge / endValue;
                float zoomAmount = Mathf.Lerp(startValue, endValue, Mathf.Pow(t, 1f / 3f)); // Cube root function - slows down zooming over time
                Camera.main.GetComponent<CameraScript>().SetZoomAmount(zoomAmount);
            }        

            // Charge range indicator - change its size and rotation
            rangeIndicator.GetComponent<SpriteRenderer>().color = new Color32(180, 180, 0, 180); ;
            rangeIndicator.transform.localScale = new Vector3(charge, baseHitboxSize + (charge * hitboxWidthMultiplier) - 0.3f, 1f); // Sets the length - chargeHitbox * 2 - 0.2f is the diameter of the indicator -0.2f is such that the player feels cheated of a hit less often
            Vector2 direction = player.GetComponent<PlayerAction>().lastFacing;
            rangeIndicator.transform.position = player.transform.position + (Vector3)direction * (charge / 2); // Move indicator
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rangeIndicator.gameObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        // Release button
        if (Input.GetButtonUp("Fire2") && abilityRDY)
        {
            StartCoroutine(ReleaseCharge());
        }
    }

    // Coroutine for suspending execution in a while loop
    private IEnumerator ReleaseCharge()
    {
        while (isCharghingCharge) { yield return null; } // Suspends charge until we have charged for a minimum amount

        player.GetComponent<PlayerAction>().StopMove();
        player.GetComponent<PlayerHealth>().AddInvulnerability(); // Cannot be hit during charge

        Vector2 direction = player.GetComponent<PlayerAction>().lastFacing;
        StartCoroutine(PointHammerForwards(direction)); // Faces hammer forward while player is charging forwards

        // Charge range indicator - change its size and rotation
        rangeIndicator.GetComponent<SpriteRenderer>().color = new Color32(0, 0, 0, 0);
        rangeIndicator.transform.position = transform.position; // Move indicator
        rangeIndicator.transform.localScale = new Vector3(1f, 1f, 1f); // Sets the length

        // Cameras
        Camera.main.GetComponent<CameraScript>().ResetZoom();

        if (charge < minCharge) // Cannot be less than min value
        {
            charge = minCharge;
        }
        StartCoroutine(Charge(direction, charge));
        ResetCharge(); // Starts ability cd and resets charge value
    }

    // CHARGE function
    private IEnumerator Charge(Vector2 dir, float chargedAmount)
    {
        float startTime = Time.time;

        Vector3 targetPos = (Vector2)player.transform.position + dir * chargedAmount;
        float distance = Vector2.Distance((Vector2)player.transform.position, targetPos);
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        RaycastHit2D[] enemies;
        List<GameObject> alreadyHit = new(); // new() is apparantly a thing - VS suggested it

        while (distance > 0.8f && !Physics2D.Raycast(player.transform.position, dir, 0.5f, obstacleLayer))
        {
            dir = (targetPos - player.transform.position).normalized;

            float speed = 1f; // Default of 1 such that it has no effect if we are beyond accelerating
            float elapsed = 0f;

            if (elapsed < 0.2f)
            {
                elapsed += Time.time - startTime;
                speed = accelerationCurve.Evaluate(elapsed);
            }

            rb.velocity = dir * chargeSpeed * speed; // Move towards targetpos

            // Hitting enemies and the consequences
            enemies = CheckForEnemies(baseHitboxSize + (chargedAmount * hitboxWidthMultiplier) / 1.8f, dir);
            foreach (RaycastHit2D enemy in enemies)
            {
                if (!alreadyHit.Contains(enemy.transform.gameObject)) // Only hit the enemy if they have not been damaged yet
                {
                    enemy.transform.gameObject.GetComponent<Health>().TakeDamage(aoeDMG);
                    alreadyHit.Add(enemy.transform.gameObject);
                    enemy.transform.gameObject.GetComponent<Crowd_Control>().Stun(stunDuration);
                }
            }

            yield return new WaitForSeconds(chargeUpdateInterval); // Time between steps

            distance = Vector2.Distance(player.transform.position, targetPos);
        }

        player.GetComponent<PlayerAction>().StartMove(); // Allow player to move again
        player.GetComponent<PlayerAction>().StopSlow();
        player.GetComponent<PlayerAction>().CanDash();
        player.GetComponent<PlayerHealth>().RemoveInvulnerability();

        EnableHammer(); // Hammer can hit enemies again
    }

    // Minimum time standing and charging
    private IEnumerator InitialCharging()
    {
        while (charge < minCharge)
        {
            isCharghingCharge = true;
            yield return null;
        }

        isCharghingCharge = false;
    }

    private IEnumerator PointHammerForwards(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;                // Angle for pointing to player
        mjoelnirSprite.transform.rotation = Quaternion.AngleAxis(angle + -135f, Vector3.forward);   // Point hammer away from player

        isCharging = true;

        while (isCharging)
        {
            // Points hammer and player in direction
            transform.position = player.transform.position + (Vector3)dir * 1.5f; // Position hammer in front of player

            yield return null;
        }
    }

    // AoE ability
    private void AOEAbility()
    {
        if (!hasAreaOfEffectUpgrade) { return; }                  // Checks if we have the upgrade

        if (Input.GetButton("Fire2") && abilityRDY)         // 'K' button held charges the ability, note you also need to have the cd ready
        {
            DisableHammer();

            // Toggle aoe indicator
            GameObject indicator = Instantiate(aoeIndicator, player.transform);
            Vector2 direction = player.GetComponent<PlayerAction>().lastFacing;

            StartCoroutine(AbilityCD(areaOfEffectCD));
            StartCoroutine(AreaOfEffect(direction, indicator));
        }
    }

    // Aoe ability
    private IEnumerator AreaOfEffect(Vector3 dir, GameObject indicator)
    {
        transform.position = player.transform.position; // Places hammer on player

        yield return new WaitForSeconds(castTime / 2);

        // Deal damage
        RaycastHit2D[] enemies = CheckForEnemies(aoeRadius, dir);
        foreach (RaycastHit2D enemy in enemies)
        {
            enemy.transform.gameObject.GetComponent<Health>().TakeDamage(aoeDMG);
        }
        
        yield return new WaitForSeconds(castTime);

        Destroy(indicator); // Removes indicator from view
        EnableHammer();
    }

    private RaycastHit2D[] CheckForEnemies(float radius, Vector2 direction)
    {
        return Physics2D.CircleCastAll(player.transform.position, radius, direction, radius, enemyLayers);
    }

    // Resets charge value so we can't do a max charge right away next time - also calls the cooldown function
    private void ResetCharge()
    {
        charge = 0f;

        if (abilityCDFunction != null) { StopCoroutine(abilityCDFunction); } // If we already have a cooldown running, stop it
        abilityCDFunction = StartCoroutine(AbilityCD(chargeCD));
        weaponCDVisual.StartCoroutine("MjoelnirCD");
    }

    // Puts ability on cd and is reset after a duration
    private IEnumerator AbilityCD(float cooldown)
    {
        abilityRDY = false;

        yield return new WaitForSeconds(cooldown);

        abilityRDY = true;  
    }

    // Kills enemies and deletes projectiles when they enter mj�lnir's collider
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (cannotHitList.ContainsKey(col.gameObject) && cannotHitList[col.gameObject] < Time.time)
        {
            cannotHitList.Remove(col.gameObject);
        }

        if (col.CompareTag("Enemy") && !cannotHitList.ContainsKey(col.gameObject))
        {
            //TODO rettes til et egentlig Knockback script eller metode paa enemy. Kald derfra metoden her. 
            col.transform.position += (col.transform.position - player.transform.position).normalized * 0.3f; // Slight knockback
            col.gameObject.GetComponent<Health>().TakeDamage(spinDMG);
            if (!onFreezeCD) StartCoroutine(FreezeSpin());
            AddToSpinCDList(col.gameObject);
        }
        else if (col.CompareTag("Projectile"))
        {
            Destroy(col.gameObject);
        }
    }

    public void UpgradeOption1()
    {
        if(!hasAreaOfEffectUpgrade)
            hasChargeUpgrade = true;
    }

    public void UpgradeOption2()
    {
        if (!hasChargeUpgrade)
            hasAreaOfEffectUpgrade = true;
    }

    public void Downgrade()
    {
        hasAreaOfEffectUpgrade = false;
        hasChargeUpgrade = false;
    }
}
