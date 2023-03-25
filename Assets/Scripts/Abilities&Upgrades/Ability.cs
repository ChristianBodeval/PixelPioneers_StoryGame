using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;

public class Ability : ScriptableObject
{
    public new string name;
    public float damage;

    //TODO Hide unrelevant info, for different settings. E.g cooldownTime, for non-timed cooldown,
    //and KeyCode for effectTriggered from other effects

    //TODO choose which layers can be hit

    //This might be redundant since keyTrigger can be an event
    public float cooldownTime;
    public KeyCode keyTrigger;

    [Header("Only for effect over time")]
    public UnityEvent activateEvent;
    public UnityEvent offCooldownEvent;
    
    public TriggerType triggerType;
    public CooldownType cooldownType;
    public EffectType effectType;


    //Only for effect over time
    [Header("Only for effect over time")]
    public float duration;
    public float tickEveryXSeconds;

    //Hold down trigger
    //Todo finetune this
    [Header("Only for KeyHold")]
    public float holdingChargeValue; //0 to 1 which is 0% to 100%
    public float startChargeValue; //E.g 0.05
    public float endChargeValue; 
    public float chargeUpSpeed;

    public bool followCaster;

    public enum TriggerType
    {
        OnEvent, //Fx saml spyd op, samtidig med en anden ability eller efter en anden ability er slut
        KeyPress,
        KeyHold
    }

    public enum EffectType
    {
        Instant,
        OverTime
    }

    public enum CooldownType
    {
        Timed,
        OnEvent
        //Eventuelt combination.
    }

    public IEnumerator EffectCoroutine;



    public virtual void ActivateEffect(List<GameObject> targets)
    {
        
    }

    public virtual void ActivateEffect(AbilityHolder ability)
    {
        
        //Follow caster
    }



    public virtual void BeginCooldown(GameObject player)
    {

    }


    public virtual List<GameObject> GetTargets()
    {

        //TODO Throw expection
        Debug.Log("Getting targets not specified");
        return null;
    }
}
