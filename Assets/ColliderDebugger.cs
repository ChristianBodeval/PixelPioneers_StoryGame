using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

[ExecuteAlways]
public class ColliderDebugger : MonoBehaviour
{

    public float radius;

    public LayerMask obstacleLayer;
    public LayerMask groundLayer;
    public LayerMask enemyLayer;


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, radius);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer.Equals(enemyLayer))
        {
            Debug.Log("Enemy in trigger");
        }
        
    }


    private void Update()
    {


        if (!Physics2D.OverlapPoint(transform.position, groundLayer))
        {
            Debug.Log("Out of map");
        }


        if (Physics2D.OverlapCircle(transform.position, radius, obstacleLayer)) //If hit an obstacle or hit no collider (out of map)
        {
            Debug.Log("Hit an obstacle");
        }        
    }
}
