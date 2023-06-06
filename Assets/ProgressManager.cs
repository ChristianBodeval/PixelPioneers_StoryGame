using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class ProgressManager : MonoBehaviour
{
    //TODO DELETE
    public int currentCavesCleared;
    //Make a singleton
    public static ProgressManager instance;
    
    //UI's
    public GameObject slashUI;
    public GameObject dashUI;
    public GameObject mjoelnirUI;
    public GameObject gungnirUI;

    private bool isNewScene = false;
    private Scene scene;
    private LoadSceneMode mode;
    
    //Abilities
    [FormerlySerializedAs("slashGO")] public Ability slashScript;
    [FormerlySerializedAs("dashGO")] public Ability dashScript;
    [FormerlySerializedAs("mjoelnirGO")] public Ability mjoelnirScript;
    [FormerlySerializedAs("gungnirGO")] public Ability gungnirScript;
    

    public List<CaveEntrance> caveEntrances = new List<CaveEntrance>();
    public string lastSceneName;

    private TextAsset exitCave01;
    private TextAsset exitCave02;
    private TextAsset exitCave03;

    private void OnEnable()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        
        
    }

    private void OnSceneUnloaded(Scene unloadedScene)
    {
        lastSceneName = unloadedScene.name;
    }

    private void Awake()
    {

        
        
        //Reset playerprefs
        PlayerPrefs.DeleteAll();
        
        
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

     
        FindAbilityComponents();

       
                
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        lastSceneName = SceneManager.GetActiveScene().name;
        

        exitCave01 = Resources.Load<TextAsset>("Dialogue/ExitCave01");
        exitCave02 = Resources.Load<TextAsset>("Dialogue/ExitCave02");
        exitCave03 = Resources.Load<TextAsset>("Dialogue/ExitCave03");

    }

    private void Start()
    {
        DisableAllAbilities();
        UpdateAllAbilities();

       
        SaveManager.singleton.cavesCleared = currentCavesCleared;
       
    }

    public void UpdatePlayerPosition()
    {
        foreach(CaveEntrance caveEntrance in caveEntrances)
        {
            if (lastSceneName == "")
            {
                return;
            }
            if (String.Equals(caveEntrance.connectedToSceneName, lastSceneName)) 
            {
                Debug.Log(caveEntrance.connectedToSceneName + " is equal to " + lastSceneName);
                //Set the player position to the cave entrance position
                GameObject.Find("Player").transform.position = caveEntrance.spawnPoint.position;
            }
        }
    }

    private void Update()
    {
        if (isNewScene) NewScene();
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isNewScene = true;
        this.scene = scene;
        this.mode = mode;

        
        // Reset caveEntrances to null
        caveEntrances.Clear();
        
        
        caveEntrances = new List<CaveEntrance>(FindObjectsOfType<CaveEntrance>());
        
        if (scene.name == "CaveHub")
        {
            UpdatePlayerPosition();

            //Erase all caveEntrances from the list that are not tagged CaveEntrance
            for (int i = caveEntrances.Count - 1; i >= 0; i--)
            {
                if (!caveEntrances[i].gameObject.CompareTag("CaveEntrance"))
                {
                    caveEntrances.RemoveAt(i);
                }
            }

            //Sort them by gameobject name
            caveEntrances.Sort((x, y) => x.gameObject.name.CompareTo(y.gameObject.name));

            

            //Activate the number of caves equal to currentCaveAvailible
            for (int i = 0; i < caveEntrances.Count; i++)
            {
                caveEntrances[i].SetAccessibility(i < SaveManager.singleton.cavesCleared + 1);
            }

            UpgradeManager.instance.UpdateProgress(SaveManager.singleton.cavesCleared + 1);
        }
    }

    private void NewScene()
    {
        isNewScene = false;

        caveEntrances = new List<CaveEntrance>(FindObjectsOfType<CaveEntrance>());

        FindAbilityComponents();
        UpdateAllAbilities();

        if (HealthPickUp.pickUpPool != null && HealthPickUp.pickUpPool.isActiveAndEnabled) HealthPickUp.pickUpPool.ClearLists();
        if (Pool.pool != null && Pool.pool.isActiveAndEnabled) Pool.pool.ClearLists();
        
        if (WeaponPickUp.stoneConvoPrepped > 0 && WeaponPickUp.isConvoPrepped)
        {

            switch (WeaponPickUp.stoneConvoPrepped)
            {
                case 1:
                    DialogueManager.dialogManager.EnterDialogueMode(exitCave01);
                    break;
                case 2:
                    DialogueManager.dialogManager.EnterDialogueMode(exitCave02);
                    break;
                case 3:
                    DialogueManager.dialogManager.EnterDialogueMode(exitCave03);
                    break;
            }
            WeaponPickUp.isConvoPrepped = false;

        }
    }

    private void FindAbilityComponents()
    {
        GameObject cds = GameObject.Find("CDs");
        slashUI = FindChildObjectByName(cds.transform, "SlashCD");
        dashUI = FindChildObjectByName(cds.transform, "DashCD");
        mjoelnirUI = FindChildObjectByName(cds.transform, "MjoelnirCD");
        gungnirUI = FindChildObjectByName(cds.transform, "GungnirCD");

        slashScript = GameObject.Find("SlashAbility").GetComponent<Ability>();
        dashScript = GameObject.Find("Dash").GetComponent<Ability>();
        mjoelnirScript = GameObject.Find("Mjoelnir").GetComponent<Ability>();
        gungnirScript = GameObject.Find("GungnirThrow").GetComponent<Ability>();
    }

    private GameObject FindChildObjectByName(Transform parent, string objectName)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform childTransform = parent.GetChild(i);
            if (childTransform.name == objectName)
            {
                return childTransform.gameObject;
            }
        }
        return null;
    }


    public void DisableAllAbilities()
    {
        //Disable all abilities && their UI's
        slashUI.SetActive(false);
        slashScript.GetComponent<Ability>().enabled = false;
        dashUI.SetActive(false);
        dashScript.GetComponent<Ability>().enabled = false;
        mjoelnirUI.SetActive(false);
        mjoelnirScript.GetComponent<Ability>().enabled = false;
        gungnirUI.SetActive(false);
        gungnirScript.GetComponent<Ability>().enabled = false;
    }

    public void UpdateAllAbilities()
    {
        slashUI.SetActive(SaveManager.singleton.cavesCleared > -1);
        slashScript.enabled = SaveManager.singleton.cavesCleared > -1;

        dashUI.SetActive(SaveManager.singleton.cavesCleared > 0);
        dashScript.enabled = SaveManager.singleton.cavesCleared > 0;

        mjoelnirUI.SetActive(SaveManager.singleton.cavesCleared > 1);
        mjoelnirScript.enabled = SaveManager.singleton.cavesCleared > 1;

        gungnirUI.SetActive(SaveManager.singleton.cavesCleared > 2);
        gungnirScript.enabled = SaveManager.singleton.cavesCleared > 2;

        
        
        Debug.Log("Updating abilities");
        if (SaveManager.singleton.weapon1Upgrade1) slashScript.UpgradeOption1();
        if (SaveManager.singleton.weapon1Upgrade2) slashScript.UpgradeOption2();

        if (SaveManager.singleton.weapon2Upgrade1) GameObject.Find("Dash").GetComponent<Dash>().hasUpgrade1 = true;
        if (SaveManager.singleton.weapon2Upgrade2) GameObject.Find("Dash").GetComponent<Dash>().hasUpgrade2 = true;

        if (SaveManager.singleton.weapon3Upgrade1) GameObject.Find("Mjoelnir").GetComponent<Mjoelnir>().hasChargeUpgrade = true;
        if (SaveManager.singleton.weapon3Upgrade2) GameObject.Find("Mjoelnir").GetComponent<Mjoelnir>().hasAreaOfEffectUpgrade = true;

        if (SaveManager.singleton.weapon4Upgrade1) GameObject.Find("GungnirThrow").GetComponent<ThrowGungnir>().hasUpgrade1 = true;
        if (SaveManager.singleton.weapon4Upgrade2) GameObject.Find("GungnirThrow").GetComponent<ThrowGungnir>().hasUpgrade2 = true;
    }

    //Make a function that makes the next cave available
    public void CaveHasBeenCleared()
    {
        
        
        
        //If all caves are available, return
        if(SaveManager.singleton.cavesCleared == 4) return;
        SaveManager.singleton.cavesCleared++;
        SaveManager.singleton.SavePlayerData();

        FindAbilityComponents();
        UpdateAllAbilities();
    }
}