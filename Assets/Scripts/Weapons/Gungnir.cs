using System.Collections;
using UnityEngine;

public class Gungnir : Ability
{
    public float damage = 10;
    public float speed;
    public float stunDuration = 2f;

    private Rigidbody2D rb;
    private bool isDragging;
    private Vector2 dragDirection;
    private Vector2 direction;
    private BoxCollider2D gungnirCollider;

    private Collider2D pickUpCollider;

    private bool canMove = true;
    private bool isPinned = false;

    private float pierceAmount = 0f;
    public float maxPierceAmount;

    private CameraShake cameraShake;
    private bool isStuck;
    public bool canPickUp;

    public float bounceForce = 0.5f;

    public float bounceDuration = 2;

    private ThrowGungnir throwGungnir;

    private GungnirPickUp gungnirPickUp;

    [SerializeField] private LayerMask enemyLayer;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gungnirCollider = GetComponent<BoxCollider2D>();
        pickUpCollider = GameObject.Find("PickUp").GetComponent<Collider2D>();
        gungnirPickUp = GetComponent<GungnirPickUp>();
        gungnirPickUp.enabled = false;
        cameraShake = GameObject.Find("Camera").GetComponent<CameraShake>();
        throwGungnir = GameObject.Find("GungnirThrow").GetComponent<ThrowGungnir>();

        enemyLayer = LayerMask.GetMask("Enemy");

    }

    private void FixedUpdate()
    {
        if (canMove)
            transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;                // Angle for pointing to player
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);   // Point hammer away from player
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!isStuck && !throwGungnir.hasUpgrade2 && enemyLayer == (enemyLayer | (1 << col.gameObject.layer)))
        {
            pierceAmount++;
            //speed = speed - 5 * pierceAmount;
            if (speed <= 0)
                speed = 0;
            Health enemy = col.GetComponent<Health>();
            Transform enemyTrans = col.GetComponent<Transform>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            if (pierceAmount >= maxPierceAmount)
            {
                Invoke("StopMove", 1f);
                StartCoroutine(DieAfterTime(20f));
            }
        }
        if (col.CompareTag("Obstacles") && !isStuck)
        {
            StopMove();
            StartCoroutine(DieAfterTime(20f));
            //if (!throwGungnir.hasUpgrade2)
            //    StartCoroutine(Bounce());
        }
        if (col.CompareTag("Obstacles") || col.CompareTag("Enemy"))
        {
            cameraShake.SmallShake();
        }
        if (throwGungnir.hasUpgrade2)
        {
            if (col.CompareTag("Enemy"))
            {
                Health enemy = col.GetComponent<Health>();

                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
                isPinned = true;
                col.transform.SetParent(transform);
            }
        }
        if (isStuck && isPinned)
        {
            col.GetComponent<Crowd_Control>().Stun(stunDuration);
        }
    }

    //private IEnumerator Bounce()
    //{
    //    float CooldownRemaining; // The remaining cooldown time

    //    CooldownRemaining = bounceDuration; // Reset the remaining cooldown time
    //    while (CooldownRemaining > 0f) // Count down the cooldown time
    //    {
    //        transform.position += (Vector3)(-direction * bounceForce);
    //        CooldownRemaining -= Time.deltaTime;
    //        yield return null; // Wait for the end of the frame
    //    }
    //}

    public void StopMove()
    {
        canMove = false;
        gungnirCollider.enabled = false;
        isStuck = true;
        gungnirPickUp.enabled = true;
        StartCoroutine("PickUpBuffer");
    }

    public void StartMove()
    {
        canMove = true;
    }

    private IEnumerator PickUpBuffer()
    {
        yield return new WaitForSeconds(2f);
        canPickUp = true;
    }

    private IEnumerator DieAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}