using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAction : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 10f;
    public LayerMask obstacleLayer;
    [SerializeField] private LayerMask groundLayer;
    private Rigidbody2D rb;
    private Vector3 moveVector = new Vector3(1f, 0f, 0f);
    private bool canMove = true;

    [Header("Base Attack")]
    [SerializeField] private float cooldown = 0.4f;

    [SerializeField] private float coneRange = 2f;
    [SerializeField] private float innerRange = 0.4f; // Killzone range around the player, serves to kill enemies who are very close to the cone but not quite in it
    [SerializeField] private float degreeOfArc = 50f;
    [SerializeField] private LayerMask layerMask; // Enemy layer
    [SerializeField] private Transform cone; // temp
    [SerializeField] private Transform color; // temp
    [SerializeField] private float inputBuffer = 0.1f;
    [HideInInspector] public Vector2 lastFacing = new Vector2(1f, 0f);
    private float BufferCountdown;
    private bool canAttack = true;
    private ParticleSystem arcParticles;

    private Health healthScript;

    [Header("Dash")]
    public Image DashCDVisual;
    public float dashDistance = 5f; // Distance of the dash
    public float dashDuration = 0.5f; // Duration of the dash
    public float dashCooldownTime = 1.8f;
    private bool isDashing = false; // Flag to check if the player is currently dashing
    private float dashTime = 0f; // Time elapsed during the dash
    private Vector2 dashDirection; // Direction of the dash
    private bool canDash = true; //C Flag to check if the player can dash
    private float dashCooldownRemaining = 0f; // Initialize to 0 to allow dashing immediately

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Physics2D.IgnoreLayerCollision(3, 7);
        dashDirection = GetDashDirection();

        healthScript = GetComponent<Health>();
        DashCDVisual.fillAmount = 1;

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

        Dash();

        DashCDVisual.fillAmount = dashCooldownRemaining / dashCooldownTime;
    }

    private void FixedUpdate()
    {
        Move();

        // Check if the player is dashing
        if (isDashing)
        {
            // If the dash duration has not elapsed, move the player in the dash direction
            if (dashTime < dashDuration)
            {
                rb.MovePosition(rb.position + dashDirection * dashDistance / dashDuration * Time.fixedDeltaTime);
                dashTime += Time.fixedDeltaTime;
            }
            // Otherwise, end the dash
            else
            {
                EndDash();
            }
        }
    }

    private void Move()
    {
        if (moveVector.magnitude > 0f && canMove) // Horizontal movement
        {
            rb.velocity = moveVector * speed;
        }
        else if (moveVector.magnitude < 0.1f && canMove)
        {
            rb.velocity = Vector2.zero; // Stops the player
        }
    }

    public void StartMove()
    {
        canMove = true;
    }

    public void StopMove()
    {
        canMove = false;
        rb.velocity = Vector2.zero;
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

    private void Dash()
    {
        // Check if the player is not currently dashing and if they can dash
        if (!isDashing && Input.GetButton("Dash") && canDash) // Dash is on 'not k'
        {
            GetComponent<PlayerHealth>().AddInvulnerability();

            // Set the player to dashing state
            isDashing = true;
            dashTime = 0f;

            // Set the dash direction to the last facing direction of the player
            dashDirection = lastFacing;

            // Start the dash cooldown coroutine
            StartCoroutine(DashCD());


        }
    }

    public void CanDash()
    {
        canDash = true;
    }

    public void CannotDash()
    {
        canDash = false;
    }

    // Coroutine that controls the dash cooldown time
    private IEnumerator DashCD()
    {
        canDash = false; // Set the player to be unable to dash
        dashCooldownRemaining = dashCooldownTime; // Reset the remaining cooldown time
        while (dashCooldownRemaining > 0f) // Count down the cooldown time
        {
            dashCooldownRemaining -= Time.deltaTime;
            yield return null; // Wait for the end of the frame
        }
        canDash = true; // Set the player to be able to dash again
    }

    private void EndDash()
    {
        isDashing = false;
        GetComponent<PlayerHealth>().RemoveInvulnerability();
    }

    private Vector2 GetDashDirection()
    {
        // Get the direction the player is facing
        float angle = transform.eulerAngles.y;
        Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

        return direction.normalized;
    }
}