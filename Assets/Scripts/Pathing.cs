using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Pathing : MonoBehaviour
{
    [Header("General for Pathfinding")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float activateDistance = 0.5f;
    private GameObject player;
    private Rigidbody2D rb;

    [Header("A*")]
    [SerializeField] private float updateInterval = 0.1f;
    [SerializeField] private float nextWayPointDistance = 2f;
    private Path path;
    private int currentWayPoint = 0;
    private Seeker seeker;

    [Header("Custom Behavior")]
    [SerializeField] private bool isFollowing = true;

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
            PathFollow();
        }
        else
        {
            rb.velocity = new Vector3(0f,0f,0f);
        }
    }

    private void AStarMove(Vector2 dir)
    {
        //movement
        rb.velocity = speed * dir;
    }

    private void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(transform.position, player.transform.position, OnPathComplete);
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
        return dis < activateDistance && dis > 0.5f;
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
