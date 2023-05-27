using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    //Make a singleton
    public static ProgressManager Instance;
    
    public int currentCaveAvailible = 0;

    private void Awake()
    {
        currentCaveAvailible = 1;
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }
    
    public List<CaveEntrance> caveEntrances;
    
    //Make a function that makes the next cave available
    public void SetNextCaveActive()
    {
        //If all caves are available, return
        if(currentCaveAvailible == caveEntrances.Count) return;
        caveEntrances[currentCaveAvailible].SetActive(true);
    }
}