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

    [Header("Input")]
    [SerializeField] private float inputBuffer = 0.1f;

    [HideInInspector] public Vector2 lastFacing;

    private float BufferCountdown;
    private bool canAttack = true;
    private ParticleSystem arcParticles;

    //TODO Bruges ikke...
    private Health healthScript;

    public WeaponCDs weaponCDVisual;

    [Header("Gungnir")]
    public GameObject gungnir;

    public IEnumerator gungnirCDCoroutine;

    private Gungnir gungnirScript;
    [HideInInspector] public bool canThrowGungnir = true;
    private float gungnirCD;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Physics2D.IgnoreLayerCollision(3, 7);
        healthScript = GetComponent<Health>();

        gungnirCDCoroutine = GungnirCD();
        gungnirScript = gungnir.GetComponent<Gungnir>();
    }

    private void Update()
    {
        // Update player input
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

        ThrowGungnir();
        gungnirCD = gungnirScript.CD;
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

    private void ThrowGungnir()
    {
        if (Input.GetButtonDown("Fire3") && canThrowGungnir)
        {
            Gungnir spear = Instantiate(gungnirScript, transform.position, Quaternion.identity);
            spear.SetDirection(lastFacing);
            StartCoroutine("GungnirCD");
            Debug.Log("Threw gungnir");
            weaponCDVisual.StartCoroutine("GungnirCD");
        }
    }

    public IEnumerator GungnirCD()
    {
        canThrowGungnir = false;
        yield return new WaitForSeconds(gungnirCD);
        canThrowGungnir = true;
    }
}