using UnityEngine;

public class TriThrow : MonoBehaviour
{
    public float damage = 10;
    public float speed = 22;
    public float stunDuration = 2f;
    public float CD;

    private Rigidbody2D rb;
    public Vector2 direction;

    public float angleOfSpear;
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
    }

    public void SetDirection(Vector2 dir, float throwAngle)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + throwAngle;                // Angle for pointing to player
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        direction = dir + GetDirectionFromAngle(throwAngle);
    }
    //public void SetDirection(Vector2 dir, float angle)
    //{
    //    direction = dir;
    //    float angleInRadians = (playerAction.lastFacing + GetDirectionFromAngle(angle)).normalized.y >= 0 ? angle : -angle;
    //    transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg, Vector3.forward) * Quaternion.Euler(0f, 0f, angleInRadians);
    //    direction = Quaternion.Euler(0f, 0f, angleInRadians) * dir;
    //}


    private void FixedUpdate()
    {
        transform.position += (Vector3)(direction.normalized * speed * Time.deltaTime);
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

    private Vector2 GetDirectionFromAngle(float angle)
    {
        float angleInRadians = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
    }


    public void ResetCD()
    {
        CD = 0;
        Debug.Log("RESET CD on GUNGNIR");
    }
}