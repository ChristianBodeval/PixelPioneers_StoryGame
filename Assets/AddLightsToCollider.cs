using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddLightsToCollider : MonoBehaviour
{
    public GameObject lightPrefab; // The prefab for the light you want to add

    
    
    private void Start()
    {
        
        
        
        
        CompositeCollider2D collider = GetComponentInParent<CompositeCollider2D>();

        for (int i = 0; i < collider.pathCount; i++)
        {
            Vector2[] points = new Vector2[collider.GetPathPointCount(i)];
            collider.GetPath(i, points);

            for (int j = 1; j < points.Length; j+=2)
            {
                Vector3 center = (points[j] + points[(j + 1) % points.Length]) / 2f;
                
                if (transform.parent != null)
                {
                    center += transform.parent.position;
                }
                GameObject light = Instantiate(lightPrefab, center, Quaternion.identity, transform);
                
                
                
            }
        }
    }
}
