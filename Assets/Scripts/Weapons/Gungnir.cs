using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gungnir : Ability
{
    [Header("SFX")]
    [Range(0, 1)] public float sfxVolume = 1f;
    [SerializeField] private AudioClip wallImpactSFX;
    [SerializeField] private AudioClip penetrationSFX;

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
    private List<GameObject> piercedEnemies = new List<GameObject>();

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
            SFXManager.singleton.PlaySound(wallImpactSFX, transform.position, sfxVolume);

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
            if (col.CompareTag("Enemy") || col.CompareTag("Boss"))
            {
                SFXManager.singleton.PlaySound(penetrationSFX, transform.position, sfxVolume);

                Health enemy = col.GetComponent<Health>();

                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }

                isPinned = true;

                if (col.CompareTag("Enemy")) 
                {
                    col.transform.SetParent(transform);
                    piercedEnemies.Add(col.gameObject);
                }

                if (col.GetComponent<Charger_Attack>() != null) col.GetComponent<Charger_Attack>().StopCharge();
            }
        }
        if (isStuck && isPinned)
        {
            if (col.GetComponent<Crowd_Control>() != null) col.GetComponent<Crowd_Control>().Stun(stunDuration);
        }
    }

    public void StopMove()
    {
        canMove = false;
        gungnirCollider.enabled = false;
        isStuck = true;
        gungnirPickUp.enabled = true;
        StartCoroutine("PickUpBuffer");

        foreach (GameObject e in piercedEnemies)
        {
            e.transform.SetParent(null);
        }
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
        foreach (GameObject e in piercedEnemies)
        {
            e.transform.SetParent(null);
        }

        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}