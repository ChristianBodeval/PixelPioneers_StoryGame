using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeCrystal : MonoBehaviour
{
    // Health from other scripts
    private Health enemyHealth;
    private PlayerHealth playerHealth;

    // Collider attached to gameobject
    private Collider2D collider;

    // Jank way of ensuring courotine stops
    private bool stopCoroutine;

    // Changeable variables in inspector
    [SerializeField] private float damage = 10f;
    [SerializeField] private float aoe = 2f;

    private void Update()
    {
        // Ensuring couroutine plays out and then disables coroutine - so it can start again
        if (stopCoroutine = true)
        {
            StopCoroutine(CrystalShatter());

            stopCoroutine = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Establising collider
        collider = collision;

        StartCoroutine(CrystalShatter());

        // Getting player health

        if (collision.gameObject.tag == "Player")
        {
            playerHealth = collision.GetComponent<PlayerHealth>();
        }

        // Getting enemy health

        if (collision.gameObject.tag == "Enemy")
        {
            enemyHealth = collision.GetComponent<Health>();
        }
    }

    // Coroutine will play out animations and wait accordingly, also call methods for specfic instances
    IEnumerator CrystalShatter()
    {
        // Disable collider so it can't trigger again during coroutine
        GetComponent<Collider2D>().enabled = false;

        // Bunch of animation nonsense
        Animator animator = GetComponent<Animator>();

        animator.SetBool("IsTriggered", true);

        yield return new WaitForSeconds(2);

        animator.SetBool("IsTriggered", false);

        animator.SetBool("IsShatter", true);

        // Calling method
        yield return new WaitForSeconds(1);

        DoDamage();

        // Further animation nonsense
        yield return new WaitForSeconds(1);

        animator.SetBool("IsShatter", false);

        yield return new WaitForSeconds(5);

        animator.SetBool("IsReform", true);

        yield return new WaitForSeconds(1);

        animator.SetBool("IsReform", false);

        // Enable collider again
        GetComponent<Collider2D>().enabled = true;

        animator.SetBool("IsIdle", true);

        // Stops coroutine as it ends
        stopCoroutine = true;

    }
    
    // Method for doing damage in an aoe around crystal, both to player and enemy
    private void DoDamage()
    {

        Collider2D[] entitiesToDealDamageTo = Physics2D.OverlapCircleAll(gameObject.transform.position, aoe, LayerMask.GetMask("Player", "Enemy"));


        foreach (Collider2D entity in entitiesToDealDamageTo)
        {
            if (collider.gameObject.tag == "Player")
            {
                playerHealth.TakeDamage(damage);
            }
            if (collider.gameObject.tag == "Enemy")
            {
                enemyHealth.TakeDamage(damage);
            }
        } 
    }
}
