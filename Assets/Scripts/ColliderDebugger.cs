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
    public GameObject player;


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, radius);

        
        
    }
    public static Vector2 GetRandomPointInCircle(float radius, Vector2 center, LayerMask layerMask, int recursionCount = 0)
    {
        // Set a maximum number of recursive calls to prevent an infinite loop
        if (recursionCount >= 10)
        {
            Debug.LogWarning("Maximum number of recursive calls reached. Failed to find a valid random point.");
            return Vector2.zero;
        }

        Vector2 randomPoint = center + Random.insideUnitCircle * radius;
    
        // Check if the random point is inside a collider on the ground layer, but not inside a collider on the Obstacle layer
        if (!Physics2D.OverlapCircle(randomPoint, 0.1f, layerMask))
        {
            return randomPoint;
        }
    
        // Otherwise, try again
        return GetRandomPointInCircle(radius, center, layerMask, recursionCount + 1);
    }

    public bool IsObstacleBetweenPlayer()
    {
        //Draw the ray
        Debug.DrawRay(transform.position, player.transform.position - transform.position, Color.red);
        //Check if there is an obstacle between player and enemy
        RaycastHit2D hit = Physics2D.Raycast(transform.position, 
            player.transform.position - transform.position, Vector2.Distance(transform.position, 
                player.transform.position), 
            LayerMask.GetMask(obstacleLayer.ToString()));
        
        if (hit.collider != null)
        {
            return true;
        }
        return false;
    }

    private void Update()
    {
        
        float distance = Vector2.Distance(transform.position, player.transform.position);
        Debug.DrawRay(transform.position, player.transform.position - transform.position, Color.red, distance);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, player.transform.position - transform.position, distance, obstacleLayer);
        if (hit.collider != null)
        {
            // If the ray hits a collider in the specified layer, do something
            Debug.Log("Hit object with name: " + hit.collider.name);
        }


        
        //IsObstacleBetweenPlayer();
        
        //Call when space is pressed
        if(Input.GetKeyDown(KeyCode.Space))
        {
            GetRandomPointInCircle(10f, transform.position, groundLayer);
        }
        
        
        

        if (!Physics2D.OverlapPoint(transform.position, groundLayer))
        {
            Debug.Log("Out of map");
        }


        if (Physics2D.OverlapCircle(transform.position, radius, obstacleLayer)) //If hit an obstacle or hit no collider (out of map)
        {
            Debug.Log("Hit an obstacle");
        } 
        
        if (Physics2D.OverlapCircle(transform.position, radius, groundLayer)) //If hit an obstacle or hit no collider (out of map)
        {
            Debug.Log("Hit the ground");
        }
        
        
        bool isOnGround = Physics2D.OverlapCircle(transform.position, radius, groundLayer);
        bool isObstacleAhead = Physics2D.OverlapCircle(transform.position, radius, obstacleLayer);

        if (!isObstacleAhead && isOnGround)
        {
            Debug.Log("Hit the ground, but not an obstacle");
        }

        
        
        
        
    }
}
