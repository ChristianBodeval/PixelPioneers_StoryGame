using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float projectileDMG = 0f;

    public void SetProjectileDMG(float dmg)
    {
        projectileDMG = dmg;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            col.gameObject.GetComponent<PlayerHealth>().TakeDamage(0.5f);
        }

        if (col.CompareTag("Obstacles") || col.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
