using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CircleCastTestDELETE : MonoBehaviour
{
    Collider2D[] hits;

    public Vector3 upVector;
    public float distance;
    public float radius;

    // Update is called once per frame
    void Update()
    {
        hits = Physics2D.OverlapCircleAll(transform.position, radius,LayerMask.GetMask("Enemy"));

        Debug.Log(hits.Length);


        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
