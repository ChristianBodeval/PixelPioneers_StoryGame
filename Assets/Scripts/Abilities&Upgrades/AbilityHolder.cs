using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;

public class AbilityHolder : MonoBehaviour
{
    public GameObject caster;
    public Ability ability;
    private float cooldownTime;
    private float duration;
    private float timeCasted;
    List<GameObject> targets;

    //TODO make an event for ready,active and cooldown and make the ColliderDrawer listen for them
    public Color readyColor;
    public Color activeColor;
    public Color cooldownColor;

    public AbilityHolder nextAbility;

    [SerializeField] public ColliderDrawer collider;
    [SerializeField] private SlashCone colliderStat;

    IEnumerator effectOverTimeCoroutine;
    IEnumerator chainDamageCoroutine;

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
        ability.Initialize(this.gameObject);

        if (ability.isFollowingCaster)
        {
            //Set caster as parent
            transform.parent = caster.transform;
            //Place them right on the parent
            transform.position = caster.transform.position;
            //Same rotation as caster
            transform.rotation = caster.transform.rotation;
        }
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
        if(ability.canChangeColors)
        {
            collider.spriteShapeRenderer.color = activeColor;
            yield return new WaitForSeconds(ability.duration);
            collider.spriteShapeRenderer.color = cooldownColor;
            yield return null;
        }
    }

    private void SetActive()
    {
        this.gameObject.transform.position = caster.transform.position;
        this.gameObject.transform.rotation = caster.transform.rotation;

        targets = collider.targets;

        if (targets.Count > 0)
            ability.ActivateEffect(targets);

        Debug.Log("Called Active");
        StartCoroutine(ChangeColor());
        //TODO reduce this to only all once
        duration = ability.duration;
        state = AbilityState.active;

        //Effect type
        if (nextAbility != null)
        {
            nextAbility.SetActive();
        }
    }

    private void SetCooldown()
    {
        //TODO tight solution, should instead be Ability specific
        chainDamageCoroutine = null;
        effectOverTimeCoroutine = null;

        cooldownTime = ability.cooldownTime;
        state = AbilityState.cooldown;
    }

    private void Start()
    {
        SetReady();
    }

    //TODO consider using nested switch-statements instead

    private void FixedUpdate()
    {
        switch (state)
        {
            case AbilityState.ready:

                if(Input.GetKeyDown(ability.keyTrigger)) 
                    SetActive();

                break;

                
            case AbilityState.active:

                SetCooldown();

                /*
                if (ability.effectType == Ability.EffectType.Instant)
                {
                    //Activate
                    ability.ActivateEffect(collider.targets);
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

                else if (ability.effectType == Ability.EffectType.Chain)
                {
                    targets = collider.targets;

                    //Run once only when there is targets
                    if (targets.Count == 0 || chainDamageCoroutine != null) //Guard clause to call once
                    {
                        return;
                    }

                    int randomNum = Random.Range(0, targets.Count - 1);

                    ability.ActivateEffect(targets);
                    StartCoroutine(chainDamageCoroutine);
                }*/
                break;

            case AbilityState.cooldown:
                Debug.Log("On cooldown");

                
                if (cooldownTime > 0)
                {
                    cooldownTime -= Time.deltaTime;
                }
                else
                {
                    SetReady();
                }

                break;

        }
    }

}
