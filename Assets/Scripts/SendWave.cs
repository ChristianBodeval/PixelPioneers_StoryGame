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
    private void OnTriggerEnter2D (Collider2D col)
    {
        if (col.gameObject.CompareTag("Player") && !wavesSent && sendWavesCoroutine == null)
        {
            wavesSent = true;
            sendWavesCoroutine = StartCoroutine(SendWaves());
        }
    }

    private IEnumerator SendWaves()
    {
        while (currentWave < waves.Length)
        {
            if (wavesSent)
            {
                spawnSystem.AddWave(waves[currentWave]); // Add wave to spawnsystem
                Debug.Log(currentWave);
                currentWave++;
            }

            yield return null;
        }
    }
}
