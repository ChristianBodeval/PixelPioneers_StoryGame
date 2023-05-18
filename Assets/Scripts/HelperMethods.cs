using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperMethods
{
    public static LayerMask groundLayer = LayerMask.GetMask("Ground");
    public static LayerMask obstacleLayer = LayerMask.GetMask("Obstacles");
    public static Vector2 GetRandomPointInRadiusOnGround(float radius, Vector2 center, int recursionCount = 0)
    {
        // Set a maximum number of recursive calls to prevent an infinite loop
        if (recursionCount >= 10)
        {
            Debug.LogWarning("Maximum number of recursive calls reached. Failed to find a valid random point.");
            return Vector2.zero;
        }

        Vector2 randomPoint = center + Random.insideUnitCircle * radius;
        
        bool isOnGround = Physics2D.OverlapCircle(randomPoint, 0.1f, groundLayer);
        bool isObstacleAhead = Physics2D.OverlapCircle(randomPoint, 0.1f, obstacleLayer);

        if (!isObstacleAhead && isOnGround)
        {
            Debug.Log("Hit the ground, but not an obstacle");
            return randomPoint;
        }
        // Otherwise, try again
        return GetRandomPointInRadiusOnGround(radius, center, recursionCount + 1);
    }
    
    
    
}
