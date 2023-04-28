using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

[CreateAssetMenu]
public class UpgradeSO : ScriptableObject
{
    public string name;
    [TextArea(15,20)]
    public string description;
    public Sprite sprite;
}
