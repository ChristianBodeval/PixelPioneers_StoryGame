using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Audio;

public class SegmentScript : MonoBehaviour
{
    [Header("SFX")]
    [Range(0, 1)] public float sfxVolume = 1f;
    [SerializeField] private AnimationCurve accelerationCurve;
    [SerializeField] private AudioClip sfx;

    [SerializeField] private float alphaMaximum = 0.5f;
    [HideInInspector] public float castTimeMultiplier = 1f;
    private GameObject parentBruiser;
    private SpriteRenderer sr;
    private Coroutine cr;
    private float r = 0f;
    private float g = 0f;
    private float b = 0f;
    private float a = 0f;

    private void Update()
    {
        if (!parentBruiser.activeSelf) ReturnToPool();
    }

    public IEnumerator LerpAlphaIn(float angle, float damage, GameObject parent)
    {
        bool hasPlayed = false;
        parentBruiser = parent;
        sr = GetComponent<SpriteRenderer>();
        r = sr.color.r;
        g = sr.color.g;
        b = sr.color.b;

        // Slow increase of alpha
        while (a < 1f)
        {
            if (a > 0.8f && !hasPlayed)
            {
                SFXManager.singleton.PlaySound(sfx, transform.position, sfxVolume);
                hasPlayed = true;
            }

            a += 0.1f;
            sr.color = new Color(r, g, b, accelerationCurve.Evaluate(a) * alphaMaximum);
            yield return new WaitForSeconds(0.05f * castTimeMultiplier);
        }

        // Sudden blink
        a = alphaMaximum;
        sr.color = new Color(1f, 1f, 1f, a + 0.1f);

        StartCoroutine(LerpAlphaOut(angle, damage));
    }

    private IEnumerator LerpAlphaOut(float angle, float damage)
    {
        yield return new WaitForSeconds(0.1f); // Let the flash be up for a bit
        a = 0.6f;

        ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
        ps.Play();

        // Deal damage to player
        RaycastHit2D player = Physics2D.BoxCast(transform.position, new Vector2(transform.localScale.x, transform.localScale.y), angle, Vector2.zero, 0f, LayerMask.GetMask("Player"));
        if (player.collider != null) player.collider.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);

        while (a > 0f)
        {
            a -= 0.1f;
            sr.color = new Color(r, g, b, a);
            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(1f);

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        Destroy(gameObject);
        // TODO Return to pool
    }
}
