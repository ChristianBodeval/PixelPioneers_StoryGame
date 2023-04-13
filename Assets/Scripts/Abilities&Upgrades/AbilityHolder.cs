using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;
using UnityEngine.Windows;
using Input = UnityEngine.Input;

public class AbilityHolder : MonoBehaviour
{
    public GameObject caster;
    public Ability ability;
    private float cooldownTime;
    private float duration;
    private float timeCasted;
    List<GameObject> targets;
    public Color readyColor;
    public Color activeColor;
    public Color cooldownColor;

    public AbilityHolder nextAbility;

    [SerializeField] public ColliderDrawer collider;
    [SerializeField] private SlashCone colliderStat;
    private Vector2 movement;

    IEnumerator effectOverTimeCoroutine;
    IEnumerator chainDamageCoroutine;



    AbilityState state = AbilityState.ready; 

    enum AbilityState
    {
        ready,
        active,
        cooldown
    }

    private void ActivateNextAbility()
    {
        if(nextAbility != null)
            nextAbility.SetActive();
    }

    private void Awake()
    {
        ability.Initialize(this.gameObject);
        duration = ability.duration;

        if (ability.isFollowingCaster)
        {
            transform.parent = caster.transform;
            transform.position = caster.transform.position;
            transform.rotation = caster.transform.rotation;
        }
    }


    private void SetReady()
    {
        collider.spriteShapeRenderer.color = readyColor;
        state = AbilityState.ready;
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
        targets = collider.targets;

        if (targets.Count > 0)
        {
            ability.ActivateEffect(this,targets);
        }

        StartCoroutine(ChangeColor());
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
        chainDamageCoroutine = null;
        effectOverTimeCoroutine = null;

        cooldownTime = ability.cooldownTime;
        state = AbilityState.cooldown;
    }

    private void Start()
    {
        SetReady();
    }

    private void FixedUpdate()
    {
        //Change direction of the ability according to the input
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        movement = new Vector2(inputX, inputY);


        if(inputX != 0 || inputY != 0)
            transform.right = movement;

        switch (state)
        {
            case AbilityState.ready:

                if(Input.GetKeyDown(ability.keyTrigger)) 
                    SetActive();
                break;

                
            case AbilityState.active:

                SetCooldown();
                break;

            case AbilityState.cooldown:
                
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