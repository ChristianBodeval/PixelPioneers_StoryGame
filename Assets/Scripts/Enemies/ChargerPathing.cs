using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.UI;

public class ChargerPathing : MonoBehaviour
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
    private Path path;
    private int currentWayPoint = 0;
    private Seeker seeker;

    [Header("Custom Behavior")]
    [SerializeField] private bool isFollowing = true;

    [Header("HP Bar")]
    private Slider hpBar;
    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        attackRange = GetComponent<Charger_Attack>().attackRange;
        hpBar = GetComponentInChildren<Slider>();

        StartCoroutine(UpdatePath()); // Updates pathfinding regularly
    }

    private void OnEnable()
    {
        if (seeker != null) { StartCoroutine(UpdatePath()); } // Updates pathfinding regularly - only starts if we have a seeker        
    }

    private void FixedUpdate()
    {
        FlipSprite(); // FlipSprites sprite

        //A* pathing
        if (IsTargetNotAttackable())
        {
            PathFollow();
        }
        else if (animator.GetBool("CanMove"))
        {
            rb.velocity = new Vector3(0f, 0f, 0f);
        }
    }

    // Decides where to make an A* path to
    private Vector2 PathingTarget()
    {
        isFollowing = true; // Reset variable - we want to move by default

        // If we are out of range or can't see player > move to player
        if (IsTargetNotAttackable())
        {
            return player.transform.position;
        }
        else
        {
            isFollowing = false;
            return transform.position;
        }
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
        if (path == null || currentWayPoint >= path.vectorPath.Count || !isFollowing) { return; } // Guard clause - There is no path or we are at our endpoint or not following

        Vector2 direction = (path.vectorPath[currentWayPoint] - transform.position).normalized;

        Move(direction);

        float distance = Vector2.Distance(transform.position, path.vectorPath[currentWayPoint]); // Swaps early to new waypoint

        if (distance < nextWayPointDistance)
        {
            currentWayPoint++;
        }
    }

    private void Move(Vector2 dir)
    {
        if (!animator.GetBool("CanMove") && !animator.GetBool("IsCharging")) 
        { 
            rb.velocity = Vector2.zero; 
            return; 
        }
        else if (animator.GetBool("CanMove"))
        {
            rb.velocity = dir * speed; // Movement
        }
        else if (GetComponent<Charger_Attack>().chargingCharge || animator.GetBool("IsStunned"))
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void FlipSprite()
    {
        Vector2 dir = ((Vector2)player.transform.position - rb.position).normalized; // Look to player

        if (dir.x > 0.24f && !animator.GetBool("IsCharging")) // Right
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            hpBar.transform.localScale = new Vector3(0.03175378f, 0.0259373f, 0.2461215f);
        }
        else if (dir.x < -0.24f && !animator.GetBool("IsCharging")) // Left
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            hpBar.transform.localScale = new Vector3(-0.03175378f, 0.0259373f, 0.2461215f);
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

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWayPoint = 1;
        }
    }
}
