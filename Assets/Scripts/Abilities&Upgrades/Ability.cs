using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;

public class Ability : ScriptableObject
{
    public new string name;
    public float damage;
    public float duration;

    //TODO Hide unrelevant info, for different settings. E.g cooldownTime, for non-timed cooldown,
    //and KeyCode for effectTriggered from other effects

    //TODO choose which layers can be hit

    //This might be redundant since keyTrigger can be an event

    public float cooldownTime;
    public KeyCode keyTrigger;

    public bool isFollowingCaster;
    public bool canChangeColors;

    //TODO Split these into Abstract classes instead

    public virtual void Initialize(GameObject obj)
    {

    }
    
    
    public virtual void ActivateEffect(ColliderDrawer colliderDrawer)
    {

    }

    public virtual List<GameObject> GetTargets()
    {

        //TODO Throw expection
        Debug.Log("Getting targets not specified");
        return null;
    }
}
