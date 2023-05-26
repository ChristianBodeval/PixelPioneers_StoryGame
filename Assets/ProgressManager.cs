using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    //Make a singleton
    public static ProgressManager Instance;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }
    
    public List<CaveEntrance> caveEntrances;
    
    public void SetCaveActive(int caveIndex, bool isActive)
    {
        if (caveIndex >= 0 && caveIndex < caveEntrances.Count)
        {
            CaveEntrance caveEntrance = caveEntrances[caveIndex];
            caveEntrance.SetActive(isActive);
        }
        else
        {
            Debug.LogError("Invalid cave index!");
        }
    }
}