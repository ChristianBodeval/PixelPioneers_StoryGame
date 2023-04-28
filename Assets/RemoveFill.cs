using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemoveFill : MonoBehaviour
{
    [SerializeField] private Image fill;

    private void OnEnable()
    {
        fill.enabled = true;
    }

    public void Remove()
    {
        fill.enabled = false;
    }

    public Image GetImageComponent()
    {
        return fill;
    }
}
