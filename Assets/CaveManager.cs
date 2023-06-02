using UnityEngine;
using UnityEngine.SceneManagement;

public class CaveManager : MonoBehaviour
{
    public SendWave sendWave;
    
    public GameObject player;
    public CaveEntrance caveEntrance;
    
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
        player.transform.position = GameObject.Find("PlayerSpawnPoint").transform.position;
    }

    public void StartCave()
    {
        //TODO Setup with caves
        if (SceneManager.GetActiveScene().name == "Village" || SceneManager.GetActiveScene().name != "VillageWithTL")
        {
            return;
        }
            caveEntrance.SetAccessibility(false);
    }

    public void EndCave()
    {
        caveEntrance.SetAccessibility(true);
        ProgressManager.instance.SetNextCaveActive();
    }
}
