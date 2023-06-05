using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSystem : MonoBehaviour
{
    [Header("Pool")]
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float spawnApartDistance;
    [HideInInspector] public List<WaveObject> wavesToSpawn = new List<WaveObject>();
    private static int remainingAlive = 0;
    private List<int> randomizingList = new List<int>();
    private Coroutine waveAliveCoroutine = null;
    [HideInInspector] public static bool isSpawning = false;
    private float postWaveWaitTime = 0f;
    private Coroutine awaitAddWaveCoroutine;
    private GameObject player;
    [HideInInspector] public static bool waveAlive = false;
    [HideInInspector] public static int totalWaves;
    [HideInInspector] public static int currentWave;

    private void Start()
    {
        ClearLists();

        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(SpawnWaves());
        StartCoroutine(CheckIfWaveIsDead()); // Function checks if wave is alive during play
    }

    public void AddWave(WaveObject wave)
    {
        player = GameObject.FindGameObjectWithTag("Player");

        // If list is being used while we are iterating on it we break the game - so we hold the info in a while loop until the list is no longer used
        if (isSpawning)
        {
            StartCoroutine(AwaitSpawnComplete(wave));
            return;
        }

        totalWaves++; // Increase max waves by 1
        wavesToSpawn.Add(wave);
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

    private IEnumerator SpawnWaves()
    {
        while (true)
        {
            // Suspend execution of function until wave is dead
            while (waveAlive || wavesToSpawn.Count < 1)
            {
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(0.3f); // Be sure all waves are added before isSpawning is set to true

            isSpawning = true; // Tell other functions to save themselves when we are spawning

            // Find the amount of each type of enemy in wave
            for (int i = 0; i < Enum.GetNames(typeof(WaveObject.EnemyType)).Length; i++) // For each enemytype
            {
                int amountToSpawn = 0;
                if (wavesToSpawn.Count > 0) amountToSpawn = wavesToSpawn[0].GetNumberOfType((WaveObject.EnemyType)i); // Amount of each type of enemies from the wave

                for (int j = 0; j < amountToSpawn; j++)
                {
                    randomizingList.Add(i);
                    waveAlive = true;
                }
            }

            // Spawn wave > remove it from waiting list > tell UI its the next wave
            if (wavesToSpawn.Count > 0)
            {
                postWaveWaitTime = wavesToSpawn[0].waitTimeAfterWave;
                yield return StartCoroutine(IterateListRandomly(wavesToSpawn[0].timeBetweenMobs)); // Spawn the enemies in a random order
                if (wavesToSpawn.Count > 0) wavesToSpawn.Remove(wavesToSpawn[0]); // Remove waves we have already used
            }

            isSpawning = false; // Its safe to edit the wavelist again
        }
    }

    private IEnumerator CheckIfWaveIsDead()
    {
        while (true)
        {
            while (remainingAlive > 0 || isSpawning)
            {
                yield return new WaitForSeconds(0.1f);
            }

            if (!waveAlive && remainingAlive < 1)
            {
                // Resets wave variables if the waves are all done
                if (currentWave > 0 && currentWave >= totalWaves && !isSpawning && remainingAlive < 1) // Last wave is dead and waitingDeathList is empty
                {
                    totalWaves = 0;
                    currentWave = 0;
                }
            }

            if (waveAlive && !isSpawning)
            {
                currentWave++; // UI knows it's the next wave - only true if wave was alive and now is dead
                if (remainingAlive < 1 && !isSpawning) waveAlive = false;
                yield return new WaitForSeconds(postWaveWaitTime);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void RemoveFromWaitDeathList(GameObject e)
    {
        remainingAlive--;
    }

    private IEnumerator IterateListRandomly(float timeBetweenMobs)
    {
        while (randomizingList.Count > 0)
        {
            int i = UnityEngine.Random.Range(0, randomizingList.Count);
            int enemyType = randomizingList[i];
            SpawnEnemy((WaveObject.EnemyType)enemyType); // Spawn enemy of type
            randomizingList.Remove(enemyType);
            yield return new WaitForSeconds(timeBetweenMobs); // Space mob spawning out - for performance and look reason, it looks better if mobs seem a bit more random in their timing
        }

        yield return new WaitForSeconds(0.2f);
    }

    private void SpawnEnemy(WaveObject.EnemyType type)
    {
        waveAlive = true;
        GameObject enemy = Pool.pool.DrawFromEnemyPool(type);
        enemy.transform.position = FindSpawnPoint();
        enemy.transform.rotation = Quaternion.Euler(-45f, 0f, 0f);
        remainingAlive++;
    }

    // Returns a point eligible for spawning an enemy outside of the screen
    private Vector2 FindSpawnPoint()
    {
        Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector3 point = Vector2.zero;

        int recursionCount = 0;
        const int maxRecursions = 20;

        while (recursionCount < maxRecursions)
        {
            var randomEdge = UnityEngine.Random.Range(0, 4);
            switch (randomEdge)
            {
                case 0:
                    point = Camera.main.ViewportToWorldPoint(new Vector3(0f, UnityEngine.Random.Range(0f, 1f), 0f)); // Left
                    break;

                case 1:
                    point = Camera.main.ViewportToWorldPoint(new Vector3(UnityEngine.Random.Range(0f, 1f), 1f, 0f)); // Top
                    break;

                case 2:
                    point = Camera.main.ViewportToWorldPoint(new Vector3(1f, UnityEngine.Random.Range(0f, 1f), 0f)); // Right
                    break;

                case 3:
                    point = Camera.main.ViewportToWorldPoint(new Vector3(UnityEngine.Random.Range(0f, 1f), 0f, 0f)); // Bottom
                    break;
            }

            point = new Vector3(point.x, point.y - point.z, 0f); // Compensate for 45 degree angle of cam
            point += (point - new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + 10f, 0f)).normalized * 1.5f; // Put point outside screenspace

            bool isGrounded = Physics2D.OverlapPoint(point, groundLayer);
            bool isObstructed = Physics2D.OverlapPoint(point, obstacleLayer);
            bool isEnemyInTheWay = Physics2D.CircleCast(point, spawnApartDistance / recursionCount, Vector2.up, spawnApartDistance / recursionCount, LayerMask.GetMask("Enemy"));

            if (isGrounded && !isObstructed && !isEnemyInTheWay) // Checks if we have ground and no obstacles in the way
            {
                return point;
            }

            recursionCount++;
        }

        return playerPosition; // Default case in case of recursion limit
    }

    public void ClearLists()
    {
        wavesToSpawn.Clear();
        randomizingList.Clear();
        remainingAlive = 0;
    }

    private void OnDisable()
    {
        ClearLists();
    }
}