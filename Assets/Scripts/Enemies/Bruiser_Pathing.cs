using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.UI;

public class Bruiser_Pathing : MonoBehaviour
{
    [Header("General for Pathfinding")]
    [SerializeField] private float speed = 3f;
    private float attackRange;
    private GameObject player;
    private Rigidbody2D rb;
    private Animator animator;

    [Header("A*")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float updateInterval = 0.1f;
    [SerializeField] private float nextWayPointDistance = 2f;
    [SerializeField] private float unitRadius;
    private Path path;
    private int currentWayPoint = 0;
    private Seeker seeker;

    [Header("Custom Behavior")]
    [SerializeField] private bool isFollowing = true;
    private Slider hpBar;
    private object normalized;

    private List<Vector2> debug1 = new List<Vector2>();
    private List<Vector2> debug2 = new List<Vector2>();

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        attackRange = GetComponent<Bruiser_Attack>().waveRange / 2 - 0.5f;
        hpBar = GetComponentInChildren<Slider>();
        hpBar.maxValue = GetComponent<Health>().maxHealth;
        hpBar.value = GetComponent<Health>().currentHealth;

        InvokeRepeating("UpdatePath", 0f, updateInterval); // Updates pathfinding regularly
    }

    private void FixedUpdate()
    {       
        if (TargetNotAttackable())
        {
            PathFollow();
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Wave"))
        {
            Flip(); // Flips sprite
            rb.velocity = Vector2.zero;
        }
    }

    private void Move(Vector2 dir)
    {
        if (!animator.GetBool("CanMove")) { rb.velocity = Vector2.zero; return; } // Guard clause - can we move

        float modifier = (Vector2.Distance(player.transform.position, transform.position) > 14f) ? ((player.transform.position - transform.position).magnitude / 3f) + 1f : 1f;

        rb.velocity = speed * dir * modifier; // Movement
    }

    private void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(transform.position, player.transform.position, OnPathComplete);
        }
    }

    public Vector3 FindClosestPointOnLineAndGetVectorAway(Vector3 pointA, Vector3 pointB, Vector3 thirdPoint)
    {
        Vector3 lineVector = pointB - pointA;
        float lineLengthSquared = Vector3.SqrMagnitude(lineVector);

        if (lineLengthSquared == 0f)
        {
            // If the line has no length, return pointA as the closest point
            return pointA;
        }

        float t = Vector3.Dot(thirdPoint - pointA, lineVector) / lineLengthSquared;
        t = Mathf.Clamp01(t);

        Vector3 closestPoint = pointA + t * lineVector;

        return (closestPoint - thirdPoint).normalized;
    }

    private void PathFollow()
    {
        Flip(); // Flips sprite

        // Guard clause
        if (path == null || currentWayPoint >= path.vectorPath.Count || false ) { return; } // Is not there yet and has a path && is not Digging

        Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - rb.position).normalized;

        Move(direction);

        // Swaps early to new waypoint
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);

        if (distance < nextWayPointDistance)
        {
            currentWayPoint++; 
        }
    }

    private void Flip()
    {
        if (!GetComponentInChildren<Animator>().GetBool("CanMove") && !animator.GetCurrentAnimatorStateInfo(0).IsTag("Wave")) return;

        Vector2 dir = ((Vector2)player.transform.position - rb.position).normalized; // Look to player

        if (dir.x > 0.24f) // Right
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            hpBar.transform.localScale = new Vector3(Mathf.Abs(hpBar.transform.localScale.x) * 1f, hpBar.transform.localScale.y, hpBar.transform.localScale.z);
        }
        else if (dir.x < -0.24f) // Left
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            hpBar.transform.localScale = new Vector3(Mathf.Abs(hpBar.transform.localScale.x) * -1f, hpBar.transform.localScale.y, hpBar.transform.localScale.z);
        }
    }

    private bool TargetNotAttackable()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Wave")) return false;

        float dis = Vector2.Distance(transform.position, player.transform.position);

        // Return true if terrain is in the way
        if (Physics2D.Raycast(transform.position, player.transform.position - transform.position, attackRange, obstacleLayer))
        {
            return true;
        }
        return dis > attackRange; 
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            currentWayPoint = 2;

            // Get the calculated path
            Vector3[] waypoints = p.vectorPath.ToArray();
            p.vectorPath.Clear();

            // Adjust the waypoints to account for unit's collider size
            for (int i = 0; i < waypoints.Length; i++)
            {
                Vector3 temp = CheckCollision(waypoints[i]);
                debug1.Add(temp);
                debug2.Add(waypoints[i] + (waypoints[i] - temp).normalized * unitRadius);
                p.vectorPath.Add(waypoints[i] + (waypoints[i] - temp).normalized * unitRadius);
            }

            path = p;
        }
    }

    private Vector2 CheckCollision(Vector2 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, unitRadius);
        Vector2 dir = Vector3.zero;

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Obstacles"))
            {
                dir += collider.ClosestPoint(position);
                return collider.ClosestPoint(position);
            }
        }
        return position;
    }


    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;

        foreach (var item in debug1)
        {
            Gizmos.DrawSphere(item, 0.1f);
        }

        foreach (var item in debug2)
        {
            Gizmos.DrawCube(item, new Vector3(0.1f, 0.1f, 0.1f));
        }

        debug1.Clear();
        debug2.Clear();
    }
}
