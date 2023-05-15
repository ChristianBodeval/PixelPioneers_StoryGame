using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashDashArea : MonoBehaviour
{
    private void Update()
    {
        transform.position = GameObject.Find("Player").transform.position;
        transform.rotation = GameObject.Find("Player").transform.rotation;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Health>().TakeDamage(1);
        }
    }
}
