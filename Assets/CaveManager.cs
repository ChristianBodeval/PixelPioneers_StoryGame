using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class CaveManager : MonoBehaviour
{
    public SendWave sendWave;
    
    public GameObject player;
    public CaveEntrance caveEntrance;
    public Transform playerSpawnTransform;
    
    public static CaveManager instance;
    
    private void Awake()
    {
        sendWave = FindObjectOfType<SendWave>();
        caveEntrance = FindObjectOfType<CaveEntrance>();
        player = GameObject.FindWithTag("Player");
        sendWave.caveStartedEvent.AddListener(StartCave);
        sendWave.caveClearedEvent.AddListener(EndCave);

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

    public void EndCave()
    {
        caveEntrance.SetAccessibility(true);
        ProgressManager.instance.SetNextCaveActive();
    }
}
