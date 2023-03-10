using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class RangePathing : MonoBehaviour
{
    [Header("General for Pathfinding")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float activateDistance = 0.5f;
    public float attackRange = 0.5f;
    private GameObject player;
    private Rigidbody2D rb;

    [Header("A*")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float updateInterval = 0.1f;
    [SerializeField] private float nextWayPointDistance = 2f;
    private Path path;
    private int currentWayPoint = 0;
    private Seeker seeker;

    [Header("Custom Behavior")]
    [SerializeField] private bool isFollowing = true;
    private int recurses = 0;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, updateInterval); //updates pathfinding regularly
    }

    private void FixedUpdate()
    {
        //A* pathing
        if (TargetInDistance())
        {
            isFollowing = true;
            PathFollow();
        }
        else if (Vector3.Distance(player.transform.position, transform.position) < attackRange - 0.2f) // Too close to ranged enemy
        {
            isFollowing = false;
            PathFollow();
        }
        else
        {
            rb.velocity = new Vector3(0f, 0f, 0f);
        }
    }

    private void AStarMove(Vector2 dir)
    {
        //movement
        rb.velocity = speed * dir;
    }

    private Vector2 PathingTarget(Vector2 offset)
    {
        Vector2 direction = player.transform.position - transform.position; // Direction of player

        if (Vector3.Distance(player.transform.position, transform.position) > attackRange && isFollowing || Physics2D.Raycast(transform.position, direction, attackRange, obstacleLayer) && isFollowing)
        {
            recurses = 0;
            return player.transform.position;
        }

        Vector2 point = (Vector2)player.transform.position + direction * 1f + offset; // Point to walk to

        if (!isFollowing && !Physics2D.Raycast(transform.position, direction, 1f, obstacleLayer)) // Is point free of obstacles
        {
            recurses = 0;
            return point;
        }
        else if (recurses < 50)
        {
            recurses++;
            return PathingTarget(new Vector2(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f))); // Try again but with a random offset
        }
        else
        {
            recurses = 0;
            return player.transform.position;
        }
    }

    private void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(transform.position, PathingTarget(Vector2.zero), OnPathComplete);
        }
    }

    private void PathFollow()
    {
        //guard clause
        if (path == null || currentWayPoint >= path.vectorPath.Count || !isFollowing) { return; } //is not there yet and has a path

        Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - rb.position).normalized;

        AStarMove(direction);

        //swaps early to new waypoint
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);

        if (distance < nextWayPointDistance)
        {
            currentWayPoint++;
        }

        Flip();
    }

    private void Flip()
    {
        Vector2 dir = ((Vector2)player.transform.position - rb.position).normalized; //look to player

        if (dir.x > 0.24f) //right
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (dir.x < -0.24f) //left
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

    private bool TargetInDistance()
    {
        float dis = Vector2.Distance(transform.position, player.transform.position);

        // Return true if terrain is in the way
        if (Physics2D.Raycast(transform.position, player.transform.position - transform.position, attackRange, obstacleLayer) && dis > 0.5f)
        {
            return true;
        }
        return dis < activateDistance && dis > attackRange; // Return true if we are in range and not in attackrange
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWayPoint = 0;
        }
    }
}
