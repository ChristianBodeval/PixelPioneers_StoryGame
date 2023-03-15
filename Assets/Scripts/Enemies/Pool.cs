using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pool : MonoBehaviour
{
    private List<GameObject> storageEnemies = new List<GameObject>();
    private List<GameObject> inUseEnemies = new List<GameObject>();
    [Header("Spawner")]
    [SerializeField] private float spawnFrequency = 5f;
    [SerializeField] private GameObject prefab;
    private Camera camera;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask obstacleLayer;
    private int recursions = 0;

    public static Pool pool { get; private set; }

    private void Awake()
    {
        //if there is an instance, and it's not me, delete myself
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
        InvokeRepeating("SpawnEnemy", 0f, spawnFrequency);
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

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

    public void ReturnToPool(GameObject enemy)
    {
        enemy.SetActive(false);

        if (inUseEnemies.Contains(enemy))
        {
            inUseEnemies.Remove(enemy);
        }

        storageEnemies.Add(enemy);
    }

    private void SpawnEnemy()
    {
        GameObject enemy = DrawFromPool();
        enemy.transform.position = FindSpawnPoint();
    }

    //returns a point eligible for spawning an enemy outside of the screen
    private Vector2 FindSpawnPoint()
    {
        float width = camera.pixelWidth / 68f + 0.5f; //half the width of the screen
        float height = camera.pixelHeight / 68f + 0.5f; //half the height of the screen

        Vector2 point = Vector2.zero;

        // Random edge of the screen
        switch (Random.Range(1,5))
        {
            case 1:
                point = new Vector2(-width, Random.Range(-height, height)); // Left
                break;
            case 2:
                point = new Vector2(Random.Range(-width, width), height); // Top
                break;
            case 3:
                point = new Vector2(width, Random.Range(-height, height)); // Right
                break;
            case 4:
                point = new Vector2(Random.Range(-width, width), -height); // Bottom
                break;
            default:
                return new Vector2(Random.Range(-width, width), Random.Range(-height, height));
        }

        bool grounded = Physics2D.OverlapPoint((Vector2)GameObject.FindGameObjectWithTag("Player").transform.position + point, groundLayer);
        bool unobstructed = !Physics2D.OverlapPoint((Vector2)GameObject.FindGameObjectWithTag("Player").transform.position + point, obstacleLayer);



        if (grounded && unobstructed)  // Checks if we have ground and no obstacles in the way
        {
            recursions = 0; // Reset recussion count
            return point + (Vector2)GameObject.FindGameObjectWithTag("Player").transform.position; 
        }
        else if (recursions < 50 && (!grounded || !unobstructed)) // Call method again if we can't spawn
        {
            recursions++; // Keep track of how many recursions we do, to avoid stackoverflow
            return FindSpawnPoint();
        }
        else // Default case in case of recussion limit
        {
            recursions = 0;
            return (Vector2)GameObject.FindGameObjectWithTag("Player").transform.position;
        }
    }
}
