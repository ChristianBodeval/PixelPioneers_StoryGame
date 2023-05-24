using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using Color = UnityEngine.Color;
//TODO Require SpriteShape

[ExecuteAlways]
public class ColliderDrawer : MonoBehaviour
{
    public List<GameObject> targets;
    

    [SerializeField] private LayerMask enemyLayer;
    

    private void Start()
    {
        // Set the enemy layer using the layer name
        enemyLayer = LayerMask.GetMask("Enemy");

        // Alternatively, set the enemy layer using the layer index
        // enemyLayer = 1 << LayerMask.NameToLayer("Enemies");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Make a layer mask for enemies
        
        
        
        
        if (!targets.Contains(collision.gameObject) && enemyLayer == (enemyLayer | (1 << collision.gameObject.layer)))
        {
            targets.Add(collision.gameObject);
        }        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (targets.Contains(collision.gameObject) && enemyLayer == (enemyLayer | (1 << collision.gameObject.layer)))
        {
            targets.Remove(collision.gameObject);
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
    

    public void UpdateCollider()
    {
        points = GetPolygonPoints();
        UpdateSpriteShapeController();
    }


    public void UpdateSpriteShapeController()
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
            UpdateSpriteShapeController();

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
        if (width == 0 && colliderStat.angle < 5)
            colliderStat.angle = 5f;

        endPoints = new List<Vector3>();
        //For one corner, just update

        //Step values
        float angleStep = colliderStat.angle / (colliderStat.corners - 1);
        float pointDistanceStep = colliderStat.width / (colliderStat.corners - 1);
        //Current values
        float currentPointDistance = 0 - (colliderStat.width / 2);
        //Use instead
        //float currentAngle = 0 - (angle / 2) + transform.rotation.eulerAngles.z;
        float currentAngle = 0 - (colliderStat.angle / 2);

            for (int i = 0; i < colliderStat.corners; i++)
            {
                Vector3 start = Vector3.up * currentPointDistance;                                //Gets start position by moving the point a little up or down dependenet on currentPointDistance
                Vector3 direction = HelperMethods.GetVectorFromAngle(currentAngle);                               //Vector from the angle
                Vector3 target = direction * colliderStat.range + start;                                         //Ending of the line

                endPoints.Add(target);

                currentPointDistance += pointDistanceStep;
                currentAngle += angleStep;
            }

        List<Vector3> returnList = new List<Vector3>();

        foreach (Vector3 point in endPoints)
        {
            returnList.Add(point);
        }

        if (Mathf.Approximately(colliderStat.angle, 360f))
        {
            Vector3 firstValue = returnList[0];
            Vector3 lastValue = returnList[returnList.Count-1];
            Vector3 finalValue = firstValue + 0.5f * (lastValue - firstValue);


            returnList.RemoveAt(returnList.Count-1);
            returnList.RemoveAt(0);

            returnList.Add(finalValue);
        }
        
        if(colliderStat.width > 0)
        {
            returnList.Add(Vector3.zero + Vector3.up * 0.5f * colliderStat.width);
            returnList.Add(Vector3.zero - Vector3.up * 0.5f * colliderStat.width);
        }

        else { 
            returnList.Add(Vector3.zero);
        }
        return returnList;
    }
    
}
