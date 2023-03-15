using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSystem : MonoBehaviour
{
    [Header("Pool")]
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private float timeBetweenMobs = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private WaveObject tempWave;
    private List<(WaveObject.EnemyType, int)> enemies = new List<(WaveObject.EnemyType, int)>(); // List of tuples
    private Camera camera;
    private int recursions = 0;

    private void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        StartCoroutine(TempSpawnSystem());
    }

    private IEnumerator TempSpawnSystem()
    {
        while (true)
        {
            // Find the amount of each type of enemy in wave
            for (int i = 0; i < Enum.GetNames(typeof(WaveObject.EnemyType)).Length; i++) // For each enemytype
            {
                int amount = tempWave.GetNumberOfType((WaveObject.EnemyType)i);
                enemies.Add(((WaveObject.EnemyType)i, amount)); // Extra ( ) such that c# thinks its a tuple
            }

            // Spawn the appropriate amount of type from wave
            foreach ((WaveObject.EnemyType, int) e in enemies)
            {
                for (int i = 0; i < e.Item2; i++) // Less than amount in wave
                {
                    SpawnEnemy(e.Item1); // Spawn enemy of type
                    yield return new WaitForSeconds(timeBetweenMobs);
                }
            }

            enemies.Clear();
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    private void SpawnEnemy(WaveObject.EnemyType type)
    {
        GameObject enemy = Pool.pool.DrawFromPool(type);
        enemy.transform.position = FindSpawnPoint();
    }

    //returns a point eligible for spawning an enemy outside of the screen
    private Vector2 FindSpawnPoint()
    {
        float width = camera.pixelWidth / 80f + 0.5f; //half the width of the screen
        float height = camera.pixelHeight / 80f + 0.5f; //half the height of the screen

        Vector2 point = Vector2.zero;

        // Random edge of the screen
        switch (UnityEngine.Random.Range(1, 5))
        {
            case 1:
                point = new Vector2(-width, UnityEngine.Random.Range(-height, height)); // Left
                break;
            case 2:
                point = new Vector2(UnityEngine.Random.Range(-width, width), height); // Top
                break;
            case 3:
                point = new Vector2(width, UnityEngine.Random.Range(-height, height)); // Right
                break;
            case 4:
                point = new Vector2(UnityEngine.Random.Range(-width, width), -height); // Bottom
                break;
            default:
                return new Vector2(UnityEngine.Random.Range(-width, width), UnityEngine.Random.Range(-height, height));
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

