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

    public Image topBorder;
    
    public bool isAvailable;
    private void OnEnable()
    {
        nameText.text = upgradeSO.name;
        descriptionText.text = upgradeSO.description;
        imageComponent.sprite = upgradeSO.sprite;
    }
    
    
    
    
    public bool isSelected;

    //Run when varible is changed
    void OnValidate()
    {
        //SetOutline(isSelected);
    }
    
    public void SetOutline(bool boolean)
    {
        topBorder.enabled = boolean;
        
    }
    
    
    
}

public interface ISelectable
{
    void SetOutline(bool boolean);
}
