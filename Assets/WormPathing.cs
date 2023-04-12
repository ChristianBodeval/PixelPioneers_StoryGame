using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Unity.VisualScripting;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class WormPathing : MonoBehaviour
{
    [Header("General for Pathfinding")]
    [SerializeField] private float speed = 3f;
    private float attackRange = 0.5f;
    private GameObject player;
    private Rigidbody2D rb;
    private Animator animator;
    private Health healthScript;
    private SpriteRenderer spriteRenderer;

    [Header("A*")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float updateInterval = 0.1f;
    [SerializeField] private float nextWayPointDistance = 2f;

    private Path path;
    private int currentWayPoint = 0;
    private Seeker seeker;
    private bool canMove;

    [Header("Custom Behavior")]
    [SerializeField] private bool isFollowing = true;
    [SerializeField] private float digLength;
    [SerializeField] private float digSpeed;

    [SerializeField] private CircleCollider2D enemyCollider;
    private Vector3 digDirection;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private ParticleSystem diggingEffect;



    private void Awake()
    {
        player = GameObject.FindWithTag("Player");


        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        //attackRange = GetComponent<Melee_Attack>().attackRange;
        healthScript = GetComponent<Health>();
    }

    private void Start()
    {
        InvokeRepeating("UpdatePath", 0f, updateInterval); // Updates pathfinding regularly
        canMove = true;
        healthScript.DamageTakenEvent.AddListener(DigMovement); //Listens to when player takes damage, so it can perfrom a Dig movement
    }

    private void FixedUpdate()
    {
        //A* pathing
        if (animator.GetBool("IsDigging")) {
            return;
        }

        if (IsTargetNotAttackable() && !animator.GetBool("IsStunned"))
        {
            PathFollow();
        }

        else if (animator.GetBool("CanMove") || animator.GetBool("IsStunned"))
        {
            Debug.Log("Called velodown");
            rb.velocity = new Vector3(0f, 0f, 0f);
        }

    }



    private void Move()
    {
        Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - rb.position).normalized;


        if (!animator.GetBool("CanMove") || animator.GetBool("IsStunned")) { rb.velocity = Vector2.zero; return; } // Guard clause - can we move

        rb.velocity = speed * direction; // Movement
    }

    private void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(transform.position, player.transform.position, OnPathComplete);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(digDirection, 1.5f);
    }


    private IEnumerator MyCoroutine()
    {
        canMove = false;
        yield return new WaitForSeconds(0.5f);
        float originalSpeed = speed;
        speed = digSpeed;
        Collider2D opstacles = Physics2D.OverlapCircle(digDirection, 0.1f, obstacleLayer);

        digDirection = player.transform.position + digLength * (player.transform.position - transform.position).normalized;


        Debug.Log("Disabling collider");


        rb.bodyType = RigidbodyType2D.Kinematic;
        healthScript.enabled = false;


        spriteRenderer.enabled = false;

        diggingEffect.Play();

        animator.SetBool("IsDigging", true);

        //TODO Dosen't work for pit layer, should pit  be in obstacle layer?
        while (Physics2D.OverlapCircle(digDirection, 0.1f, groundLayer) == null || Physics2D.OverlapCircle(digDirection, 0.1f, obstacleLayer)) //If hit an obstacle or hit no collider (out of map)
        {



            digDirection += (player.transform.position - digDirection).normalized * 1f;
            yield return null;
        }

        //When in obstacle layer
        /* 
        while (Physics2D.OverlapCircle(digDirection, 0.1f, obstacleLayer)) //If hit an obstacle or hit no collider (out of map)
        {
            Debug.Log("Hit an obstacle");
            digDirection += (player.transform.position - digDirection).normalized * 1f;
            yield return null;
        }
        */





        //Move towards digging point
        while (Vector3.Distance(transform.position, digDirection) > 0.1f ) //If not close to the digPosition or hit an obstacle
        {

            rb.velocity = (digDirection - transform.position).normalized*digSpeed*Time.fixedDeltaTime;
            Debug.Log("Moving");

            Debug.Log("rb.velocity" + rb.velocity);
            /*
            // Move our position a step closer to the target.
            var step = speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, digDirection, step);

            Debug.Log("Current position");
            */


            yield return null;

        }

        //Ending dig
        spriteRenderer.enabled = true;
        diggingEffect.Stop();
        animator.SetBool("IsDigging", false);
        Debug.Log("Stopping velocity");
        rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(0.5f);


        Debug.Log("Enabling collider");
        rb.bodyType = RigidbodyType2D.Dynamic;


        healthScript.enabled = true;
        enemyCollider.enabled = true;
        speed = originalSpeed;
        canMove = true;
        coroutine = null;
        yield return null;
    }


    private void DigMovement()
    {
        Debug.Log("DigMovement called");
        if(coroutine == null)
        {
            coroutine = MyCoroutine();
            StartCoroutine(coroutine);

        }
    }


    IEnumerator coroutine;
    private void PathFollow()
    {
        Flip(); // Flips sprite

        // Guard clause
        if (path == null || currentWayPoint >= path.vectorPath.Count) { return; } // Is not there yet and has a path

        if(animator.GetBool("IsDigging"))
        {
            return;
        }

        if(canMove)
        {
            Move();
        }

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

    private bool IsTargetNotAttackable()
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
