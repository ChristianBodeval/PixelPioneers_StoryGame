using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CaveStateHandler : MonoBehaviour
{
    //Unused
    /*
    IEnumerator coroutine;
    private PlayerHealth playerHealth;
    private SendWave sendWave;
    [SerializeField] private CaveEntrance caveEntrance;
    
    
    private void Awake()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerHealth.playerDeathEvent.AddListener(RestartCave);
        sendWave = FindObjectOfType<SendWave>();
        sendWave.caveClearedEvent.AddListener(EndCave);
    }
    
    private void Start()
    {
        StartCave();
    }

    private void StartCave()
    {
        caveEntrance.GetComponent<CircleCollider2D>().enabled = false;
    }
    
    private void RestartCave()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void EndCave()
    {
        caveEntrance.GetComponent<CircleCollider2D>().enabled = true;
        ProgressManager.Instance.SetNextCaveActive();
    }*/
}

