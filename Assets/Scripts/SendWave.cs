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
    private bool wavesSent = false;
    private int currentWave = 0;

    private void Start()
    {
        spawnSystem = GameObject.Find("EnemyFactory").GetComponent<SpawnSystem>();
    }

    // Temporary proof of concept
    public void StartWaves()
    {
        if (!wavesSent && sendWavesCoroutine == null)
        {
            StartCoroutine(ActivationCD());
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

    private IEnumerator ActivationCD()
    {
        wavesSent = true;

        yield return new WaitForSeconds(10f);

        wavesSent = false;
    }
}
