using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hermes_Gungnir : MonoBehaviour
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

    private bool isStopped = false;

    private void Start()
    {
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
        if (col.CompareTag("Obstacles") && !isStopped)
        {
            SFXManager.singleton.PlaySound(wallImpactSFX, transform.position, sfxVolume);
            StartCoroutine(StopMove(0f));
        }
        else if (col.CompareTag("Player") && !isStopped)
        {
            SFXManager.singleton.PlaySound(penetrationSFX, transform.position, sfxVolume);
            col.transform.GetComponent<PlayerAction>().StopMove();
            col.transform.GetComponent<PlayerHealth>().TakeDamage(damage);
            StartCoroutine(StopMove());
        }

        if (col.CompareTag("Boss") && isStopped)
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator StopMove(float multiplier = 1f)
    {
        isStopped = true;
        yield return new WaitForSeconds(0.2f * multiplier);

        rb.velocity = Vector2.zero;
        if (GameObject.FindWithTag("Player").activeSelf) GameObject.FindWithTag("Player").GetComponent<PlayerAction>().StartMove();
        StartCoroutine(ReturnToHermes(waitDuration));
    }

    private IEnumerator ReturnToHermes(float time)
    {
        GameObject hermes = GameObject.FindWithTag("Boss");
        yield return new WaitForSeconds(time);
        
        Vector3 dir = (hermes.transform.position - transform.position).normalized;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        float moveOppositeDuration = Time.time + 0.2f;

        // Moves away from hermes briefly
        while (moveOppositeDuration > Time.time)
        {
            rb.velocity = -dir * 2f; 
            // Move opposite hermes dir
            yield return new WaitForSeconds(0.01f);
        }

        // Moves towards hermes
        while (true)
        {
            dir = hermes.transform.position - transform.position;
            rb.velocity = dir * 2f + dir.normalized * 5f; // Moves faster when further away
            yield return new WaitForSeconds(0.01f);
        }
    }
}