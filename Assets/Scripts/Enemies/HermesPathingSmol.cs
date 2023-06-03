using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class HermesPathingSmol : MonoBehaviour
{
    [Header("General for Pathfinding")]
    [SerializeField] private float speed = 3f;
    private GameObject player;
    private Rigidbody2D rb;

    [Header("A*")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float updateInterval = 0.1f;
    [SerializeField] private float nextWayPointDistance = 2f;
    private Path path;
    private int currentWayPoint = 0;
    private Seeker seeker;
    private Vector2 targetPos;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        targetPos = GameObject.Find("SpawnPointOutsideEntrance").transform.position;

        player.GetComponent<PlayerAction>().StopMove();
        player.GetComponent<PlayerHealth>().HealDamage(10f);

        StartCoroutine(SetBehavior());
    }

    private IEnumerator SetBehavior()
    {
        yield return new WaitForSeconds(1f);

        switch (ExtractNumberFromName(SceneManager.GetActiveScene().name))
        {
            case 1: case 2: case 3:
                InvokeRepeating("UpdatePath", 0f, updateInterval); // Updates pathfinding regularly
                break;

            case 4:
                // Do something for cave 4 hermes, post fight
                break;
            default:
                break;
        }
    }

    private int ExtractNumberFromName(string name)
    {
        Match match = Regex.Match(name, @"[1-9]");
        if (match.Success)
        {
            string numberString = match.Value;
            int number;
            if (int.TryParse(numberString, out number))
            {
                return number;
            }
        }
        Debug.LogWarning("Failed to extract number from name: " + name);
        return -1; // Return a default value or handle the error as needed
    }

    private void FixedUpdate()
    {       
        PathFollow();

        if (Vector2.Distance(transform.position, targetPos) < 1.5f) { player.GetComponent<PlayerAction>().StartMove(); Destroy(gameObject); }
    }

    private void Move(Vector2 dir)
    {
        rb.velocity = speed * dir; // Movement
    }

    private void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(transform.position, targetPos, OnPathComplete);
        }
    }

    private void PathFollow()
    {
        Flip(); // Flips sprite

        // Guard clause
        if (path == null || currentWayPoint >= path.vectorPath.Count || false ) { return; } // Is not there yet and has a path && is not Digging

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
        Vector2 dir = (rb.velocity).normalized; 
        if (dir.x > 0.24f) // Right
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (dir.x < -0.24f) // Left
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
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
