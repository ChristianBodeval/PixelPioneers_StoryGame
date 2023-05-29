using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class CaveManager : MonoBehaviour
{
    public SendWave sendWave;
    private bool isTriggered;
    
    public GameObject player;
    public CaveEntrance caveEntrance;
    
    
    public Transform playerSpawnTransform;
    
    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
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
        caveEntrance.SetAccessibility(false);
        sendWave.SendWaves();
    }
    
    
    
    
    private void RestartCave()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void EndCave()
    {
        caveEntrance.SetAccessibility(true);
        ProgressManager.instance.SetNextCaveActive();
    }
    
    //Endcave when pressing space
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EndCave();
        }
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
