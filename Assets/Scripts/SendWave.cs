using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class SendWave : MonoBehaviour
{
    [Header("Music")]
    [Range(0, 1)] public float musicVolume = 0.5f;
    [SerializeField] protected AudioClip combatTrack;
    private SpawnSystem spawnSystem;
    private Coroutine sendWavesCoroutine = null;
    private int currentWave;
    private bool isSent = false;
    
    public UnityEvent caveStartedEvent;

    private void Start()
    {
        spawnSystem = GameObject.Find("GameManager").GetComponent<SpawnSystem>();
        isSent = false;
    }

    public void SendWaves()
    {
        caveStartedEvent.Invoke();
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

        foreach (var wave in GetComponentsInChildren<WaveObject>())
        {
            spawnSystem.AddWave(wave); // Add wave to spawnsystem
            currentWave++;
        }
    }
}
