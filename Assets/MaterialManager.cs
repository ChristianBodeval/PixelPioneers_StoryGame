using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    public Material blinkMaterial;
    public Material baseMaterial;
    public Material deathMaterial;

    public static MaterialManager singleton { get; private set; }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself
        if (singleton != null && singleton != this)
        {
            Destroy(this);
        }
        else
        {
            singleton = this;
        }
    }
}
