using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pool : MonoBehaviour
{
    // Enemy pool
    private List<GameObject> storageEnemies = new List<GameObject>();
    private List<GameObject> inUseEnemies = new List<GameObject>();

    [Header("Spawner")]
    [SerializeField] private float spawnFrequency = 5f;
    [SerializeField] private GameObject prefab;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private LayerMask groundLayer;
    private Camera camera; // To spawn enemies outside the player's view
    private int recurses = 0; // Restricts the amount of recursive calls to avoid memory leak

    public static Pool pool { get; private set; }

    private void Awake()
    {
        // Singleton pattern
        if (pool != null && pool != this)
        {
            Destroy(this);
        }
        else
        {
            pool = this;
        }
    }

    private void Start()
    {
        InvokeRepeating("SpawnEnemy", 0f, spawnFrequency); // Sets interval for spawning enemies
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Generates an enemy
    public GameObject DrawFromPool()
    {
        if (storageEnemies.Count > 0)
        {
            GameObject enemy = storageEnemies[0];
            storageEnemies.Remove(enemy);
            inUseEnemies.Add(enemy);
            enemy.SetActive(true);
            return enemy;
        }
        else
        {
            //**spawn outside screen
            GameObject enemy = Instantiate(prefab, new Vector3(0f,0f,0f), Quaternion.identity); //new enemy
            inUseEnemies.Add(enemy);
            enemy.SetActive(true);
            return enemy;
        }
    }

    // Removes an enemy and stores it
    public void ReturnToPool(GameObject enemy)
    {
        enemy.SetActive(false);

        if (inUseEnemies.Contains(enemy))
        {
            inUseEnemies.Remove(enemy);
        }

        storageEnemies.Add(enemy);
    }

    // Positions an enemy - Activated by the InvokeRepeating in Start()
    private void SpawnEnemy()
    {
        GameObject enemy = DrawFromPool();
        enemy.transform.position = FindSpawnPoint();
    }

    // Returns a point eligible for spawning an enemy outside of the screen
    private Vector2 FindSpawnPoint()
    {
        float width = camera.pixelWidth / 96f + 0.5f; // Half the width of the screen
        float height = camera.pixelHeight / 96f + 0.5f; // Half the height of the screen

        Vector2 point = new Vector2();

        // Random edge of the screen
        switch (Random.Range(1,5))
        {
            case 1:
                point = new Vector2(-width - 0.5f, Random.Range(0, height)); // Left
                break;
            case 2:
                point = new Vector2(Random.Range(0, width), height + 0.5f); // Top
                break;
            case 3:
                point = new Vector2(width + 0.5f, Random.Range(0, height)); // Right
                break;
            case 4:
                point = new Vector2(Random.Range(0, width), -height -0.5f); // Bottom
                break;
            default:
                return new Vector2(Random.Range(-width, width), Random.Range(-height, height));
        }

        point += (Vector2)GameObject.FindGameObjectWithTag("Player").transform.position;

        // Checks if we have ground and no obstacles in the way
        if (Physics2D.CircleCast(point, 0.5f, Vector2.up, 0.5f, groundLayer) && !Physics2D.CircleCast(point, 0.5f, Vector2.up, 0.5f, obstacleLayer)) // Has ground and not in a wall
        {
            recurses = 0; // Resets the recusion count
            return point;
        }
        else if (recurses < 25)
        {
            recurses++; // Counts the amount of times we recursively call this function, we limit the amount to avoid memory leaks
            return FindSpawnPoint(); // Find new eligible spawn location
        }
        else
        {
            recurses = 0; // Resets the recusion count
            return (Vector2)GameObject.FindGameObjectWithTag("Player").transform.position;
        }
    }
}
