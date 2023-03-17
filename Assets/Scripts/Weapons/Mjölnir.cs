using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mjölnir : MonoBehaviour
{
    [Header("Hammer Movement")]
    [SerializeField] private float spinRadius;
    [SerializeField] private float spinSpeed;
    [SerializeField] private float spriteRotationSpeed;
    [SerializeField] private GameObject mjölnirSprite;
    private GameObject player;

    [Header("Charge Upgrade")]
    public bool hasChargeUpgrade = true;
    [SerializeField] private float chargeSpeed;
    [SerializeField] private float chargeHitbox;
    [SerializeField] private float chargeCD;
    [SerializeField] private float buttonChargeUpRate = 10;
    [SerializeField] private float chargeUpdateInterval = 0.05f;
    [SerializeField] private float minCharge;
    [SerializeField] private float maxCharge;
    [SerializeField] private float stunDuration = 1f;
    [SerializeField] private GameObject rangeIndicator;
    private bool chargeBeingUsed = false;
    private float charge = 0f;

    [Header("Area Of Effect Upgrade")]
    public bool hasAreaOfEffectUpgrade = false;
    [SerializeField] private float castTime = 0.3f;
    [SerializeField] private float aoeRadius = 3f;
    [SerializeField] private float areaOfEffectCD;
    [SerializeField] private GameObject aoeIndicator;
    private bool areaOfEffectBeingUsed = false;

    private bool abilityRDY = true;
    private Coroutine abilityCDFunction;
    

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        Spin();

        ChargeAbility();

        AOEAbility();
    }

    private void FixedUpdate()
    {
        if (!chargeBeingUsed)
        {
            transform.RotateAround(player.transform.position, new Vector3(0f, 0f, 1f), spinSpeed);
        }
    }

    // Spins the hammer around the player
    private void Spin()
    {
        if (chargeBeingUsed) // Guard clause - if an ability using mjölnir is being used, stop spinning
        {
            Vector2 direction = player.GetComponent<PlayerAction>().lastFacing;
            transform.position = player.transform.position + (Vector3)direction * 1.5f;
            return;
        }

        transform.position = player.transform.position + (transform.right + transform.up) * spinRadius;
        mjölnirSprite.transform.Rotate(0f, 0f, spriteRotationSpeed);
    }

    private void EnableHammer()
    {
        chargeBeingUsed = false;    // Allow the hammer to spin again
        GetComponent<CircleCollider2D>().enabled = true;    // Player can move again
    }

    private void DisableHammer()
    {
        chargeBeingUsed = true;    // Stop hammer's spin
        GetComponent<CircleCollider2D>().enabled = false;   // Cannot hit enemies with hammer sprite
    }

    private void ChargeAbility()
    {
        if (!hasChargeUpgrade) { return; }                  // Checks if we have the upgrade

        if (Input.GetButton("Fire2") && abilityRDY)         // 'K' button held charges the ability, note you also need to have the cd ready
        {
            chargeBeingUsed = true;
            player.GetComponent<PlayerAction>().StopMove(); // Root the player while casting
            DisableHammer();

            // Position the hammer on player
            transform.position = player.transform.position;
            mjölnirSprite.transform.Rotate(0f, 0f, spriteRotationSpeed);

            charge += Time.deltaTime * buttonChargeUpRate;  // Charge value
            if (charge > maxCharge) // Cannot exceed max value
            {
                charge = maxCharge;
            }

            // Charge range indicator - change its size and rotation
            rangeIndicator.GetComponent<SpriteRenderer>().color = new Color32(180, 180, 0, 180); ;
            rangeIndicator.transform.localScale = new Vector3(charge, chargeHitbox * 2 - 0.2f, 1f); // Sets the length - chargeHitbox * 2 - 0.2f is the diameter of the indicator -0.2f is such that the player feels cheated of a hit less often
            Vector2 direction = player.GetComponent<PlayerAction>().lastFacing;
            rangeIndicator.transform.position = player.transform.position + (Vector3)direction * (charge / 2); // Move indicator
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rangeIndicator.gameObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        if (Input.GetButtonUp("Fire2") && abilityRDY)
        {
            Vector2 direction = player.GetComponent<PlayerAction>().lastFacing;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;                // Angle for pointing to player
            mjölnirSprite.transform.rotation = Quaternion.AngleAxis(angle + -135f, Vector3.forward);   // Point hammer away from player

            // Charge range indicator - change its size and rotation
            rangeIndicator.GetComponent<SpriteRenderer>().color = new Color32(0, 0, 0, 0);
            rangeIndicator.transform.position = transform.position; // Move indicator
            rangeIndicator.transform.localScale = new Vector3(1f, 1f, 1f); // Sets the length

            if (charge < minCharge) // Cannot be less than min value
            {
                charge = minCharge;
            }
            StartCoroutine(Charge(direction, charge));
            ResetCharge(); // Starts ability cd and resets charge value
        }
    }

    // CHARGE function
    private IEnumerator Charge(Vector2 direction, float chargedAmount)
    {
        Vector3 targetPos = (Vector2)player.transform.position + direction * chargedAmount;
        float distance = Vector2.Distance((Vector2)player.transform.position, targetPos);
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        Vector3 dir = Vector3.zero;
        LayerMask obstacleLayer = player.GetComponent<PlayerAction>().obstacleLayer;
        RaycastHit2D[] enemies;

        while (distance > 0.2f && !Physics2D.Raycast(player.transform.position, direction, 0.5f, obstacleLayer))
        {
            dir = (targetPos - player.transform.position).normalized;
            rb.velocity = dir * chargeSpeed; // Lerp from our pos to targetpos

            // Hitting enemies and the consequences
            enemies = CheckForEnemies(chargeHitbox, direction);
            foreach (RaycastHit2D enemy in enemies)
            {
                enemy.transform.position = Vector2.Lerp(enemy.transform.position, transform.position, 0.6f);        // Move enemy closer to hammer
                enemy.transform.SetParent(transform);                                                               // Enemy moves with player
                enemy.transform.gameObject.GetComponent<Crowd_Control>().Stun();                                    // Enemy is stunned and cannot attack or move
            }

            yield return new WaitForSeconds(chargeUpdateInterval); // Time between steps

            distance = Vector2.Distance(player.transform.position, targetPos);
        }

        // Remove player as parent
        enemies = CheckForEnemies(chargeHitbox + 5f, direction); // + 5f to be sure we don't miss any enemies
        foreach (RaycastHit2D enemy in enemies)
        {
            enemy.transform.SetParent(null);
            enemy.transform.gameObject.GetComponent<Crowd_Control>().Stun(stunDuration);
            if (!Physics2D.Raycast(enemy.transform.position, dir, 0.5f, obstacleLayer))
            {
                enemy.transform.position += dir * 0.5f; // Move enemies futher away
            }
        }

        player.GetComponent<PlayerAction>().StartMove(); // Allow player to move again

        EnableHammer(); // Hammer can hit enemies again
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
            indicator.transform.localScale = new Vector3(aoeRadius * 2, aoeRadius * 2, 1f); // Sets the length

            StartCoroutine(AbilityCD(areaOfEffectCD));
            StartCoroutine(AreaOfEffect(direction, indicator));
        }
    }

    private IEnumerator AreaOfEffect(Vector3 dir, GameObject indicator)
    {
        yield return new WaitForSeconds(castTime);

        RaycastHit2D[] enemies = CheckForEnemies(aoeRadius, dir);

        foreach (RaycastHit2D enemy in enemies)
        {
            Pool.pool.ReturnToPool(enemy.transform.gameObject);
        }

        Destroy(indicator); // Removes indicator from view
        EnableHammer();
    }

    private RaycastHit2D[] CheckForEnemies(float radius, Vector2 direction)
    {
        return Physics2D.CircleCastAll(player.transform.position, radius, direction, radius, LayerMask.GetMask("Enemy"));
    }

    // Resets charge value so we can't do a max charge right away next time - also calls the cooldown function
    private void ResetCharge()
    {
        charge = 0f;

        if (abilityCDFunction != null) { StopCoroutine(abilityCDFunction); } // If we already have a cooldown running, stop it
        abilityCDFunction = StartCoroutine(AbilityCD(chargeCD));
    }

    // Puts ability on cd and is reset after a duration
    private IEnumerator AbilityCD(float cooldown)
    {
        abilityRDY = false;

        yield return new WaitForSeconds(cooldown);

        abilityRDY = true;
    }

    // Kills enemies and deletes projectiles when they enter mjölnir's collider
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            Pool.pool.ReturnToPool(col.gameObject);
        }
        else if (col.CompareTag("Projectile"))
        {
            Destroy(col.gameObject);
        }
    }
}
