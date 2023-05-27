using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class CaveManager : MonoBehaviour
{
    private SendWave sendWave;
    private bool isTriggered;
    
    public GameObject player;
    public CaveEntrance caveEntrance;
    
    
    public Transform playerSpawnTransform;
    
    private void Awake()
    {
        player.GetComponent<PlayerHealth>().playerDeathEvent.AddListener(RestartCave);
        sendWave = FindObjectOfType<SendWave>();
        sendWave.caveClearedEvent.AddListener(EndCave);
    }
    
    private void Start()
    {
        player.transform.position = playerSpawnTransform.position;
    }
    
    private void StartCave()
    {
        
        caveEntrance.GetComponent<CircleCollider2D>().enabled = false;
        sendWave.SendWaves();
    }
    
    private void RestartCave()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void EndCave()
    {
        caveEntrance.GetComponent<CircleCollider2D>().enabled = true;
        ProgressManager.Instance.SetNextCaveActive();
    }
    
    
    void OnTriggerEnter2D(Collider2D other)
    {
        //Call only once
        if(isTriggered) return;
        
        if (other.CompareTag("Player"))
        {
            isTriggered = true;
            StartCave();
            Debug.Log("Sending waves");
        }
    }
}
