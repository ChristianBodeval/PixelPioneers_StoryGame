using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hermes_Gungnir_Side : MonoBehaviour
{
    [Header("SFX")]
    [Range(0, 1)] public float sfxVolume = 1f;
    [SerializeField] private AudioClip wallImpactSFX;
    [SerializeField] private AudioClip penetrationSFX;

    public float damage = 10;
    public float speed;
    public float waitDuration = 2f;

    private Rigidbody2D rb;
    private Vector2 direction;
    private BoxCollider2D gungnirCollider;
    private float thrownTime = 0f;

    private bool isStopped = false;

    private void Start()
    {
        thrownTime = Time.time;
        rb = GetComponent<Rigidbody2D>();
        gungnirCollider = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        if (!isStopped)
            rb.velocity = direction * speed;
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;                // Angle for pointing to player
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);   // Point hammer away from player
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Obstacles"))
        {
            SFXManager.singleton.PlaySound(wallImpactSFX, transform.position, sfxVolume);
            StartCoroutine(StopMove(0f));
        }
        else if (col.CompareTag("Player"))
        {
            SFXManager.singleton.PlaySound(penetrationSFX, transform.position, sfxVolume);
            col.transform.GetComponent<PlayerHealth>().TakeDamage(damage);
            StartCoroutine(StopMove());
        }

        if (col.CompareTag("Boss") && thrownTime + 1f < Time.time)
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator StopMove(float multiplier = 1f)
    {
        isStopped = true;
        gungnirCollider.enabled = false;
        yield return new WaitForSeconds(0.2f * multiplier);

        rb.velocity = Vector2.zero;
        Destroy(gameObject);
    }
}