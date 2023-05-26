using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    public CharacterSO characterSO;

    public TMP_Text nameText;
    public Image imageComponent;
    public Material outlineMaterial;
    
    public bool isAvailable;
    private void OnEnable()
    {
        nameText.text = characterSO.name;
        imageComponent.sprite = characterSO.faceIcon;
    }
    
    //Change the characterSO and then set all the values again
    public void SetCharacterSO(CharacterSO newCharacterSO)
    {
        characterSO = newCharacterSO;
        nameText.text = characterSO.name;
        imageComponent.sprite = characterSO.faceIcon;
    }
    

    public void SetOutline(bool boolean)
    {
        if(boolean)
            imageComponent.material = outlineMaterial;
        else
            imageComponent.material = null;
    }

}

