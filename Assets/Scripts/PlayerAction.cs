using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAction : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 10f;

    [SerializeField] private LayerMask obstacleLayer;
    private Rigidbody2D rb;
    private Vector3 moveVector = new Vector3(1f, 0f, 0f);
    private bool canMove = true;

    [Header("Base Attack")]
    [SerializeField] private float cooldown = 0.4f;

    [SerializeField] private float coneRange = 2f;
    [SerializeField] private float innerRange = 0.4f; //killzone range immediatly around the player
    [SerializeField] private float degreeOfArc = 50f;
    [SerializeField] private LayerMask layerMask; //enemy layer
    [SerializeField] private Transform cone; //temp
    [SerializeField] private Transform color; //temp
    [SerializeField] private float inputBuffer = 0.1f;
    private float BufferCountdown;
    private bool canAttack = true;
    private Vector2 lastFacing = new Vector2();
    private ParticleSystem arcParticles;

    private Health healthScript;

    [Header("Dash")]
    public Slider DashCDVisual;
    public float dashDistance = 5f; // Distance of the dash
    public float dashDuration = 0.5f; // Duration of the dash
    private bool isDashing = false; // Flag to check if the player is currently dashing
    private float dashTime = 0f; // Time elapsed during the dash
    private Vector2 dashDirection; // Direction of the dash
    private bool canDash = true; //C Flag to check if the player can dash
    private float dashCooldownTime = 3f;
    private float dashCooldownRemaining = 0f; // Initialize to 0 to allow dashing immediately

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Physics2D.IgnoreLayerCollision(3, 7);
        dashDirection = GetDashDirection();

        healthScript = GetComponent<Health>();

    }

    private void Update()
    {
        if (Input.GetButton("Fire1")) //j,k,l are Fire1, Fire2, Fire3
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

        DashCDVisual.value = dashCooldownRemaining / dashCooldownTime;
        
        if (!canDash)
        Debug.Log("Dash Cooldown:" + dashCooldownRemaining);
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
        if (!canMove) { return; } // Guard clause

        moveVector.x = Input.GetAxis("Horizontal");
        moveVector.y = Input.GetAxis("Vertical");
        moveVector = moveVector.normalized; //as to not have faster movement when going diagonal

        //horizontal movement
        if (moveVector.x != 0)
        {
            rb.velocity = new Vector3(moveVector.x * speed, rb.velocity.y, 0f);
        }
        else
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f); //stops the player
        }

        //vertical movement
        if (moveVector.y != 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, moveVector.y * speed, 0f);
        }
        else
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, 0f); //stops the player
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
        //save where the player has been looking
        if (moveVector.magnitude > 0.5f)
        {
            lastFacing = new Vector2(moveVector.x, moveVector.y);
        }

        //face a cone in the direction of attack
        cone.eulerAngles = new Vector3(0f, 0f, Quaternion.LookRotation(lastFacing, Vector3.up).eulerAngles.x);
        cone.position = (Vector2)transform.position + lastFacing.normalized * coneRange;
    }

    private void BaseMelee()
    {
        if (BufferCountdown > 0f && canAttack) //**needs an los check
        {
            StartCoroutine(MeleeCD()); //cooldown
            StartCoroutine(MeleeAttackEffects()); //change color to show attack being used

            RaycastHit2D[] enemies = Physics2D.CircleCastAll(transform.position, coneRange, lastFacing, coneRange, layerMask); //enemies in circle around player

            foreach (RaycastHit2D e in enemies)
            {
                bool isHit = IsPointInsideCone(e.transform.position, transform.position, lastFacing, degreeOfArc, coneRange); //enemies within a cone in the circle

                if (isHit || Vector3.Distance(transform.position, e.transform.position) < innerRange)
                {
                    Pool.pool.ReturnToPool(e.transform.gameObject); //deactivate and return to pool
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
        if (!isDashing && Input.GetButton("Fire2") && canDash) // Dash is on 'K'
        {

            healthScript.enabled = false;

            // Set the player to dashing state
            isDashing = true;
            dashTime = 0f;

            // Set the dash direction to the last facing direction of the player
            dashDirection = lastFacing;

            // Start the dash cooldown coroutine
            StartCoroutine("DashCD");


        }
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
        healthScript.enabled = true;

    }

    private Vector2 GetDashDirection()
    {
        // Get the direction the player is facing
        float angle = transform.eulerAngles.y;
        Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

        return direction.normalized;
    }
}