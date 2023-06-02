using UnityEngine;
using UnityEngine.SceneManagement;

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

        if (SceneManager.GetActiveScene().name != "Village" || SceneManager.GetActiveScene().name != "VillageWithTL")
        {
            return;
            caveEntrance = null;
            sendWave = null;
            playerSpawnTransform = null;
            
        }


        //if(GameObject.Find("PlayerSpawnPoint").transform != null)
        //    playerSpawnTransform = GameObject.Find("PlayerSpawnPoint").transform;
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
        if (SceneManager.GetActiveScene().name != "Village" || SceneManager.GetActiveScene().name != "VillageWithTL")
        {
            return;
        }
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Call only once
        if (isTriggered) return;

        if (other.CompareTag("Player"))
        {
            isTriggered = true;
            StartCave();
            Debug.Log("Sending waves");
        }
    }
}