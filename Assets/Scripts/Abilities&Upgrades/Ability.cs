using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : ScriptableObject
{
    public new string name;

    public float activeTime;
    public float damage;

    //TODO Hide unrelevant info, for different settings. E.g cooldownTime, for non-timed cooldown,
    //and KeyCode for effectTriggered from other effects

    //TODO choose which layers can be hit

    //This might be redundant since keyTrigger can be an event
    public float cooldownTime;
    public KeyCode keyTrigger;
    public Event eventTrigger;
    public Event offCooldownEvent;
    

    public TriggerType triggerType;
    public CooldownType cooldownType;
    public EffectType effectType;


    //Only for cooldown type of Timed
    public float duration;
    public float tickEveryXSeconds;


    //Hold down trigger
    //Todo finetune this 
    public float holdingChargeValue; //0 to 1 which is 0% to 100%
    public float startChargeValue; //E.g 0.05
    public float endChargeValue; 
    public float chargeUpSpeed;
    

    public enum TriggerType
    {
        OnEvent, //Fx saml spyd op, samtidig med en anden ability eller efter en anden ability er slut
        KeyPress,
        KeyHold
    }

    public enum EffectType
    {
        OneTime,
        OverTime,
        ChainEffect
    }

    public enum CooldownType
    {
        Timed,
        OnEvent
        //Eventuelt combination.
    }

    public IEnumerator EffectCoroutine;


    //Use Event instead?
    public List<Ability> OnStartAbilities;
    public List<Ability> OnEndAbilities;

    public virtual void ActivateEffect()
    {
        
        
    }

    public virtual void ActivateEffect(GameObject target)
    {
                
    }

    public virtual List<GameObject> GetTargets()
    {
        //TODO Throw expection
        Debug.Log("Getting targets not specified");
        return null;
    }
}
