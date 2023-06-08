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
    private Image hpBar;
    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        animator.SetBool("CanMove", true);
        attackRange = GetComponent<Charger_Attack>().attackRange;

        foreach (var item in GetComponentsInChildren<Image>())
        {
            if (item.gameObject.name == "Fill") hpBar = item;
        }

        StartCoroutine(UpdatePath()); // Updates pathfinding regularly
    }

    private void OnEnable()
    {
        if (seeker != null) { StartCoroutine(UpdatePath()); } // Updates pathfinding regularly - only starts if we have a seeker        
        if (animator != null) animator.SetBool("CanMove", true);
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
            if (seeker.IsDone()) { seeker.StartPath(transform.position, PathingTarget(), OnPathComplete); } // StateUpdate path

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
        if (animator.GetBool("CanMove") && !animator.GetBool("IsStunned") && !animator.GetBool("IsCharging"))
        {
            float modifier = Vector2.Distance(player.transform.position, transform.position) > 14f ? ((player.transform.position - transform.position).magnitude / 3f) + 1f : 1f;

            rb.velocity = dir * speed * modifier; // Movement
        }
        else if (animator.GetBool("IsStunned") || (!animator.GetBool("CanMove") && !animator.GetBool("IsCharging")) )
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void FlipSprite()
    {
        if (animator.GetBool("IsStunned")) return;

        Vector2 dir = ((Vector2)player.transform.position - rb.position).normalized; // Look to player

        if (dir.x > 0.24f && !animator.GetBool("IsCharging")) // Right
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            hpBar.transform.localScale = new Vector3(Mathf.Abs(hpBar.transform.localScale.x) * 1f, hpBar.transform.localScale.y, hpBar.transform.localScale.z);
        }
        else if (dir.x < -0.24f && !animator.GetBool("IsCharging")) // Left
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            hpBar.transform.localScale = new Vector3(Mathf.Abs(hpBar.transform.localScale.x) * -1f, hpBar.transform.localScale.y, hpBar.transform.localScale.z);
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
