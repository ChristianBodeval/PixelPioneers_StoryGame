using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public Vector2 playerPosition;
    public string currentScene;

    // Booleans for weapons and upgrades
    public bool weapon1;
    public bool weapon1Upgrade1;
    public bool weapon1Upgrade2;
    public bool weapon2;
    public bool weapon2Upgrade1;
    public bool weapon2Upgrade2;
    public bool weapon3;
    public bool weapon3Upgrade1;
    public bool weapon3Upgrade2;
    public bool weapon4;
    public bool weapon4Upgrade1;
    public bool weapon4Upgrade2;
    public bool cave1Cleared;
    public bool cave2Cleared;
    public bool cave3Cleared;
    public bool cave4Cleared;

    public static SaveManager singleton { get; private set; }

    private const string PlayerPrefsKey = "PlayerData";

    private void Awake()
    {
        if (singleton != null && singleton != this)
        {
            Destroy(this);
        }
        else
        {
            singleton = this;
        }
    }

    private void Start()
    {
        playerPosition = GameObject.Find("Player").transform.position;
        currentScene = SceneManager.GetActiveScene().name;
        LoadPlayerData();
    }

    private void OnDestroy()
    {
        SavePlayerData();
    }

    public void SaveAbilityUpgrade(string abilityName, int upgradeNumber)
    {
        PlayerPrefs.SetInt(abilityName, upgradeNumber);
    }
    
    public void SavePlayerData()
    {
        // Save player position
        PlayerPrefs.SetFloat("PlayerPosX", playerPosition.x);
        PlayerPrefs.SetFloat("PlayerPosY", playerPosition.y);

        // Save current scene
        PlayerPrefs.SetString("CurrentScene", currentScene);

        // Save weapon and upgrade booleans
        PlayerPrefs.SetInt("Weapon1", weapon1 ? 1 : 0);
        PlayerPrefs.SetInt("Weapon1Upgrade1", weapon1Upgrade1 ? 1 : 0);
        PlayerPrefs.SetInt("Weapon1Upgrade2", weapon1Upgrade2 ? 1 : 0);

        PlayerPrefs.SetInt("Weapon2", weapon2 ? 1 : 0);
        PlayerPrefs.SetInt("Weapon2Upgrade1", weapon2Upgrade1 ? 1 : 0);
        PlayerPrefs.SetInt("Weapon2Upgrade2", weapon2Upgrade2 ? 1 : 0);

        PlayerPrefs.SetInt("Weapon3", weapon3 ? 1 : 0);
        PlayerPrefs.SetInt("Weapon3Upgrade1", weapon3Upgrade1 ? 1 : 0);
        PlayerPrefs.SetInt("Weapon3Upgrade2", weapon3Upgrade2 ? 1 : 0);

        PlayerPrefs.SetInt("Weapon4", weapon4 ? 1 : 0);
        PlayerPrefs.SetInt("Weapon4Upgrade1", weapon4Upgrade1 ? 1 : 0);
        PlayerPrefs.SetInt("Weapon4Upgrade2", weapon4Upgrade2 ? 1 : 0);

        PlayerPrefs.SetInt("Cave1Cleared", cave1Cleared ? 1 : 0);
        PlayerPrefs.SetInt("Cave2Cleared", cave2Cleared ? 1 : 0);
        PlayerPrefs.SetInt("Cave3Cleared", cave3Cleared ? 1 : 0);
        PlayerPrefs.SetInt("Cave4Cleared", cave4Cleared ? 1 : 0);
        

        
        PlayerPrefs.Save();
        
    }
    
    

    public void LoadPlayerData()
    {
        // Load player position
        float posX = PlayerPrefs.GetFloat("PlayerPosX");
        float posY = PlayerPrefs.GetFloat("PlayerPosY");
        playerPosition = new Vector2(posX, posY);

        // Load current scene
        currentScene = PlayerPrefs.GetString("CurrentScene");

        // Load weapon and upgrade booleans
        weapon1 = PlayerPrefs.GetInt("Weapon1") == 1;
        weapon1Upgrade1 = PlayerPrefs.GetInt("Weapon1Upgrade1") == 1;
        weapon1Upgrade2 = PlayerPrefs.GetInt("Weapon1Upgrade2") == 1;

        weapon2 = PlayerPrefs.GetInt("Weapon2") == 1;
        weapon2Upgrade1 = PlayerPrefs.GetInt("Weapon2Upgrade1") == 1;
        weapon2Upgrade2 = PlayerPrefs.GetInt("Weapon2Upgrade2") == 1;

        weapon3 = PlayerPrefs.GetInt("Weapon3") == 1;
        weapon3Upgrade1 = PlayerPrefs.GetInt("Weapon3Upgrade1") == 1;
        weapon3Upgrade2 = PlayerPrefs.GetInt("Weapon3Upgrade2") == 1;

        weapon4 = PlayerPrefs.GetInt("Weapon4") == 1;
        weapon4Upgrade1 = PlayerPrefs.GetInt("Weapon4Upgrade1") == 1;
        weapon4Upgrade2 = PlayerPrefs.GetInt("Weapon4Upgrade2") == 1;
        
        cave1Cleared = PlayerPrefs.GetInt("Cave1Cleared") == 1;
        cave2Cleared = PlayerPrefs.GetInt("Cave2Cleared") == 1;
        cave3Cleared = PlayerPrefs.GetInt("Cave3Cleared") == 1;
        cave4Cleared = PlayerPrefs.GetInt("Cave4Cleared") == 1;
    }
}
