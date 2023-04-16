using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class RangePathing : MonoBehaviour
{
    [Header("General for Pathfinding")]
    [SerializeField] private float speed = 3f;
    private float attackRange;
    private GameObject player;
    private Rigidbody2D rb;
    private Animator animator;

    [Header("A*")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float updateInterval = 0.1f;
    [SerializeField] private float nextWayPointDistance = 2f;
    [SerializeField] private float offset = 0.2f;
    private Path path;
    private int currentWayPoint = 0;
    private bool canGeneratePath = true;
    private Seeker seeker;

    [Header("Custom Behavior")]
    [SerializeField] private float fleeStickToPathDuration = 0.5f;
    private Coroutine fleeStickToPath;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        attackRange = GetComponent<Range_Attack>().attackRange;
        animator.SetBool("CanMove", true);

        StartCoroutine(UpdatePath()); // Updates pathfinding regularly
    }

    private void OnEnable()
    {
        if (animator != null) animator.SetBool("CanMove", true);

        if (seeker != null) { StartCoroutine(UpdatePath()); } // Updates pathfinding regularly - only starts if we have a seeker
        canGeneratePath = true;
    }

    private void FixedUpdate()
    {
        FlipSprite(); // FlipSprites sprite

        //A* pathing
        if ((IsTargetNotAttackable() || IsTargetTooClose()) && !animator.GetBool("CannotTransitionState") && !animator.GetBool("IsStunned") && animator.GetBool("CanMove"))
        {
            PathFollow();
        }
        else
        {
            animator.SetBool("IsFleeing", false); // Reset variable - only attacks if enemy isn't fleeing
            rb.velocity = Vector3.zero;
        }
    }

    // Decides where to make an A* path to
    private Vector2 PathingTarget()
    {
        // If we are out of range or can't see player > move to player
        if (IsTargetNotAttackable()) 
        {
            animator.SetBool("IsFleeing", false);
            return player.transform.position;
        }

        // Player is too close and we must move away
        Vector2 playerPos = player.transform.position;
        Vector2 direction = (playerPos - (Vector2)transform.position).normalized; // Direction to player
        Vector2 point = (Vector2)transform.position - direction * 1.5f; // Point to walk to
        
        if (IsTargetTooClose() && !Physics2D.OverlapPoint(point, obstacleLayer) && Physics2D.OverlapPoint(point, groundLayer)) // Player is too close & point can be walked to
        {
            return point;
        }

        // Point was not free of obstacles and we try again with some offset
        for (int i = 0; i < 20; i++) // x axis offset
        {
            Vector2 offsetPos = new Vector2(offset * i, offset * i);

            if (!Physics2D.OverlapPoint(point + offsetPos, obstacleLayer) && Physics2D.OverlapPoint(point + offsetPos, groundLayer) && Vector2.Distance(playerPos, point) > Vector2.Distance(playerPos, point + offsetPos))
            {
                return point + new Vector2(offset * i, offset * i);
            }
            else if (!Physics2D.OverlapPoint(point - offsetPos, obstacleLayer) && Physics2D.OverlapPoint(point - offsetPos, -groundLayer) && Vector2.Distance(playerPos, point) > Vector2.Distance(playerPos, point - offsetPos))
            {
                return point + new Vector2(-offset * i, -offset * i);
            }
        }

        return transform.position; // Default if we can't find a spot
    }

    private IEnumerator UpdatePath()
    {
        while (gameObject.activeSelf)
        {
            if (seeker.IsDone()) { seeker.StartPath(transform.position, PathingTarget(), OnPathComplete); } // Update path

            yield return new WaitForSeconds(updateInterval);
        }
    }

    private void PathFollow()
    {
        if (path == null || currentWayPoint >= path.vectorPath.Count) { return; } // Guard clause - There is no path or we are at our endpoint or not following

        Vector2 direction;
        if (!animator.GetBool("IsFleeing"))
        {
            direction = (path.vectorPath[currentWayPoint] - transform.position).normalized;
        }
        else
        {
            direction = (path.vectorPath[path.vectorPath.Count - 1] - transform.position).normalized;
        }

        Move(direction);

        float distance = Vector2.Distance(transform.position, path.vectorPath[currentWayPoint]); // Swaps early to new waypoint

        if (distance < nextWayPointDistance)
        {
            currentWayPoint++;
        }
    }

    private void Move(Vector2 dir)
    {
        if (!animator.GetBool("CanMove")) { rb.velocity = Vector2.zero; return; } // Guard clause - can we move

        rb.velocity = dir * speed; // Movement
    }

    private void FlipSprite()
    {
        if (!animator.GetBool("CanMove") || animator.GetBool("IsStunned")) return; // Guard clause - don't flip if stunned

        Vector2 dir;

        if (animator.GetBool("IsFleeing"))
        {
            if (path == null || currentWayPoint >= path.vectorPath.Count) { return; } // Guard clause - There is no path or we are at our endpoint or not following

            dir = (path.vectorPath[currentWayPoint] - transform.position).normalized; // Look away from player
        }
        else
        {
            dir = (player.transform.position - transform.position).normalized; // Look to player
        }

        if (dir.x > 0.24f) // Right
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (dir.x < -0.24f) // Left
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

    private bool IsTargetNotAttackable()
    {
        float dis = Mathf.Abs(Vector2.Distance(player.transform.position, transform.position));

        // Return true if terrain is in the way
        if (Physics2D.Raycast(transform.position, player.transform.position - transform.position, attackRange, obstacleLayer))
        {
            return true;
        }
        return dis > attackRange; // Return true if we are not in attackrange
    }

    private bool IsTargetTooClose()
    {
        float dis = Mathf.Abs(Vector2.Distance(player.transform.position, transform.position));

        // Return false if terrain is in the way
        if (Physics2D.Raycast(transform.position, player.transform.position - transform.position, attackRange, obstacleLayer))
        {
            return false;
        }

        if (dis < attackRange - 0.5f) // Return true if we are in range and not in attackrange - -0.5f so enemy has a headzone where it does not move
        {
            animator.SetBool("IsFleeing", true);
            GetComponent<Enemy_Attack>().AttackCD(0.5f); // Put attack on cd
            return true;
        }
        else
        {
            return false;
        }
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
