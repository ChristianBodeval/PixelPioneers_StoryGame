using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class SendWave : MonoBehaviour
{
    [Header("Music")]
    [Range(0, 1)] public float musicVolume = 0.5f;
    [SerializeField] protected AudioClip combatTrack;

    [SerializeField] private WaveObject[] waves;
    private SpawnSystem spawnSystem;
    private Coroutine sendWavesCoroutine = null;
    private int currentWave;
    private bool isSent = false;
    
    [FormerlySerializedAs("cavesClearEvent")] public UnityEvent caveClearedEvent;

    
    public UnityEvent caveStartedEvent;
    
    
    
    private void Start()
    {
        spawnSystem = GameObject.Find("GameManager").GetComponent<SpawnSystem>();
        isSent = false;
    }

    public void SendWaves()
    {
        CaveManager.instance.StartCave();
        if (!SpawnSystem.waveAlive && !isSent)
        {
            if (combatTrack != null) MusicManager.singleton.PlayMusic(combatTrack, musicVolume);
            isSent = true;
            currentWave = 0;
            if (sendWavesCoroutine != null) StopCoroutine(sendWavesCoroutine);
            sendWavesCoroutine = StartCoroutine(SendWavesCoroutine());
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player")) SendWaves();
    }

    private IEnumerator SendWavesCoroutine()
    {
        yield return null;
        
        if(currentWave == waves.Length) caveClearedEvent.Invoke();
        
        while (currentWave < waves.Length)
        {
            spawnSystem.AddWave(waves[currentWave]); // Add wave to spawnsystem
            currentWave++;
        }
    }
}
