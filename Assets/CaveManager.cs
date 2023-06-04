using UnityEngine;
using UnityEngine.SceneManagement;

public class CaveManager : MonoBehaviour
{
    public SendWave sendWave;
    
    public GameObject player;
    public CaveEntrance caveEntrance;
    public Transform playerSpawnTransform;
    
    public static CaveManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.Log("Destroying CaveManager");
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        playerSpawnTransform = GameObject.Find("SpawnPointOutsideEntrance").transform;
        sendWave = FindObjectOfType<SendWave>();
        caveEntrance = FindObjectOfType<CaveEntrance>();
        player = GameObject.FindWithTag("Player");
        if (sendWave != null) sendWave.caveStartedEvent.AddListener(StartCave);
    }
    

    public void StartCave()
    {
        //TODO Setup with caves
        
        //Check if currentScene name is beginning with "Cave_"
        if(SceneManager.GetActiveScene().name.StartsWith("Cave_"))
        {
            //TODO Setup with caves
            //Set cave entrance to not accessible
            caveEntrance.SetAccessibility(false);
        }
        caveEntrance.SetAccessibility(false);
    }

    public void EndCave()
    {
        caveEntrance.SetAccessibility(true);
        ProgressManager.instance.CaveHasBeenCleared();
        GameObject.Find("GameManager").GetComponent<SpawnSystem>().ClearLists();
    }
}
