using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class AbilityHolder_UI : MonoBehaviour, ISelectable
{
    public AbilitySO abilitySO;
    public TMP_Text currentUpgradeText;
    public TMP_Text upgradeNameText;
    public TMP_Text abilityNameText;
    public Image imageComponent;
    public Image changeBorder;
    public Material outlineMaterial;
    public Sprite unavailibleSprite;
    public Image border;
    public Image eKey;
    
    public bool isSelected;
    public bool isActivated;
    
    //Run when varible is changed
    void OnValidate()
    {
        
        SetOutline(isSelected);
        SetActive(isActivated);
        
    }
    
    public void SetActive(bool b)
    {
        upgradeNameText.enabled = b;
        currentUpgradeText.enabled = b;
        
        
        if (b)
        {
            Debug.Log("Called by" + this.gameObject.name);
            abilityNameText.text = "Change " + abilitySO.name + " Upgrade";
            imageComponent.sprite = abilitySO.sprite;
        }
        else
        {
            abilityNameText.text = "";
            imageComponent.sprite = unavailibleSprite;
        }
    }
    
    
    
    public void SetOutline(bool boolean)
    {
        isSelected = boolean;
        eKey.enabled = boolean;
        border.enabled = boolean;
    }

}

