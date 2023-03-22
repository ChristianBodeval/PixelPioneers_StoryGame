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
    private List<(WaveObject.EnemyType, int)> enemiesToSpawn = new List<(WaveObject.EnemyType, int)>(); // List of tuples
    private int recursions = 0;
    private bool isSpawning = false;

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    public void AddWave(WaveObject wave)
    {
        // If list is being used while we are iterating on it we break the game - so we hold the info in a while loop until the list is no longer used
        if (isSpawning)
        {
            StartCoroutine(AwaitSpawnComplete(wave));
            return;
        }

        // Find the amount of each type of enemy in wave
        for (int i = 0; i < Enum.GetNames(typeof(WaveObject.EnemyType)).Length; i++) // For each enemytype
        {
            int amount = wave.GetNumberOfType((WaveObject.EnemyType)i);
            enemiesToSpawn.Add(((WaveObject.EnemyType)i, amount)); // Extra ( ) such that c# thinks its a tuple
        }
    }

    // Waits for spawning to be complete
    private IEnumerator AwaitSpawnComplete(WaveObject wave)
    {
        while (isSpawning)
        {
            yield return new WaitForSeconds(0.01f);
        }

        AddWave(wave);
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            isSpawning = true; // Tell other functions to save themselves when we are spawning
            lock (enemiesToSpawn)
            {
                AddWave(tempWave);

                // Spawn the appropriate amount of type from wave
                foreach ((WaveObject.EnemyType, int) e in enemiesToSpawn)
                {
                    for (int i = 0; i < e.Item2; i++) // Less than amount in wave
                    {
                        SpawnEnemy(e.Item1); // Spawn enemy of type
                        yield return new WaitForSeconds(timeBetweenMobs);
                    }
                }

                enemiesToSpawn.Clear();
            }
            isSpawning = false;
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    private void SpawnEnemy(WaveObject.EnemyType type)
    {
        GameObject enemy = Pool.pool.DrawFromPool(type);
        enemy.transform.position = FindSpawnPoint();
        enemy.transform.rotation = Quaternion.Euler(-45f, 0f, 0f);
    }

    //returns a point eligible for spawning an enemy outside of the screen
    private Vector2 FindSpawnPoint()
    {
        float width = Camera.main.pixelWidth / 80f + 0.5f; //half the width of the screen
        float height = Camera.main.pixelHeight / 80f + 0.5f; //half the height of the screen

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

