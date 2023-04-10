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
    private List<WaveObject> wavesToSpawn = new List<WaveObject>();
    private List<GameObject> waitingDeathList = new List<GameObject>();
    private Coroutine waveAliveCoroutine = null;
    private int recursions = 0;
    private bool isSpawning = false;
    private bool waveAlive = false;
    private bool isWaitingForWaveToDie = false;
    [HideInInspector] public static int totalWaves;
    [HideInInspector] public static int currentWave;

    private void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    public void AddWave(WaveObject wave)
    {
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
            // Suspend execution of function until wave is dead if enabled on previous wave
            while (isWaitingForWaveToDie && waveAlive)
            {
                yield return new WaitForSeconds(0.1f);
            }

            isSpawning = true; // Tell other functions to save themselves when we are spawning
            if (wavesToSpawn.Count > 0) isWaitingForWaveToDie = wavesToSpawn[0].waitForWaveToBeDead; // Are we waiting on wave being dead before spawning the next one

            // Find the amount of each type of enemy in wave
            for (int i = 0; i < Enum.GetNames(typeof(WaveObject.EnemyType)).Length; i++) // For each enemytype
            {
                int amountToSpawn = 0;
                if (wavesToSpawn.Count > 0) amountToSpawn = wavesToSpawn[0].GetNumberOfType((WaveObject.EnemyType)i); // Amount of each type of enemies from the wave

                for (int j = 0; j < amountToSpawn; j++)
                {
                    SpawnEnemy((WaveObject.EnemyType)i); // Spawn enemy of type
                    waveAlive = true;
                    yield return new WaitForSeconds(wavesToSpawn[0].timeBetweenMobs); // Space mob spawning out - for performance and look reason, it looks better if mobs seem a bit more random in their timing
                }
            }

            currentWave++;

            if (isWaitingForWaveToDie && waveAliveCoroutine == null) StartCoroutine(CheckIfWaveIsDead()); // Function checks if wave is alive during play
            if (wavesToSpawn.Count > 0) wavesToSpawn.Remove(wavesToSpawn[0]); // Instead of iterating through the list we remove waves we have already used

            isSpawning = false;
            float waitDuration = 0.1f; // Default value
            if (wavesToSpawn.Count > 0) waitDuration = wavesToSpawn[0].waitAmountUntilNextWave; // Wait for duration specified by the wave object - edit the duration in the object
            yield return new WaitForSeconds(waitDuration);

            // Resets wave variables if the waves are all done
            if (currentWave == totalWaves && !isWaitingForWaveToDie && !waveAlive)
            {
                totalWaves = 0;
                currentWave = 0;
            }
        }
    }

    private IEnumerator CheckIfWaveIsDead()
    {
        while (waitingDeathList.Count > 0)
        {
            yield return new WaitForSeconds(0.1f);
        }

        waveAlive = false;
    }

    public void RemoveFromWaitDeathList(GameObject e)
    {
        if (waitingDeathList.Contains(e)) waitingDeathList.Remove(e);
    }

    private void SpawnEnemy(WaveObject.EnemyType type)
    {
        GameObject enemy = Pool.pool.DrawFromEnemyPool(type);
        enemy.transform.position = FindSpawnPoint();
        enemy.transform.rotation = Quaternion.Euler(-45f, 0f, 0f);
        if (isWaitingForWaveToDie) waitingDeathList.Add(enemy);
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

}