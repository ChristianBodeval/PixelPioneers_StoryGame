using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MeleePathing : MonoBehaviour
{
    [Header("General for Pathfinding")]
    [SerializeField] private float speed = 3f;
    private float attackRange = 0.5f;
    private GameObject player;
    private Rigidbody2D rb;
    private Animator animator;

    [Header("A*")]
    [SerializeField] private LayerMask obstacleLayer;
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
        animator = GetComponent<Animator>();
        //attackRange = GetComponent<Melee_Attack>().attackRange;

        InvokeRepeating("UpdatePath", 0f, updateInterval); // Updates pathfinding regularly
    }

    private void FixedUpdate()
    {
        //A* pathing
        if (TargetNotAttackable() && !animator.GetBool("IsStunned"))
        {
            PathFollow();
        }
        else if (animator.GetBool("CanMove") || animator.GetBool("IsStunned"))
        {
            rb.velocity = new Vector3(0f,0f,0f);
        }
    }

    private void Move(Vector2 dir)
    {
        if (!animator.GetBool("CanMove") || animator.GetBool("IsStunned")) { rb.velocity = Vector2.zero; return; } // Guard clause - can we move

        rb.velocity = speed * dir; // Movement
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
        Flip(); // Flips sprite

        // Guard clause
        if (path == null || currentWayPoint >= path.vectorPath.Count || !isFollowing) { return; } // Is not there yet and has a path

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
        Vector2 dir = ((Vector2)player.transform.position - rb.position).normalized; // Look to player

        if (dir.x > 0.24f) // Right
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (dir.x < -0.24f) // Left
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

    private bool TargetNotAttackable()
    {
        float dis = Vector2.Distance(transform.position, player.transform.position);

        // Return true if terrain is in the way
        if (Physics2D.Raycast(transform.position, player.transform.position - transform.position, attackRange, obstacleLayer) && dis > attackRange)
        {
            return true;
        }
        return dis > attackRange; // Return true if we are in range and not in attackrange
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWayPoint = 1;
        }
    }
}
