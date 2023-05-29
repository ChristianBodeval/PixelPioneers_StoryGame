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
    
    
    public void SetActive(bool b)
    {
        if (b)
        {
            Debug.Log("Called by" + this.gameObject.name);
            abilityNameText.text = "Change " + abilitySO.name + " Upgrade";
            imageComponent.sprite = abilitySO.sprite;
            
            upgradeNameText.enabled = true;
            currentUpgradeText.enabled = true;

        }
        else
        {
            abilityNameText.text = "";
            imageComponent.sprite = unavailibleSprite;
            
            upgradeNameText.enabled = false;
            currentUpgradeText.enabled = false;
        }
    }
    public void SetOutline(bool boolean)
    {
        if (boolean)
        {
            changeBorder.material = outlineMaterial;
            imageComponent.material = outlineMaterial;
        }
        else
        {
            changeBorder.material = null;
            imageComponent.material = null;
        }
    }

    public ScriptableObject GetScriptableObject()
    {
        throw new NotImplementedException();
    }
}

