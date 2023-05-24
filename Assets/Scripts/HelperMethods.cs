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
    
    public static float GetAngleFromVector(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        float angle = n;

        return angle;
    }
    //Helper function to get a vector from any angle. -> Angle is between 0 and 360
    public static Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }
    
    
    
}
