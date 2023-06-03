using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerAction : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 10f;
    private float slowAmount;
    private Rigidbody2D rb;
    public Vector3 moveVector;
    private bool canMove = true;

    [Header("Input")]
    [SerializeField] private Animator animator;
    [SerializeField] private float inputBuffer = 0.1f;
    [HideInInspector] public Vector2 lastFacing;
    [HideInInspector] public Vector2 lastFacing2;
    private float BufferCountdown;
    private bool canAttack = true;
    private ParticleSystem arcParticles;

    private Health healthScript;

    [Header("Dash")]
    public float dashDistance = 5f; // Distance of the dash
    public float dashDuration = 0.5f; // Duration of the dash
    [HideInInspector] public float dashCooldownTime = 1.8f;
    private bool isDashing = false; // Flag to check if the player is currently dashing
    private float dashTime = 0f; // Time elapsed during the dash
    private Vector2 dashDirection; // Direction of the dash
    private bool canDash = true; //C Flag to check if the player can dash
    [HideInInspector] public float dashCooldownRemaining = 0f; // Initialize to 0 to allow dashing immediately
    private bool isRunning = false;
    

    private void Awake()
    {
        lastFacing = Vector2.down;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Physics2D.IgnoreLayerCollision(3, 7);
        dashDirection = GetDashDirection();
        healthScript = GetComponent<Health>();
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

        Dash();

        Facing();
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
                Camera.main.GetComponent<CameraScript>().StartLagBehindPlayer();
                rb.MovePosition(rb.position + dashDirection * dashDistance / dashDuration * Time.fixedDeltaTime);
                dashTime += Time.fixedDeltaTime;
            }
            // Otherwise, end the dash
            else
            {
                Camera.main.GetComponent<CameraScript>().StopLagBehindPlayer();
                EndDash();
            }
        }
    }

    private void Move()
    {
        if (moveVector.magnitude > 0f && canMove) // Horizontal movement
        {
            rb.velocity = moveVector * speed * (1 - slowAmount);

        }
        else if (moveVector.magnitude < 0.1f && canMove)
        {
            rb.velocity = Vector2.zero; // Stops the player
        }

        animator.SetFloat("XInput", lastFacing.x);
        animator.SetFloat("YInput", lastFacing.y);

        if (moveVector.magnitude > 0.1f)
        {

            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
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
        animator.SetBool("IsRunning", false);
    }

    public IEnumerator StopMove(float time)
    {
        canMove = false;

        yield return new WaitForSeconds(time);

        canMove = true;
    }

    public void StartSlow(float percentage)
    {
        slowAmount = percentage / 100;
    }

    public void StopSlow()
    {
        slowAmount = 0;
    }

    public IEnumerator Slow(float percentage, float duration)
    {
        duration += Time.time;
        slowAmount = percentage / 100;

        yield return new WaitForSeconds(duration);

        slowAmount = 0f;
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

            WeaponCDs.Instance.StartCoroutine("DashCD");
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

    private void Facing()
    {
        // Saves the vector of where the player was last moving
        if (moveVector.magnitude > 0.5f)
        {
            lastFacing = new Vector2(moveVector.x, moveVector.y).normalized;
        }
    }

    private void EndDash()
    {
        isDashing = false;
        GetComponent<PlayerHealth>().RemoveInvulnerability(); // I frames
    }

    private Vector2 GetDashDirection()
    {
        // Get the direction the player is facing
        float angle = transform.eulerAngles.y;
        Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

        return direction.normalized;
    }
}