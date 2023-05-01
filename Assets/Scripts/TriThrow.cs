using UnityEngine;

public class TriThrow : MonoBehaviour
{
    public float damage = 10;
    public float speed = 22;
    public float stunDuration = 2f;
    public float CD;

    private Rigidbody2D rb;
    private Vector2 direction;

    private float pierceAmount = 0f;
    public float maxPierceAmount;

    private CameraShake cameraShake;
    private bool isStuck;
    public bool canPickUp;

    public float bounceForce = 0.5f;

    public float bounceDuration = 2;

    private PlayerAction playerAction;

    [Header("Upgrade")]
    public bool hasUpgrade1;

    public bool isTriSpear;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAction = GameObject.Find("Player").GetComponent<PlayerAction>();
        cameraShake = GameObject.Find("Camera").GetComponent<CameraShake>();
        //SetDirection1(playerAction.lastFacing);
        SetDirection1(playerAction.lastFacing);
    }

    private void FixedUpdate()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
        Debug.Log("Moving triSpear");
    }
    
    public void SetDirection1(Vector2 dir)
    {
        direction = dir;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;                // Angle for pointing to player
        transform.rotation = Quaternion.AngleAxis(45, Vector3.forward);
    }
    public void SetDirection2(Vector2 dir)
    {
        direction = dir;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;                // Angle for pointing to player
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
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
            }
            if (pierceAmount >= maxPierceAmount)
            {
                Destroy(gameObject);
            }
        }
        if (col.CompareTag("Obstacles") || col.CompareTag("Enemy"))
        {
            cameraShake.SmallShake();
        }
        if (col.CompareTag("Obstacles"))
        {
            Destroy(gameObject);
        }
    }

    public void ResetCD()
    {
        CD = 0;
        Debug.Log("RESET CD on GUNGNIR");
    }
}