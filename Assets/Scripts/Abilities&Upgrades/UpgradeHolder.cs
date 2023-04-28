using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeHolder : MonoBehaviour
{
    public UpgradeSO upgradeSO;

    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public Image imageComponent;

    private void OnEnable()
    {
        nameText.text = upgradeSO.name;
        descriptionText.text = upgradeSO.description;
        imageComponent.sprite = upgradeSO.sprite;
    }
}
