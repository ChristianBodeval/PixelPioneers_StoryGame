using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ColliderStat : ScriptableObject
{
    public new string name;

    //Attributes
    [Range(1f, 20f)]
    [SerializeField] public float range;
    [Range(3, 20)]
    [SerializeField] public int corners;
    [Range(0, 20f)]
    [SerializeField] public float width;
    [Range(0f, 360f)]
    [SerializeField] public float angle;
}
