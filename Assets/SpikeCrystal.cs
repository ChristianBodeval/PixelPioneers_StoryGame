using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeCrystal : MonoBehaviour
{
    [Header("SFX")]
    [Range(0, 1)] public float sfxVolume = 1f;
    [SerializeField] private AudioClip triggeredSFX;
    [SerializeField] private AudioClip shatterSFX;

    // Health from other scripts
    private Health enemyHealth;

    // Jank way of ensuring courotine stops
    private bool isTriggered;

    // Changeable variables in inspector
    [SerializeField] private float castTime = 1.5f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float aoeRadius = 2f;
    [SerializeField] private GameObject aoeRadiusIndicator;
    [SerializeField] private AnimationCurve animationCurve;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTriggered) return;

        isTriggered = true;

        StartCoroutine(CrystalShatter());
    }

    // Coroutine will play out animations and wait accordingly, also call methods for specfic instances
    IEnumerator CrystalShatter()
    {
        // Disable collider so it can't trigger again during coroutine
        GetComponent<Collider2D>().enabled = false;

        animator.Play("Windup");
        SFXManager.singleton.PlaySound(triggeredSFX, transform.position, sfxVolume);

        // Instantiate circle
        Color lowAlphaRed = new Color(1f, 0f, 0f, 0f);
        Color highAlphaRed = new Color(1f, 0f, 0f, 0.5f);
        SpriteRenderer sr = aoeRadiusIndicator.GetComponent<SpriteRenderer>();
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f); // Not visible
        aoeRadiusIndicator.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        aoeRadiusIndicator.SetActive(true);
        aoeRadiusIndicator.transform.localScale = new Vector2(1f, 1f);
        float a = 0f;
        float t = 0f;


        // Slow increase of alpha and size of cirle
        while (a < 1f)
        {
            a += 0.1f;
            aoeRadiusIndicator.transform.localScale = new Vector2(a * aoeRadius * 2f, a * aoeRadius * 2f); // Scale size over time
            t = animationCurve.Evaluate(a);
            sr.color = Color.Lerp(lowAlphaRed, highAlphaRed, t);
            yield return new WaitForSeconds(castTime / 10);
        }

        t = 0f; // Reset value

        // Lerp color to white
        while (t < 1f)
        {
            t += 0.2f;
            sr.color = Color.Lerp(highAlphaRed, Color.white, t);
            if (t > 0.7f)
            {
                animator.Play("Shatter");
                SFXManager.singleton.PlaySound(shatterSFX, transform.position, sfxVolume);
            }

            yield return new WaitForSeconds(0.01f);
        }

        aoeRadiusIndicator.SetActive(false);
        DoDamage();

        yield return new WaitForSeconds(0.2f);

        GetComponent<ParticleSystem>().Stop();

        animator.Play("Reform");

        yield return new WaitForSeconds(8);

        // Enable collider again
        GetComponent<Collider2D>().enabled = true;
        animator.Play("Idle");

        yield return new WaitForSeconds(0.1f);

        isTriggered = false;
    }
    
    // Method for doing damage in an aoeRadius around crystal, both to player and enemy
    private void DoDamage()
    {
        GetComponent<ParticleSystem>().Play();

        Collider2D[] entitiesToDealDamageTo = Physics2D.OverlapCircleAll(transform.position, aoeRadius);

        foreach (Collider2D entity in entitiesToDealDamageTo)
        {
            if (entity.gameObject.tag == "Player")
            {
                entity.GetComponent<PlayerHealth>().TakeDamage(damage);
            }
            if (entity.gameObject.tag == "Enemy" || entity.gameObject.tag == "Charger" || entity.gameObject.tag == "Boss")
            {
                entity.GetComponent<Health>().TakeDamage(damage);
            }
        } 
    }
}
