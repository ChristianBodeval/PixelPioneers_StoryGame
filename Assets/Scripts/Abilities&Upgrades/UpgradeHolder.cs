using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UpgradeHolder : MonoBehaviour, ISelectable
{
    public UpgradeSO upgradeSO;

    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public Image imageComponent;
    public Material outlineMaterial;

    public bool isAvailable;
    private void OnEnable()
    {
        nameText.text = upgradeSO.name;
        descriptionText.text = upgradeSO.description;
        imageComponent.sprite = upgradeSO.sprite;
    }
    
    public void IsSelected(bool boolean)
    {
        if (boolean)
        {   
            //Make outlineMaterial transparent
            outlineMaterial.color = Color.white;
        }
        else
        {
            outlineMaterial.color = Color.clear;
        }
        
        isAvailable = boolean; 
    }

    public void SetOutline(bool boolean)
    {
        if(boolean)
            imageComponent.material = outlineMaterial;
        else
            imageComponent.material = null;
    }

    public ScriptableObject GetScriptableObject()
    {
        throw new NotImplementedException();
    }

    public ScriptableObject SelectThis()
    {
        return upgradeSO;
    }
    
}

public interface ISelectable
{
    void SetOutline(bool boolean);
}
