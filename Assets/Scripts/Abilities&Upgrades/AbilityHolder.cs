using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;

public class AbilityHolder : MonoBehaviour
{
    public GameObject caster;
    public Ability ability;
    float cooldownTime;
    private float duration;

    private float timeCasted;
    List<Transform> targets;

    //TODO make an event for ready,active and cooldown and make the ColliderDrawer listen for them
    public Color readyColor;
    public Color activeColor;
    public Color cooldownColor;

    public AbilityHolder nextAbility;

    [SerializeField] public ColliderDrawer collider;

    [SerializeField] private SlashCone colliderStat;


    IEnumerator effectOverTimeCoroutine;


    enum AbilityState
    {
        ready,
        active,
        cooldown
    }


    private void ActivateNextAbility()
    {
        nextAbility.SetActive();
    }

    private void Awake()
    {
        duration = ability.duration;

    }

    AbilityState state = AbilityState.ready; 

    private void SetReady()
    {
        collider.spriteShapeRenderer.color = readyColor;
        state = AbilityState.ready;


        //Copy position. DOSEN'T WORK - How do i handle it should follow the player?

        
        

    }

    private IEnumerator ChangeColor()
    {
        collider.spriteShapeRenderer.color = activeColor;
        yield return new WaitForSeconds(ability.duration);
        collider.spriteShapeRenderer.color = cooldownColor;
        yield return null;
    }

    private void SetActive()
    {
        this.gameObject.transform.position = caster.transform.position;
        this.gameObject.transform.rotation = caster.transform.rotation;


        Debug.Log("Called Active");
        StartCoroutine(ChangeColor());
        //TODO reduce this to only all once
        duration = ability.duration;
        state = AbilityState.active;

        //Sets the players position to the ability
    }

    private void SetCooldown()
    {
        cooldownTime = ability.cooldownTime;
        state = AbilityState.cooldown;
    }

    private List<GameObject> GetTargets()
    {
        return collider.targets;
    }


    public IEnumerator EffectOverTime()
    {
        while (Time.time < timeCasted)
        {

            
            

            ability.ActivateEffect(GetTargets());
            Debug.Log("Casted");
            yield return new WaitForSeconds(1f);
        }
        SetCooldown();
        yield return null;
    }


    private void Start()
    {
        SetReady();
    }

    //TODO consider using nested switch-statements instead

    private void FixedUpdate()
    {
        //Follow transform or not
        if (ability.followCaster)
        {
            this.gameObject.transform.position = caster.transform.position;
            this.gameObject.transform.rotation = caster.transform.rotation;
        }        

        //Cast next ability

        

        switch (state)
        {
            case AbilityState.ready:

                Debug.Log("Ability ready");
                if (ability.triggerType == Ability.TriggerType.KeyHold && Input.GetKey(ability.keyTrigger))
                    Debug.Log("Charging up");
                    //TODO Hold && charge up value

                else if(ability.triggerType == Ability.TriggerType.KeyPress && Input.GetKeyDown(ability.keyTrigger)) 
                    SetActive();

                else if(ability.triggerType == Ability.TriggerType.OnEvent && ability.activateEvent != null)
                    SetActive();

                break;

                
            case AbilityState.active:

                Debug.Log("Activated");


                if (nextAbility != null)
                {
                    nextAbility.SetActive();
                }


                if (ability.effectType == Ability.EffectType.Instant)
                {
                    //Activate
                    ability.ActivateEffect(GetTargets());
                    SetCooldown();

                }
                else if (ability.effectType == Ability.EffectType.OverTime)
                {
                    if (effectOverTimeCoroutine != null) //Guard clause to call once
                    {
                        return;
                    }

                    effectOverTimeCoroutine = EffectOverTime();
                    timeCasted = Time.time + duration;
                    StartCoroutine(effectOverTimeCoroutine);
                }
                break;

            case AbilityState.cooldown:
                Debug.Log("On cooldown");


                if (ability.cooldownType == Ability.CooldownType.Timed)
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
                    //if (ability.offCooldownEvent != null)
                    //{
                    //    SetReady();
                    //}
                }
                break;

        }
    }

}
