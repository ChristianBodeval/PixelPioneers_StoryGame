using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class AbilityHolder_UI : MonoBehaviour, ISelectable
{
    public AbilitySO abilitySO;
    public TMP_Text nameText;
    public Image imageComponent;
    public Image changeBorder;
    public Material outlineMaterial;
    
    private void OnEnable()
    {
        Debug.Log("Called by" + this.gameObject.name);
        nameText.text = "Change " + abilitySO.name + " Upgrade";
        imageComponent.sprite = abilitySO.sprite;
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

