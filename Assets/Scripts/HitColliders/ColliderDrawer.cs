using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using Color = UnityEngine.Color;
//TODO Require SpriteShape

[ExecuteAlways]
public class ColliderDrawer : MonoBehaviour
{
    public List<GameObject> targets = new List<GameObject>();


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!targets.Contains(collision.gameObject))
        {
            targets.Add(collision.gameObject);
            Debug.Log("Added " + gameObject.name);
            Debug.Log("GameObjects in list: " + targets.Count);
        }        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (targets.Contains(collision.gameObject))
        {
            targets.Remove(collision.gameObject);
            Debug.Log("Removed " + collision.name);
            Debug.Log("GameObjects in list: " + targets.Count);
        }
        
    }


    //Sprite handling
    public SpriteShapeController spriteShapeController;
    public SpriteShapeRenderer spriteShapeRenderer;

    //Points
    private List<Vector3> endPoints;
    private List<Vector3> points;


    public bool DrawInInspector;

    public ColliderStat colliderStat;


    //Attributes
    private float range, width, angle;
    private int corners;

    private void Awake()
    {
        range = colliderStat.range;
        width = colliderStat.width;
        angle = colliderStat.angle;
        corners = colliderStat.corners;
    }

    private void OnValidate()
    {
        //Use this instead of update, to save processing power
    }

    private void Update()
    {
        range = colliderStat.range;
        width = colliderStat.width;
        angle = colliderStat.angle;
        corners = colliderStat.corners;
        points = GetPolygonPoints();
        UpdateSpriteShapeController(points);
    }


    void UpdateSpriteShapeController(List<Vector3> points)
    {
        //TODO test for when there is more existing points than what is needed
        spriteShapeController.spline.Clear();

        for (int i = 0; i < points.Count; i++)
        {
            spriteShapeController.spline.InsertPointAt(i, points[i]);
        }
    }


    private void OnDrawGizmos()
    {
        if(DrawInInspector)
        {
            points = GetPolygonPoints();
            UpdateSpriteShapeController(points);

            //Draw points
            Gizmos.color = Color.red;
            foreach (var point in points)
            {
                Gizmos.DrawSphere(point+transform.position, 0.1f);
            }

            //Draw lines between points
            Vector3 lastPoint;
            Gizmos.color = Color.green;
            lastPoint = points[0];
            foreach (var point in points)
            {
                if (lastPoint != point)
                {
                    Gizmos.DrawLine(lastPoint+ transform.position, point+ transform.position);
                }
                lastPoint = point;
            }
            Gizmos.DrawLine(lastPoint+ transform.position, points[0]+ transform.position);
        }
    }



    List<Vector3> GetPolygonPoints()
    {
        //Bugfix
        if (width == 0 && angle < 5)
            angle = 5f;

        endPoints = new List<Vector3>();
        //For one corner, just update

        //Step values
        float angleStep = angle / (corners - 1);
        float pointDistanceStep = width / (corners - 1);
        //Current values
        float currentPointDistance = 0 - (width / 2);
        //Use instead
        //float currentAngle = 0 - (angle / 2) + transform.rotation.eulerAngles.z;
        float currentAngle = 0 - (angle / 2);

            for (int i = 0; i < corners; i++)
            {
                Vector3 start = transform.up * currentPointDistance;                                //Gets start position by moving the point a little up or down dependenet on currentPointDistance
                Vector3 direction = GetVectorFromAngle(currentAngle);                               //Vector from the angle
                Vector3 target = direction * range + start;                                         //Ending of the line

                endPoints.Add(target);

                currentPointDistance += pointDistanceStep;
                currentAngle += angleStep;
            }

        List<Vector3> returnList = new List<Vector3>();

        foreach (Vector3 point in endPoints)
        {
            returnList.Add(point);
        }

        if (Mathf.Approximately(angle, 360f))
        {
            Vector3 firstValue = returnList[0];
            Vector3 lastValue = returnList[returnList.Count-1];
            Vector3 finalValue = firstValue + 0.5f * (lastValue - firstValue);


            returnList.RemoveAt(returnList.Count-1);
            returnList.RemoveAt(0);

            returnList.Add(finalValue);
        }
        
        else if(width > 0)
        {
            returnList.Add(Vector3.zero + transform.up * 0.5f * width);
            returnList.Add(Vector3.zero - transform.up * 0.5f * width);
        }

        else { 
            returnList.Add(Vector3.zero);
        }
        return returnList;
    }


    //Helper function to get an angle from any vector
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