using System;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class ProgressManager : MonoBehaviour
{
    //Make a singleton
    public static ProgressManager instance;

    
    //UI's
    public GameObject slashUI;
    public GameObject dashUI;
    public GameObject mjoelnirUI;
    public GameObject gungnirUI;

    //Abilities
    public Ability slashGO;
    public Ability dashGO;
    public Ability mjoelnirGO;
    public Ability gungnirGO;
    
    [FormerlySerializedAs("currentCaveAvailible")] public int numberOfCavesCleared;

    public string lastSceneName;
    
    private void OnEnable()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnSceneUnloaded(Scene unloadedScene)
    {
        lastSceneName = unloadedScene.name;
        
        Debug.Log("Scene unloaded: " + unloadedScene.name);
    }
    

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        lastSceneName = SceneManager.GetActiveScene().name;

        slashUI = GameObject.Find("SlashCD");
        dashUI = GameObject.Find("DashCD");
        mjoelnirUI = GameObject.Find("MjoelnirCD");
        gungnirUI = GameObject.Find("GungnirCD");
        
        slashGO = GameObject.Find("SlashAbility").GetComponent<Ability>();
        dashGO = GameObject.Find("Dash").GetComponent<Ability>();
        mjoelnirGO = GameObject.Find("Mjoelnir").GetComponent<Ability>();
        gungnirGO = GameObject.Find("GungnirThrow").GetComponent<Ability>();


        if (instance != null && instance != this)
        {
            Debug.Log("ProgressManager is existing, destroying this one");


            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }


    private void Start()
    {
        DisableAllAbilities();
        UpdateAllAbilities();
    }

    public void Update()
    {
        //If x is pressed, enable all abilities
        if (Input.GetKeyDown(KeyCode.X))
        {
            CaveHasBeenCleared();
        }
    }


    public List<CaveEntrance> caveEntrances;



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

                Debug.Log("Current cave entrance is: " + caveEntrance.gameObject.name);
                Debug.Log("Setting the player position to: " + caveEntrance.spawnPoint.position);
            }
        }
    }
    
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);
        if (scene.name == "CaveHub")
        {
            caveEntrances = new List<CaveEntrance>(FindObjectsOfType<CaveEntrance>());

            //Errase all caveEntrances from the list that are not tagged CaveEntrance
            for (int i = caveEntrances.Count - 1; i >= 0; i--)
            {
                if (caveEntrances[i].gameObject.tag != "CaveEntrance")
                {
                    caveEntrances.RemoveAt(i);
                }
            }
            
            
            //Sort them by gameobject name
            caveEntrances.Sort((x, y) => x.gameObject.name.CompareTo(y.gameObject.name));
            
            //Activate the number of caves equal to currentCaveAvailible
            for (int i = 0; i < caveEntrances.Count; i++)
            {
                caveEntrances[i].SetAccessibility(i < numberOfCavesCleared+1);

            }

            Invoke("UpdateUpgrades", 1f);
            
            //Search for the cave entrance with the connected to scene name as lastSceneName
            
        }
        

    }

    void UpdateUpgrades()
    {
        UpgradeManager.instance.UpdateProgress(numberOfCavesCleared+1);
        
        for (int i = 0; i < numberOfCavesCleared; i++)
        {
            caveEntrances[i].SetAccessibility(i < caveEntrances.Count);
            Debug.Log("Setting cave " + i + " to: " + (i < caveEntrances.Count));
        }
    }

    public void DisableAllAbilities()
    {
        //Disable all abilities && their UI's
        slashUI.SetActive(false);
        slashGO.GetComponent<Ability>().enabled = false;
        dashUI.SetActive(false);
        dashGO.GetComponent<Ability>().enabled = false;
        mjoelnirUI.SetActive(false);
        mjoelnirGO.GetComponent<Ability>().enabled = false;
        gungnirUI.SetActive(false);
        gungnirGO.GetComponent<Ability>().enabled = false;
    }

    public void UpdateAllAbilities()
    {
        if(numberOfCavesCleared == -1) return;
        
        if (numberOfCavesCleared > -1)
        {
            slashUI.SetActive(true);
            slashGO.enabled = true;
        }
        
        if (numberOfCavesCleared > 0)
        {
            dashUI.SetActive(true);
            dashGO.enabled = true;
        }
        if (numberOfCavesCleared > 1)
        {
            mjoelnirUI.SetActive(true);
            mjoelnirGO.enabled = true;
        }
        if (numberOfCavesCleared > 2)
        {
            gungnirUI.SetActive(true);
            gungnirGO.enabled = true;
        }
    }
    
    
    //Make a function that makes the next cave available
    public void CaveHasBeenCleared()
    {
        //If all caves are available, return
        if(numberOfCavesCleared == 4) return;
        numberOfCavesCleared++;
        
        UpdateAllAbilities();
    }
}