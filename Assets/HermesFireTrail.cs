using System.Collections;
using UnityEngine;

public class HermesFireTrail : MonoBehaviour
{
    public float timeToDie = 14f;
    public float damage = 1f;
    public float tickRate = 0.2f;
    private float lastTick;
    private Vector2 boxCastSize;

    private void Start()
    {
        boxCastSize = GetComponent<BoxCollider2D>().size;
        StartCoroutine(DieAfterTime(timeToDie));
        lastTick = Time.time;
    }

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxCastSize, 0f, Vector2.zero, LayerMask.GetMask("Player"));
        if (hit.collider.CompareTag("Player") && lastTick + tickRate < Time.time)
        {
            hit.transform.GetComponent<PlayerHealth>().TakeDamage(damage);
            lastTick = Time.time;
        }
    }

    private IEnumerator DieAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
