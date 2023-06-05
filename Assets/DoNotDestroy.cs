using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNotDestroy : MonoBehaviour
{
    // If already existing, destroy this one
    
    private void Awake()
    {
        if (FindObjectsOfType<DoNotDestroy>().Length > 1)
        {
            Destroy(this.gameObject);
        }
        
        else
            DontDestroyOnLoad(this.gameObject);
    }
}
