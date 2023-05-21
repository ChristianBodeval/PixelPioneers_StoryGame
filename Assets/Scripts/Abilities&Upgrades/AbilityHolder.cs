using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.U2D;
using UnityEngine.Windows;
using Input = UnityEngine.Input;

//Handles the state of the ability, position and rotation & changes color of the collider
public class AbilityHolder : Ability, IUpgradeable
{
    public GameObject caster;
    
    public PlayerAction playerAction;
    public AbilitySO ability;
    private float cooldownTime;
    private float duration;
    private float timeCasted;
    public Color readyColor;
    public Color activeColor;
    public Color cooldownColor;

    public AbilityHolder nextAbility;

    public GameObject slashAnimation;

    [FormerlySerializedAs("collider")] [SerializeField] public ColliderDrawer hitCollider;
    [SerializeField] private SlashCone colliderStat;
    private Vector2 movement;
    
    AbilityState state = AbilityState.ready;
    public Transform spawnPoint;
    [SerializeField] private AbilityHolder upgradeOption1;
    [SerializeField] private AbilityHolder upgradeOption2;

    enum AbilityState
    {
        ready,
        active,
        cooldown
    }
    
    //Calling in IEnumerator to wait for next frame, since the SpriteShapeRenderer is not updated in the same frame
    private IEnumerator ActivateNextAbility()
    {
        if (nextAbility != null)
        {
            nextAbility.SetActive();
        }
        yield return null;
    }
    
    private IEnumerator ActivateEffect()
    {
        yield return new WaitForSeconds(00.1f);
        if (hitCollider.targets.Count > 0)
        {
            ability.ActivateEffect(hitCollider);
        }

        yield return null;
    }

    private void Awake()
    {
        readyColor = GetComponent<SpriteShapeRenderer>().color;
        
        //Set caster to gameobject called player
        
        
        ability.Initialize(this.gameObject);
        duration = ability.duration;

    }

    private void Start()
    {
        if (ability.isFollowingCaster)
        {
            transform.parent = caster.transform;
            transform.position = caster.transform.position;
            transform.rotation = caster.transform.rotation;
        }
        
        if (!ability.isFollowingCaster)
        {
            transform.position = caster.transform.position;
            if (playerAction != null)
            {
                transform.right = playerAction.lastFacing;
            }
        }
        
        SetReady();
    }


    private void SetReady()
    {
        if (ability.canChangeColors)
        {
            hitCollider.spriteShapeRenderer.color = readyColor;
            state = AbilityState.ready;
        }
    }

    
    private IEnumerator ChangeColor()
    {
        if(ability.canChangeColors)
        {
            hitCollider.spriteShapeRenderer.color = activeColor;
            yield return new WaitForSeconds(ability.duration);
            hitCollider.spriteShapeRenderer.color = cooldownColor;
            yield return null;
        }
    }

    public void SetActive()
    {
        //If AbilitySO is SlashAbility
        

        Debug.Log("this.gameObject = " + this.gameObject);
        if(spawnPoint != null)
            transform.position = spawnPoint.transform.position;
        else if (!ability.isFollowingCaster)
        {
            transform.position = caster.transform.position;
            transform.right = playerAction.lastFacing;
        }

        Debug.Log("this.gameObject = " + this.gameObject);
        
        hitCollider.UpdateCollider();

        StartCoroutine(ChangeColor());
        duration = ability.duration;
        state = AbilityState.active;
        
        
        
        StartCoroutine(ActivateEffect());
//            ability.ActivateEffect(hitCollider);
        //Effect type
        if (nextAbility != null)
        {
            nextAbility.SetActive();
        }

        //

        if (abilitySO.name == "MeleeAttack")
        {
            Debug.Log("Spawn");
            //GameObject slash = Instantiate(slashAnimation, playerAction.transform.position, playerAction.lastFacing.rotation);
            //Instantiate Slashanimation

        }
    }
    
    public void SetCooldown()
    {
        cooldownTime = ability.cooldownTime;
        state = AbilityState.cooldown;
    }

    

    private void Update()
    {
        //Change direction of the ability according to where the player is moving
        //if (ability.isFollowingCaster && casterRB.velocity.magnitude > 0)
        //    transform.right = casterRB.velocity;
        
        if (ability.isFollowingCaster)
        {
            transform.right = playerAction.lastFacing;
        }

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


    public List<UpgradeSO> GetUpgrades()
    {
        throw new System.NotImplementedException();
    }

    public AbilitySO GetAbilitySO()
    {
        return ability;
    }

    public void UpgradeOption1()
    {
        nextAbility = upgradeOption1;
    }

    public void UpgradeOption2()
    {
        nextAbility = upgradeOption2;
    }

    public void Downgrade()
    {
        nextAbility = null;
    }
}
