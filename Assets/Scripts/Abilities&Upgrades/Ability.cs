using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;

[CreateAssetMenu]
public class Ability : ScriptableObject
{
    public new string name;
    [TextArea(1, 5)] public string description;
    public float damage;
    public float duration;
    public float cooldownTime;
    public KeyCode keyTrigger;
    public Sprite sprite;

    //TODO Move this to specific spells using this
    public bool isFollowingCaster;
    public bool canChangeColors;
    
    public virtual void Initialize(GameObject obj)
    {

    }
    
    public virtual void ActivateEffect(ColliderDrawer colliderDrawer)
    {

    }
}
