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

    private Rigidbody2D casterRB; 

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
        casterRB = caster.GetComponent<Rigidbody2D>();
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

        if (collider.targets.Count > 0) ability.ActivateEffect(this,targets);

        if (ability.anim != null) PlayAnim();

        StartCoroutine(ChangeColor());
        duration = ability.duration;
        state = AbilityState.active;

        //Effect type
        if (nextAbility != null)
        {
            nextAbility.SetActive();
        }
    }

    public void PlayAnim()
    {
        GameObject player = GameObject.Find("Player");
        GameObject cone = Instantiate(ability.anim, player.transform);
        Vector2 direction = player.GetComponent<PlayerAction>().lastFacing;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        cone.transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
        cone.transform.localPosition = direction * 1f;
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

    private void Update()
    {
        //Change direction of the ability according to where the player is moving
        if (casterRB.velocity.magnitude > 0)
            transform.right = casterRB.velocity;

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
