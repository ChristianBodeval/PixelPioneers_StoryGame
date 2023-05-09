using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Hermes_Pathing : MonoBehaviour
{
    [Header("General for Pathfinding")]
    [SerializeField] private float speed = 3f;
    private float attackRange = 0.5f;
    private GameObject player;
    private Rigidbody2D rb;
    private Animator animator;

    [Header("A*")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float updateInterval = 0.05f;
    [SerializeField] private float nextWayPointDistance = 2f;
    [SerializeField] private float offset = 0.2f;
    private Path path;
    private int currentWayPoint = 0;
    private Seeker seeker;

    [Header("Custom Behavior")]
    [SerializeField] private float attackDeadZone = 0.8f;
    [SerializeField] private bool isFollowing = true;
    [SerializeField] private float newPositionRange = 2f;
    [SerializeField] private float newPositionSpeed = 15f;
    [SerializeField] private float waitDurationRunAway = 1f;
    private bool isRunningAway = false;
    private bool isReadyToRunAway = false;
    private bool isWaitingToRunAwayCoroutine = false;
    private Coroutine newPositionCoroutine;
    private Coroutine movetoPositionCoroutine;
    private Coroutine waitCoroutine;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        animator.SetBool("CanMove", true);
        attackRange = GetComponent<Hermes_Attack>().attackRange;

        StartCoroutine(UpdatePath()); // Updates pathfinding regularly
    }

    private void OnEnable()
    {
        if (animator != null) animator.SetBool("CanMove", true);
    }

    private void FixedUpdate()
    {       
        if ( (IsTargetNotAttackable() || IsTargetTooClose() ) && !animator.GetBool("IsStunned"))
        {
            PathFollow();
        }
        else if (animator.GetBool("CanMove") || animator.GetBool("IsStunned"))
        {
            rb.velocity = new Vector3(0f,0f,0f);
        }
    }

    private void PathFollow()
    {
        Flip(); // Flips sprite

        // Guard clause
        if (path == null || currentWayPoint >= path.vectorPath.Count) { return; } // Is not there yet and has a path && is not Digging

        Vector2 direction = (path.vectorPath[currentWayPoint] - transform.position).normalized;

        Move(direction);

        // Swaps early to new waypoint
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);

        if (distance < nextWayPointDistance)
        {
            currentWayPoint++;
        }
    }

    private void Move(Vector2 dir)
    {
        if (animator.GetBool("IsStunned") || animator.GetCurrentAnimatorStateInfo(0).IsTag("Immobile")) { rb.velocity = Vector2.zero; return; } // Guard clause

        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Ready") && animator.GetBool("CanMove") && !animator.GetBool("IsFleeing") && !animator.GetCurrentAnimatorStateInfo(0).IsTag("Special")) // Only Move and Idle states are tagged 'Ready'
        {
            rb.velocity = speed * dir; // Movement
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

    // Decides where to make an A* path to
    private Vector2 PathingTarget()
    {
        // Player out of reach
        if (IsTargetNotAttackable())
        {
            isReadyToRunAway = false;
            return player.transform.position;
        }

        // In the deadzone
        if (!IsTargetTooClose())
        {
            isReadyToRunAway = false;
            return transform.position;
        }
        // Too close to player
        else if (!isReadyToRunAway)
        {
            // Start a waittime
            GetComponent<Enemy_Attack>().AttackCD(0.5f); // Put attack on cd
            if (!isWaitingToRunAwayCoroutine) { isWaitingToRunAwayCoroutine = true; waitCoroutine = StartCoroutine(WaitBeforeMovingAway(waitDurationRunAway)); }
            return transform.position;
        }

        // Player is too close and we must move away
        Vector2 playerPos = player.transform.position;
        Vector2 direction = (playerPos - (Vector2)transform.position).normalized; // Direction to player
        Vector2 point = (Vector2)transform.position - direction * 1.5f; // Position away from player

        // Is point free to walk to?
        if (!Physics2D.OverlapPoint(point, obstacleLayer) && Physics2D.OverlapPoint(point, groundLayer)) 
        {
            return point;
        }

        // Point was not free of obstacles, we try again
        for (int i = 0; i < 20; i++) // x axis offset
        {
            Vector2 offsetPos = new Vector2(offset * i, offset * i);

            if (!Physics2D.OverlapPoint(point + offsetPos, obstacleLayer) && Physics2D.OverlapPoint(point + offsetPos, groundLayer) && Vector2.Distance(playerPos, transform.position) > Vector2.Distance(playerPos, point + offsetPos))
            {
                return point + new Vector2(offset * i, offset * i);
            }
            else if (!Physics2D.OverlapPoint(point - offsetPos, obstacleLayer) && Physics2D.OverlapPoint(point - offsetPos, groundLayer) && Vector2.Distance(playerPos, transform.position) > Vector2.Distance(playerPos, point - offsetPos))
            {
                return point + new Vector2(-offset * i, -offset * i);
            }
            else if (!Physics2D.OverlapPoint(point + new Vector2(-offsetPos.x, offsetPos.y), obstacleLayer) && Physics2D.OverlapPoint(point + new Vector2(-offsetPos.x, offsetPos.y), groundLayer) && Vector2.Distance(playerPos, transform.position) > Vector2.Distance(playerPos, point + new Vector2(-offsetPos.x, offsetPos.y)))
            {
                return point + new Vector2(-offset * i, offset * i);
            }
            else if (!Physics2D.OverlapPoint(point + new Vector2(offsetPos.x, -offsetPos.y), obstacleLayer) && Physics2D.OverlapPoint(point + new Vector2(offsetPos.x, -offsetPos.y), groundLayer) && Vector2.Distance(playerPos, transform.position) > Vector2.Distance(playerPos, point + new Vector2(offsetPos.x, -offsetPos.y)))
            {
                return point + new Vector2(offset * i, -offset * i);
            }
        }
        return transform.position; // Default if we can't find a spot
    }

    public void MoveOnHitTaken()
    {
        if (animator.GetBool("IsBusy") || animator.GetBool("IsStunned") || animator.GetCurrentAnimatorStateInfo(0).IsTag("Immobile") || animator.GetCurrentAnimatorStateInfo(0).IsTag("Special"))
        {
            return;
        }
        else
        {
            animator.SetBool("IsBusy", true);
        }

        if (waitCoroutine != null) 
        { 
            isWaitingToRunAwayCoroutine = false; 
            StopCoroutine(waitCoroutine);
        }

        if (newPositionCoroutine != null) StopCoroutine(newPositionCoroutine);
        newPositionCoroutine = StartCoroutine(FindNewLocation());
    } 

    private IEnumerator FindNewLocation()
    {
        StopWaitingOnFlee();

        // Animator
        animator.SetBool("IsFleeing", true);
        animator.Play("Sprint");

        // Variables
        bool isLocationFound = false;
        float distanceToPlayer = 0f;
        float distanceToPos = 0f;

        while (!isLocationFound)
        {
            // Random position around the player
            Vector2 pos = Random.insideUnitSphere.normalized * Hermes_Attack.waveRange;
            distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
            distanceToPos = Vector2.Distance(pos, transform.position);

            // Position is valid if its further away than hermes is from the player
            if (distanceToPos >= distanceToPlayer)
            {
                // Call coroutine to move hermes
                if (movetoPositionCoroutine != null) StopCoroutine(movetoPositionCoroutine);
                movetoPositionCoroutine = StartCoroutine(MoveToNewPos(pos));
                isLocationFound = true;
            }

            yield return null;
        }
    }

    public void StopWaitingOnFlee()
    {
        // Stop coroutine to avoid strange behavior
        if (isWaitingToRunAwayCoroutine) isWaitingToRunAwayCoroutine = false;
        if (waitCoroutine != null) StopCoroutine(waitCoroutine);
    }

    private IEnumerator MoveToNewPos(Vector3 newPos)
    {
        // Variables
        Vector3 dir = (newPos - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, newPos);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        float start = Time.time;

        // While not at the position and collided with a wall
        while (distance > 1.5f && !Physics2D.Raycast(transform.position, dir, 0.6f, LayerMask.GetMask("Obstacles")))
        {
            // Updates direction and movement
            dir = (newPos - transform.position).normalized;
            rb.velocity = dir * newPositionSpeed;

            yield return new WaitForSeconds(0.02f); // Time between steps

            // Updates distance for the next while loop iteration
            distance = Vector2.Distance(transform.position, newPos); 
        }

        // Animator
        animator.SetBool("IsFleeing", false);
    }

    private void Flip()
    {
        // TODO Stop flipping while attacking
        if (!GetComponentInChildren<Animator>().GetBool("CanMove")) return;

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

    private bool IsTargetNotAttackable()
    {
        float dis = Mathf.Abs(Vector2.Distance(player.transform.position, transform.position));

        // Return true if terrain is in the way
        if (Physics2D.Raycast(transform.position, player.transform.position - transform.position, Hermes_Attack.waveRange, obstacleLayer))
        {
            return true;
        }

        return dis > Hermes_Attack.waveRange; // Return true if we are not in attackrange
    }

    private bool IsTargetTooClose()
    {
        float dis = Mathf.Abs(Vector2.Distance(player.transform.position, transform.position));

        // Return false if terrain is in the way
        if (Physics2D.Raycast(transform.position, player.transform.position - transform.position, Hermes_Attack.waveRange, obstacleLayer))
        {
            return false;
        }

        // Return true if we are in range and not in attackrange - -0.5f so enemy has a deadzone where it does not move
        if (dis < Hermes_Attack.waveRange - attackDeadZone) 
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator WaitBeforeMovingAway(float waitDuration)
    {
        float wait = waitDuration + Time.time;

        while (!isReadyToRunAway)
        {
            yield return new WaitForSeconds(0.1f);
            if (wait < Time.time)
            {
                isReadyToRunAway = true;
                isWaitingToRunAwayCoroutine = false;
            }
        }
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
