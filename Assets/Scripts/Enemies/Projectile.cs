using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Obstacles") || col.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
