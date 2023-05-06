using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Anvil : Interactable
{
    [SerializeField] private SpriteRenderer makerIcon;
    [SerializeField] private SpriteRenderer inRangeIcon;
    public override void ShowIndicatorForInRange()
    {
        makerIcon.enabled = false;
        inRangeIcon.enabled = true;
    }

    public override void HideIndicatorForInRange()
    {
        makerIcon.enabled = true;
        inRangeIcon.enabled = false;
    }

    public override void Interact()
    {
        UpgradeManager.instance.OpenUpgradeUI();
    }
}
