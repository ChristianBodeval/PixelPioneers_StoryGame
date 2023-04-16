using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentScript : MonoBehaviour
{
    SpriteRenderer sr;
    float r = 0;
    float g = 0;
    float b = 0;
    float a = 0;

    public IEnumerator LerpAlphaIn(float angle, float damage)
    {
        sr = GetComponent<SpriteRenderer>();
        r = sr.color.r;
        g = sr.color.g;
        b = sr.color.b;

        // Slow increase of alpha
        while (a < 0.5f)
        {
            a += 0.1f;
            sr.color = new Color(r, g, b, a);
            yield return new WaitForSeconds(0.02f);
        }

        // Sudden blink
        a = 1f;
        sr.color = new Color(r, g, b, a);

        StartCoroutine(LerpAlphaOut(angle, damage));
    }

    private IEnumerator LerpAlphaOut(float angle, float damage)
    {
        yield return new WaitForSeconds(0.1f); // Let the flash be up for a bit
        a = 0.6f;

        // Deal damage to player
        RaycastHit2D player = Physics2D.BoxCast(transform.position, new Vector2(transform.localScale.x, transform.localScale.y), angle, Vector2.zero, 0f, LayerMask.GetMask("Player"));
        if (player.collider != null) player.collider.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);

        while (a > 0f)
        {
            a -= 0.1f;
            sr.color = new Color(r, g, b, a);
            yield return new WaitForSeconds(0.01f);
        }

        Destroy(gameObject);
        // TODO Return to pool
    }
}
