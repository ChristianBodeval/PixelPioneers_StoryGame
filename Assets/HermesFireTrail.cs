using System.Collections;
using UnityEngine;

public class HermesFireTrail : MonoBehaviour
{
    public float timeToDie = 14f;
    public float damage = 1f;
    public float tickRate = 0.2f;
    private Vector2 boxCastSize;
    private static bool isPlayerHit = false;
    private static bool isCooldownActive = false;

    private void Start()
    {
        boxCastSize = GetComponent<BoxCollider2D>().size;
        StartCoroutine(DieAfterTime(timeToDie));
    }

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxCastSize, 0f, Vector2.zero, LayerMask.GetMask("Player"));
        if (hit.collider.CompareTag("Player") && !isPlayerHit && !isCooldownActive)
        {
            hit.transform.GetComponent<PlayerHealth>().TakeDamage(damage);
            isPlayerHit = true;
            isCooldownActive = true;
            StartCoroutine(ResetCooldown());
        }
    }

    private IEnumerator DieAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    private IEnumerator ResetCooldown()
    {
        yield return new WaitForSeconds(tickRate);
        isPlayerHit = false;
        isCooldownActive = false;
    }

    private void OnDestroy()
    {
        isPlayerHit = false;
        isCooldownActive = false;
    }
}
