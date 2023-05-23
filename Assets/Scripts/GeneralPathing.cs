using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Unity.VisualScripting;
using UnityEngine.Events;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.UI;

public class GeneralPathing : MonoBehaviour
{
    [Header("General for Pathfinding")]
    private GameObject player;
    private Rigidbody2D rb;
    
    [SerializeField] private float updateInterval = 0.1f;
    [SerializeField] private float nextWayPointDistance = 2f;

    private Path path;
    private int currentWayPoint = 0;
    private Seeker seeker;

    public float speed;
    public Vector3 direction;

    public UnityEvent OnDirectionReached;
    
    [SerializeField] private LayerMask obstacleLayer;
    private Slider hpBar;

    public void SetDirection(Vector3 dir)
    {
        UpdatePath();
        direction = dir;
    }

    public void StopMoving()
    {
        SetDirection(transform.position);
        rb.velocity = Vector3.zero;
    }
    
    public void SetSpeed(float spd)
    {
        speed = spd;
    }
    
    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
    }
    
    
    
    private void Start()
    {
        hpBar = GetComponentInChildren<Slider>();
        hpBar.maxValue = GetComponent<Health>().maxHealth;
        hpBar.value = GetComponent<Health>().currentHealth;
        seeker = GetComponent<Seeker>();
        //InvokeRepeating("UpdatePath", 0f, updateInterval); // Updates pathfinding regularly
    }
    
    //StateUpdate when changes inspector

    private void FixedUpdate()
    {
        PathFollow();
        
        //When pressing space the the direction to 300,0,0
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetDirection(new Vector3(300, 0, 0));
        }
    }

    private void Move()
    {
        //If position is almost the same is the last point in path.vectorPath, then stop moving
        if (Vector2.Distance(rb.position, path.vectorPath[path.vectorPath.Count - 1]) < 0.1f)
        {
            OnDirectionReached.Invoke();
            rb.velocity = Vector2.zero;
            return;
        }
        
        Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - rb.position).normalized;

        float modifier = Vector2.Distance(player.transform.position, transform.position) > 14f ? ((player.transform.position - transform.position).magnitude / 3f) + 1f : 1f;

        rb.velocity = speed * direction * modifier; // Movement
    }

    private void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(transform.position, direction, OnPathComplete);
        }
    }
    
    
    
    

    private void PathFollow()
    {
        if (path == null || currentWayPoint >= path.vectorPath.Count) { return; } // Is not there yet and has a path
        FlipSprite();
        Move();

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);
        if (distance < nextWayPointDistance)
        {
            currentWayPoint++;
        }
    }

    private void FlipSprite()
    {
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
    
    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWayPoint = 1;
        }
    }

    
    public bool IsObstacleBetweenPlayer()
    {
        float distance = Vector2.Distance(transform.position, player.transform.position);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, player.transform.position - transform.position, distance, obstacleLayer);
        if (hit.collider != null)
        {
            return true;
            // If the ray hits a collider in the specified layer, do something
        }
        return false;
    }

    public float GetDistanceToPlayer()
    {
        //Get distance to player
        return Vector2.Distance(transform.position, player.transform.position);
    }
}
