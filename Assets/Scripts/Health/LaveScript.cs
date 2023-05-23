using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;




// Adds damage to objects inside a trigger, which has a Health script attached.
// Deals {damage} every {takeDamageEverySeconds}.

public class LaveScript : MonoBehaviour
{
    public float damage;
    public float takeDamageEverySeconds;

    public Dictionary<Health, Coroutine> healthCoroutines;

    private void Start()
    {
        healthCoroutines = new Dictionary<Health, Coroutine>();
        GetComponent<CompositeCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("TriggerEnter: " + collision.gameObject.name);

        Health health = collision.gameObject.GetComponent<Health>();

        if (health != null && !healthCoroutines.ContainsKey(health))
        {
            Coroutine coroutine = StartCoroutine(DealDamageRepeatedly(health));
            healthCoroutines.Add(health, coroutine);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("TriggerExit: " + collision.gameObject.name);

        Health health = collision.gameObject.GetComponent<Health>();

        if (health != null && healthCoroutines.ContainsKey(health))
        {
            Coroutine coroutine = healthCoroutines[health];
            StopCoroutine(coroutine);
            healthCoroutines.Remove(health);
        }
    }
    
    private bool IsColliderFullyInside(Collider2D collider1, Collider2D collider2)
    {
        UnityEngine.Bounds bounds1 = collider1.bounds;
        UnityEngine.Bounds bounds2 = collider2.bounds;

        // Check if bounds2 fully contains bounds1
        return bounds2.Contains(bounds1.min) && bounds2.Contains(bounds1.max);
    }

    private IEnumerator DealDamageRepeatedly(Health health)
    {
        while (true)
        {
            Debug.Log("Dictionary length" + healthCoroutines.Count);
            if (!IsColliderFullyInside(health.gameObject.GetComponent<Collider2D>(), GetComponent<CompositeCollider2D>()))
            {
                Debug.Log("Collider is not fully inside");
                yield return new WaitForSeconds(takeDamageEverySeconds);
                break;
            }
            health.TakeDamage(damage);
            yield return new WaitForSeconds(takeDamageEverySeconds);
        }
    }
}