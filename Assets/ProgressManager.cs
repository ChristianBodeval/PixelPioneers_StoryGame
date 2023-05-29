using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressManager : MonoBehaviour
{
    //Make a singleton
    public static ProgressManager instance;

    public int currentCaveAvailible;

    
    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (instance != null && instance != this)
        {
            Debug.Log("Destroying ProgressManager");
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    
    public List<CaveEntrance> caveEntrances;

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        if (scene.name == "CaveHub")
        {
            caveEntrances = new List<CaveEntrance>(FindObjectsOfType<CaveEntrance>());
            
            //Find all CaveEntrances with tag CaveEntrance




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
                caveEntrances[i].SetAccessibility(i < currentCaveAvailible);
                Debug.Log("Setting cave " + i + " to: " + (i < currentCaveAvailible));
            }
            Debug.Log("On Scene loaded");
            
            //FindObjectOfType<UpgradeUI>().progressNumber = currentCaveAvailible;
            
            

            Invoke("UpdateUpgrades", 1f);
            
            //UpgradeManager.instance.UpdateProgress(currentCaveAvailible);
        }
    }

    void UpdateUpgrades()
    {
        UpgradeManager.instance.UpdateProgress(currentCaveAvailible+1);
        
        for (int i = 0; i < currentCaveAvailible; i++)
        {
            caveEntrances[i].SetAccessibility(i < caveEntrances.Count);
            Debug.Log("Setting cave " + i + " to: " + (i < caveEntrances.Count));
        }
    }
    
    
    //Make a function that makes the next cave available
    public void SetNextCaveActive()
    {
        //If all caves are available, return
        if(currentCaveAvailible == 4) return;
        currentCaveAvailible++;
        
    }
}