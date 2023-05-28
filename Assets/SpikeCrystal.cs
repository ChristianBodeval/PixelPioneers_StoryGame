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
    [SerializeField] private float damage = 10f;
    [SerializeField] private float aoe = 2f;

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

        // Bunch of animation nonsense
        Animator animator = GetComponent<Animator>();
        animator.SetBool("IsTriggered", true);
        SFXManager.singleton.PlaySound(triggeredSFX, transform.position, sfxVolume);

        yield return new WaitForSeconds(2);
        
        animator.SetBool("IsShatter", true);
        SFXManager.singleton.PlaySound(shatterSFX, transform.position, sfxVolume);
        DoDamage();

        yield return new WaitForSeconds(0.2f);

        GetComponent<ParticleSystem>().Stop();

        yield return new WaitForSeconds(7);

        animator.SetBool("IsReform", true);

        yield return new WaitForSeconds(1);

        // Reset bools
        animator.SetBool("IsTriggered", false);
        animator.SetBool("IsShatter", false);
        animator.SetBool("IsReform", false);

        // Enable collider again
        GetComponent<Collider2D>().enabled = true;
        animator.SetBool("IsIdle", true);

        yield return new WaitForSeconds(1f);

        isTriggered = false;
    }
    
    // Method for doing damage in an aoe around crystal, both to player and enemy
    private void DoDamage()
    {
        GetComponent<ParticleSystem>().Play();

        Collider2D[] entitiesToDealDamageTo = Physics2D.OverlapCircleAll(transform.position, aoe);

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
