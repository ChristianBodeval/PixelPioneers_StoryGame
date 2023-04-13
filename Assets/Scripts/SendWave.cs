using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendWave : MonoBehaviour
{
    [SerializeField] private WaveObject[] waves;
    [SerializeField] private float timeBetweenWaves;
    private SpawnSystem spawnSystem;
    private Coroutine sendWavesCoroutine = null;
    [HideInInspector] public static bool canSendWaves = true;
    private int currentWave = 0;

    private void Start()
    {
        spawnSystem = GameObject.Find("EnemyFactory").GetComponent<SpawnSystem>();
    }

    // Temporary proof of concept
    public void StartWaves()
    {
        if (canSendWaves && sendWavesCoroutine == null)
        {
            canSendWaves = false;
            sendWavesCoroutine = StartCoroutine(SendWaves());
        }
    }

    private IEnumerator SendWaves()
    {
        while (currentWave < waves.Length)
        {
            spawnSystem.AddWave(waves[currentWave]); // Add wave to spawnsystem
            currentWave++;

            yield return null;
        }
    }
}
