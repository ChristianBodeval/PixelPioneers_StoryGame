using System.Collections;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 10f;

    private float slowAmount;
    private Rigidbody2D rb;
    public Vector3 moveVector;
    private bool canMove = true;

    [Header("SFX")]
    [Range(0f, 1f)][SerializeField] private float volume;
    [SerializeField] private AudioClip footstepSFX;
    [SerializeField] private float footstepDelay = 0.12f;
    private float lastSFX;

    public bool canMoveAccessor
    {
        get => canMove;
        set => canMove = value;
    }

    [Header("Input")]
    [SerializeField] private float inputBuffer = 0.1f;

    [HideInInspector] public Vector2 lastFacing;

    private float BufferCountdown;
    private bool canAttack = true;
    private ParticleSystem arcParticles;

    //TODO Bruges ikke...
    private Health healthScript;

    public WeaponCDs weaponCDVisual;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Physics2D.IgnoreLayerCollision(3, 7);
        healthScript = GetComponent<Health>();
    }

    private void Update()
    {
        // StateUpdate player input
        moveVector.x = Input.GetAxis("Horizontal");
        moveVector.y = Input.GetAxis("Vertical");
        moveVector = moveVector.normalized; // As to not have faster movement when going diagonal

        //TODO Dette bliver ikke brugt her, saa vi kan slette det, eller rykke det til et andet script, hvor det bruges.
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

            // Footsteps
            if (footstepDelay + lastSFX < Time.time)
            {
                lastSFX = Time.time + footstepDelay;
                SFXManager.singleton.PlaySound(footstepSFX, transform.position, volume, false, transform);
            }
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

    // Coroutine that controls the dash cooldown time

    private void Facing()
    {
        // Saves the vector of where the player was last moving
        if (moveVector.magnitude > 0.5f)
        {
            lastFacing = new Vector2(moveVector.x, moveVector.y).normalized;
        }
    }
}