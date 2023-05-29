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
    
    //Make it a singleton
    public static CaveManager instance;
    
    private void Awake()
    {
        
        sendWave = FindObjectOfType<SendWave>();
        caveEntrance = FindObjectOfType<CaveEntrance>();
        player = GameObject.FindWithTag("Player");
        playerSpawnTransform = player.transform;
        sendWave.caveStartedEvent.AddListener(StartCave);
        
        //player.GetComponent<PlayerHealth>().playerDeathEvent.AddListener(RestartCave);
        
        
        if (instance != null && instance != this)
        {
            Debug.Log("Destroying CaveManager");
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }
    
    
    
    private void Start()
    {
        player.transform.position = playerSpawnTransform.position;
    }
    
    public void StartCave()
    {
        caveEntrance.SetAccessibility(false);
    }
    
    
    
    /*
    private void RestartCave()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }*/

    public void EndCave()
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
