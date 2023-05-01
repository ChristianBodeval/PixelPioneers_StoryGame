using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class AbilityHolder_UI : MonoBehaviour
{
    public Ability abilitySO;
    public TMP_Text nameText;
    public Image imageComponent;
    private void OnEnable()
    {
        nameText.text = "Change " + abilitySO.name + " Upgrade";
        imageComponent.sprite = abilitySO.sprite;
    }
    private void OnBecameInvisible()
    {
        nameText.text = null;
        imageComponent = null;
    }
}

