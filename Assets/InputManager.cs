using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public static InputManager inputManager { get; private set; } // Singleton

    private void Awake()
    {
        // Singleton pattern
        if (inputManager != null && inputManager != this)
        {
            Destroy(this);
        }
        else
        {
            inputManager = this;
        }
    }



}
