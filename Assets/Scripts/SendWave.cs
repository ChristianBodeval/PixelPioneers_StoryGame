using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendWave : MonoBehaviour
{
    [SerializeField] private WaveObject[] waves;
    private SpawnSystem spawnSystem;
    private Coroutine sendWavesCoroutine = null;
    private int currentWave;

    private void Start()
    {
        spawnSystem = GameObject.Find("GameManager").GetComponent<SpawnSystem>();
    }
    
    

    public void StartWaves()
    {
        if (!SpawnSystem.waveAlive)
        {
            currentWave = 0;
            if (sendWavesCoroutine != null) StopCoroutine(sendWavesCoroutine);
            sendWavesCoroutine = StartCoroutine(SendWaves());
        }
    }

    private IEnumerator SendWaves()
    {
        yield return null;
        while (currentWave < waves.Length)
        {
            spawnSystem.AddWave(waves[currentWave]); // Add wave to spawnsystem
            currentWave++;
        }
    }
}
