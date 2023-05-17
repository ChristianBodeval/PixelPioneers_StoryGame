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
    private LayerMask obstacleLayer;
    private LayerMask groundLayer;
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
    [SerializeField] private float sprintAwayCD = 1f;
    [HideInInspector] public bool isSprintRDY = true;
    private bool isAwaiting = false;
    private Coroutine newPositionCoroutine;
    private Coroutine movetoPositionCoroutine;
    private Coroutine sprintCDCoroutine;

    [Header("Music")]
    public AudioClip bossTrack;

    [Header("SFX")]
    [Range(0, 1)] public float sfxVolume = 1f;
    [SerializeField] private AudioClip sprintSFX;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        animator.SetBool("CanMove", true);
        attackRange = GetComponent<Hermes_Attack>().attackRange;
        groundLayer = LayerMask.GetMask("Ground");
        obstacleLayer = LayerMask.GetMask("Obstacles");

        MusicManager.singleton.PlayMusic(bossTrack);

        StartCoroutine(UpdatePath()); // Updates pathfinding regularly
    }

    private void OnEnable()
    {
        if (animator != null) animator.SetBool("CanMove", true);
    }

    private void FixedUpdate()
    {       
        PathFollow();

        // Is Hermes in attack range and not too close
        if (!IsTargetNotAttackable() && !IsTargetTooClose())
        {
            animator.SetBool("InAttackRange", true);
        }
        else
        {
            animator.SetBool("InAttackRange", false);
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
        if (animator.GetBool("IsStunned") || animator.GetCurrentAnimatorStateInfo(0).IsTag("Immobile"))
        {
            rb.velocity = Vector2.zero;
            return;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Special"))
        {
            return;
        } 

        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Ready")) // Only Move and Idle states are tagged 'Ready'
        {
            float modifier = (Vector2.Distance(transform.position, player.transform.position) > Hermes_Attack.waveRange + 2f) ? 1.5f : 1f; // Move faster when far from the player
            modifier = (IsTargetNotAttackable() ? modifier : 0f); // Do not move if hermes is close enough
            rb.velocity = speed * dir * modifier; // Movement
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
        return player.transform.position; // Default
    }

    public void MoveOnHitTaken()
    {
        if (animator.GetBool("IsFleeing")) return;

        Debug.Log("State good: " + animator.GetCurrentAnimatorStateInfo(0).IsTag("Ready") + " CD rdy: " + isSprintRDY);
        if (!isSprintRDY || !animator.GetCurrentAnimatorStateInfo(0).IsTag("Ready"))
        {
            if (isSprintRDY && !isAwaiting) { StartCoroutine(WaitForNotBusy()); isAwaiting = true; }
            return;
        }

        animator.SetBool("IsFleeing", true);
        animator.Play("Sprint");

        // Start cooldown
        if (sprintCDCoroutine != null) StopCoroutine(sprintCDCoroutine);
        sprintCDCoroutine = StartCoroutine(SprintCooldown());

        if (newPositionCoroutine != null) StopCoroutine(newPositionCoroutine);
        newPositionCoroutine = StartCoroutine(FindNewLocation());
    }

    // If hermes was hit while doing something he will remember for a time and once he is ready to flee will do so
    private IEnumerator WaitForNotBusy()
    {
        float expirationTime = Time.time + 0.7f; 

        while (!isSprintRDY || !animator.GetCurrentAnimatorStateInfo(0).IsTag("Ready") || expirationTime > Time.time)
        {
            if (animator.GetBool("IsFleeing")) { isAwaiting = false; yield break; }
            yield return new WaitForSeconds(0.1f);
        }

        isAwaiting = false;
        MoveOnHitTaken();
    }

    private IEnumerator FindNewLocation()
    {
        // Variables
        bool isLocationFound = false;
        float distanceToPlayer = 0f;
        float distanceToPos = 0f;

        while (!isLocationFound)
        {
            // Random position around the player
            Vector3 pos = (Vector3)(Random.insideUnitCircle).normalized * (Hermes_Attack.waveRange - attackDeadZone) + player.transform.position;
            distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
            distanceToPos = Vector2.Distance(pos, transform.position);

            // Position is valid if its further away than hermes is from the player
            if ((distanceToPos >= distanceToPlayer) /* Position is further away than player*/
                && !Physics2D.Raycast(transform.position, pos - transform.position, Hermes_Attack.waveRange, obstacleLayer) /* Hermes can go in a straight line to the position */
                && !Physics2D.OverlapPoint(pos, obstacleLayer) && Physics2D.OverlapPoint(pos, groundLayer)) /* Position is walkable */
            {
                // Call coroutine to move hermes
                if (movetoPositionCoroutine != null) StopCoroutine(movetoPositionCoroutine);
                movetoPositionCoroutine = StartCoroutine(MoveToNewPos(pos));
                isLocationFound = true;
            }

            yield return null;
        }
    }

    private IEnumerator MoveToNewPos(Vector3 newPos)
    {
        // Variables
        Vector3 startPos = transform.position;
        Vector3 dir = (newPos - startPos).normalized;
        float distance = Vector2.Distance(startPos, newPos);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        float start = Time.time;

        SFXManager.singleton.PlaySound(sprintSFX, transform.position, sfxVolume, transform);

        // Move to new position
        while (distance > 1.5f)
        {
            // Move through obstacles if any are encountered
            float t = 0;
            while (Physics2D.CircleCast(transform.position, 0.6f, dir, 0.6f, obstacleLayer))
            {
                t += 0.1f;
                transform.position = Vector3.Lerp(startPos, newPos, t);
                yield return new WaitForSeconds(0.02f);
            }

            // Updates direction and movement
            dir = (newPos - transform.position).normalized;
            rb.velocity = dir * newPositionSpeed;

            yield return new WaitForSeconds(0.02f); // Time between steps

            // Updates distance for the next while loop iteration
            distance = Vector2.Distance(transform.position, newPos); 
        }

        // Stop
        rb.velocity = Vector2.zero;
        animator.SetBool("IsFleeing", false);
    }

    private IEnumerator SprintCooldown()
    {
        rb.velocity = Vector2.zero;
        isSprintRDY = false;

        while (animator.GetBool("IsFleeing"))
        {
            yield return null;
        }

        yield return new WaitForSeconds(sprintAwayCD);

        isSprintRDY = true;
    }

    private void Flip()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Ready")) return;

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

        return dis > Hermes_Attack.waveRange - 0.5f; // Return true if we are not in attackrange
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

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWayPoint = 2;
        }
    }
}
