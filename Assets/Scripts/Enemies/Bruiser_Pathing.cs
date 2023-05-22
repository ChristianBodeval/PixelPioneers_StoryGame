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
    private Path path;
    private int currentWayPoint = 0;
    private Seeker seeker;

    [Header("Custom Behavior")]
    [SerializeField] private bool isFollowing = true;
    private Slider hpBar;

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

    private void PathFollow()
    {
        Flip(); // Flips sprite

        // Guard clause
        if (path == null || currentWayPoint >= path.vectorPath.Count || false ) { return; } // Is not there yet and has a path && is not Digging
        // animator.GetBool("IsDigging")) was removed for debugging purposes


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
            path = p;
            currentWayPoint = 2;
        }
    }
}
