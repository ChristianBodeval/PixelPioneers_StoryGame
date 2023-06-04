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
    private bool isRunning = false;

    [Header("Input")]
    [SerializeField] private Animator animator;
    [SerializeField] private float inputBuffer = 0.1f;
    [HideInInspector] public Vector2 lastFacing;
    [HideInInspector] public Vector2 lastFacing2;
    private float BufferCountdown;
    private bool canAttack = true;
    private ParticleSystem arcParticles;

    private Health healthScript;

    private void Awake()
    {
        lastFacing = Vector2.down;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Physics2D.IgnoreLayerCollision(3, 7);
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

        Facing();
    }

    private void FixedUpdate()
    {
        Move();
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

    private void Facing()
    {
        // Saves the vector of where the player was last moving
        if (moveVector.magnitude > 0.5f)
        {
            lastFacing = new Vector2(moveVector.x, moveVector.y).normalized;
        }
    }
}