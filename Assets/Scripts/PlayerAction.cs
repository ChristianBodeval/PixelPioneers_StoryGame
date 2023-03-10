using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private LayerMask groundLayer;
    private Rigidbody2D rb;
    private Vector3 moveVector = new Vector3(1f, 0f, 0f);
    private bool canMove = true;

    [Header("Dash")]
    [SerializeField] private float dashDuration = 0.1f;
    [SerializeField] private float dashStepLength = 0.2f; // The distance covered pr frame
    [SerializeField] private float dashCD = 0.8f;
    private bool canDash = true;

    [Header("Base Attack")]
    [SerializeField] private float cooldown = 0.4f;
    [SerializeField] private float coneRange = 2f;
    [SerializeField] private float innerRange = 0.4f; // Killzone range around the player, serves to kill enemies who are very close to the cone but not quite in it
    [SerializeField] private float degreeOfArc = 50f;
    [SerializeField] private LayerMask layerMask; // Enemy layer
    [SerializeField] private Transform cone; // temp
    [SerializeField] private Transform color; // temp
    [SerializeField] private float inputBuffer = 0.1f;
    private float BufferCountdown;
    private bool canAttack = true;
    private Vector2 lastFacing = new Vector2(1f, 0f);
    private ParticleSystem arcParticles;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Physics2D.IgnoreLayerCollision(3,7);
    }

    private void Update()
    {
        // Update player input
        moveVector.x = Input.GetAxis("Horizontal");
        moveVector.y = Input.GetAxis("Vertical");
        moveVector = moveVector.normalized; // As to not have faster movement when going diagonal

        if (Input.GetButton("Fire1")) // j,k,l are Fire1, Fire2, Fire3
        {
            BufferCountdown = inputBuffer;
        }
        else
        {
            BufferCountdown -= Time.deltaTime;
        }

        BaseMelee(); //melee coneattack

        Facing();
    }

    private void FixedUpdate()
    {
        Move();

        Dash();
    }

    private void Move()
    {
        if (!canMove) { return; } // Guard clause

        // Horizontal movement
        if (moveVector.x != 0)
        {
            rb.velocity = new Vector3(moveVector.x * speed, rb.velocity.y, 0f);
        }
        else
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f); // Stops the player
        }

        // Vertical movement
        if (moveVector.y != 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, moveVector.y * speed, 0f);
        }
        else
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, 0f); // Stops the player
        }
    }

    public void StartMove()
    {
        canMove = true;
    }

    public void StopMove()
    {
        canMove = false;
    }

    public IEnumerator StopMove(float time)
    {
        canMove = false;

        yield return new WaitForSeconds(time);

        canMove = true;
    }

    private void Facing()
    {
        // Saves the vector of where the player was last moving
        if (moveVector.magnitude > 0.5f)
        {
            lastFacing = new Vector2(moveVector.x, moveVector.y).normalized;
        }

        // Faces a cone in the direction of attack
        if (Mathf.Abs(lastFacing.x) > 0.9f) // Right & left
        {
            cone.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if (Mathf.Abs(lastFacing.y) > 0.9f) // Up & down
        {
            cone.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        }
        else if (lastFacing.x > 0.5f && lastFacing.y > 0.5f || lastFacing.x < -0.5f && lastFacing.y < -0.5f) // Up right & down left
        {
            cone.transform.rotation = Quaternion.Euler(0f, 0f, 45f);
        }
        else if (lastFacing.x < -0.5f && lastFacing.y > 0.5f || lastFacing.x > -0.5f && lastFacing.y < 0.5f) // Up left & down right
        {
            cone.transform.rotation = Quaternion.Euler(0f, 0f, -45f);
        }

        // Repositions the cone to be infront of player
        cone.position = (Vector2)transform.position + lastFacing.normalized * coneRange;
    }

    private void Dash()
    {
        if (!canDash || !Input.GetButton("Dash")) { return; } // Guard clause - Continues if we can dash and have pressed button

        StartCoroutine(DashCD());
        StartCoroutine(DashMove());
        StartCoroutine(StopMove(dashDuration));
    }

    private IEnumerator DashMove()
    {
        float currentDashTime = dashDuration;

        while (currentDashTime > 0)
        {
            currentDashTime -= Time.deltaTime; // Countdown
            transform.position += dashStepLength * (Vector3)lastFacing;
            CircleCollider2D col = GetComponent<CircleCollider2D>(); // Players collider, used to check if the player is in a wall

            // Gets the player through walls if near them
            if (Physics2D.CircleCast(transform.position, col.radius-0.2f, lastFacing, col.radius-0.2f, obstacleLayer))
            {
                transform.position += dashStepLength * (Vector3)lastFacing; // Try and move through it
            }

            yield return null;
        }
    }

    private IEnumerator DashCD()
    {
        canDash = false;

        yield return new WaitForSeconds(dashDuration + dashCD);

        canDash = true;
    }

    private void BaseMelee()
    {
        if (BufferCountdown > 0f && canAttack)
        {
            StartCoroutine(MeleeCD()); // Cooldown
            StartCoroutine(MeleeAttackEffects()); // Changes the color or the cone indicating when we attack

            RaycastHit2D[] enemies = Physics2D.CircleCastAll(transform.position, coneRange, lastFacing, coneRange, layerMask); // List of enemies around player

            foreach (RaycastHit2D e in enemies)
            {
                bool isHit = IsPointInsideCone(e.transform.position, transform.position, lastFacing, degreeOfArc, coneRange); // Checks if enemies are within a cone of a circle

                Vector2 enemyPos = e.transform.position;
                Vector2 playerPos = transform.position;
                bool los = !Physics2D.Raycast(playerPos, enemyPos - playerPos, coneRange, obstacleLayer); // Line of sight

                if (isHit && los || Vector3.Distance(playerPos, enemyPos) < innerRange && los) // Is hit and in line of sight of the player
                {
                    Pool.pool.ReturnToPool(e.transform.gameObject); // Deactivates enemy and returns them to the pool
                }
            }
        }
    }

    private IEnumerator MeleeCD()
    {
        canAttack = false;

        yield return new WaitForSeconds(cooldown);

        canAttack = true;
    }

    private IEnumerator MeleeAttackEffects()
    {
        SpriteRenderer c = color.GetComponent<SpriteRenderer>();
        c.color = new Color32(255, 0, 0, 100);

        yield return new WaitForSeconds(0.1f);

        c.color = new Color32(255, 255, 255, 100);
    }

    public static bool IsPointInsideCone(Vector3 point, Vector3 coneOrigin, Vector3 coneDirection, float maxAngle, float maxDistance)
    {
        var distanceToConeOrigin = (point - coneOrigin).magnitude;
        if (distanceToConeOrigin < maxDistance)
        {
            var pointDirection = point - coneOrigin;
            var angle = Vector3.Angle(coneDirection, pointDirection);
            if (angle < maxAngle)
                return true;
        }
        return false;
    }
}
