using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public Transform playerTransform;
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

    private const string PlayerPrefsKey = "PlayerData";

    private void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        currentScene = SceneManager.GetActiveScene().name;
        LoadPlayerData();
    }

    private void OnDestroy()
    {
        SavePlayerData();
    }

    public void SavePlayerData()
    {
        // Save player position
        PlayerPrefs.SetFloat("PlayerPosX", playerTransform.position.x);
        PlayerPrefs.SetFloat("PlayerPosY", playerTransform.position.y);

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

        PlayerPrefs.Save();
    }

    public void LoadPlayerData()
    {
        // Load player position
        float posX = PlayerPrefs.GetFloat("PlayerPosX");
        float posY = PlayerPrefs.GetFloat("PlayerPosY");
        playerTransform.position = new Vector2(posX, posY);

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
    }
}
