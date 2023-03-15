using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHolder : MonoBehaviour
{
    public Ability ability;
    float cooldownTime;
    float activeTime;
    List<Transform> targets;



    public KeyCode key;

    enum AbilityState
    {
        ready,
        active,
        cooldown
    }

    AbilityState state = AbilityState.ready; 


    private void SetReady()
    {
        state = AbilityState.ready;
    }

    private void SetActive()
    {
        //TODO reduce this to only all once
        activeTime = ability.activeTime;
        state = AbilityState.active;
    }

    private void SetCooldown()
    {
        cooldownTime = ability.cooldownTime;
        state = AbilityState.cooldown;
    }


    public IEnumerator EffectOverTime()
    {
        float duration = Time.deltaTime + ability.activeTime;

        while (Time.deltaTime < duration)
        {
            ability.ActivateEffect();
            yield return new WaitForSeconds(ability.tickEveryXSeconds);
        }
        yield return null;
    }


    //TODO consider using nested switch-statements instead

    private void Update()
    {
        switch (state)
        {
            case AbilityState.ready:

                if (ability.triggerType == Ability.TriggerType.KeyHold && Input.GetKey(ability.keyTrigger))
                    Debug.Log("Charging up");
                    //TODO Hold && charge up value

                else if(ability.triggerType == Ability.TriggerType.KeyPress && Input.GetKeyDown(ability.keyTrigger)) 
                    SetActive();

                else if(ability.triggerType == Ability.TriggerType.OnEvent && ability.eventTrigger != null)
                    SetActive();

                break;

                //Might be redundant, since Active() should handle how it is active.
            case AbilityState.active:

                if(ability.effectType == Ability.EffectType.OneTime)
                {
                    //GetTargets
                    //Activate
                    ability.ActivateEffect();
                    SetCooldown();

                }
                else if(ability.effectType == Ability.EffectType.OverTime)
                {
                    //GetTargets
                    EffectOverTime();
                    SetCooldown();

                }
                else if(ability.effectType == Ability.EffectType.ChainEffect)
                {
                    //TODO test later
                    //GetTargets

                    /*
                    foreach (var target in targets)
                    {
                        //Get new target x times by running for loop
                        for()
                        ability.ActivateEffect
                    }
                    
                    ability.ActivateEffect(target);
                    */
                }


                // HIS CODE - look at what he did
                if(activeTime > 0) 
                {
                    activeTime -= Time.deltaTime;
                }
                else
                {
                    state = AbilityState.cooldown;
                    cooldownTime = ability.cooldownTime;
                }
                break;

            case AbilityState.cooldown:
                if(ability.cooldownType == Ability.CooldownType.Timed)
                {
                    if (cooldownTime > 0)
                    {
                        cooldownTime -= Time.deltaTime;
                    }
                    else
                    {
                        SetReady();
                    }
                }

                else if (ability.cooldownType == Ability.CooldownType.OnEvent)
                {
                    //Listen for event then set ready
                    if (ability.offCooldownEvent != null)
                    {
                        SetReady();
                    }
                }
                break;

        }
    }

}
