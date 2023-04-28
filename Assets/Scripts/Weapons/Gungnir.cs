using System.Collections;
using UnityEngine;

public class Gungnir : MonoBehaviour, IUpgradeable
{
    public float damage = 10;
    public float speed;
    public float stunDuration = 2f;
    public float CD;

    private Rigidbody2D rb;
    private bool isDragging;
    private Vector2 dragDirection;
    private Vector2 direction;
    private Collider2D gungnirCollider;
    private Collider2D pickUpCollider;

    private bool canMove = true;

    private float pierceAmount = 0f;
    public float maxPierceAmount;

    private CameraShake cameraShake;
    private bool isStuck;
    public bool canPickUp;

    public float bounceForce = 0.5f;
    public float bounciness = 0.2f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gungnirCollider = GetComponent<Collider2D>();
        pickUpCollider = GameObject.Find("PickUp").GetComponent<Collider2D>();
        cameraShake = GameObject.Find("Camera").GetComponent<CameraShake>();
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
        if (col.CompareTag("Enemy") && !isStuck)
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
                //enemy.transform.gameObject.GetComponent<Crowd_Control>().Stun(stunDuration);
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
        }
        if (col.CompareTag("Obstacles") || col.CompareTag("Enemy"))
        {
            cameraShake.SmallShake();
        }

        if (col.CompareTag("Obstacles"))
        {
            Vector2 normal = (transform.position - col.transform.position).normalized;
            Vector2 direction = Vector2.Reflect(rb.velocity.normalized, normal);
            float speed = Mathf.Max(rb.velocity.magnitude * (1 - bounciness), 0);
            rb.velocity = direction * speed * bounceForce;
        }
    }

    public void StopMove()
    {
        canMove = false;
        gungnirCollider.enabled = false;
        isStuck = true;
        pickUpCollider.enabled = true;
        StartCoroutine("PickUpBuffer");
    }

    public void StartMove()
    {
        canMove = true;
    }

    public void ResetCD()
    {
        CD = 0;
        Debug.Log("RESET CD on GUNGNIR");
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

    public void UpgradeOption1()
    {
        Debug.Log("Gungnir has been upgraded with upgrade 1");
        throw new System.NotImplementedException();
    }

    public void UpgradeOption2()
    {
        Debug.Log("Gungnir has been upgraded with upgrade 2");

        throw new System.NotImplementedException();
    }

    public void Downgrade()
    {
        Debug.Log("Gungnir has been Downgraded");

        throw new System.NotImplementedException();
    }
}