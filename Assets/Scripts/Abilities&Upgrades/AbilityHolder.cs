using System;
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
    
    private PlayerAction playerAction;
    public AbilitySO ability;
    private float cooldownTime;
    private float duration;
    private float timeCasted;
    public Color readyColor;
    public Color activeColor;
    public Color cooldownColor;

    public AbilityHolder nextAbility;
    [SerializeField] public ColliderDrawer hitCollider;
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

    private void Awake()
    {
        upgradeOption1 = GameObject.Find("FloorIsElectric").GetComponent<AbilityHolder>();
        upgradeOption2 = GameObject.Find("ElectricLine").GetComponent<AbilityHolder>();
    }


    void OnEnable()
    {
        if(gameObject.name == "SlashAbility" && SaveManager.singleton != null)
            SaveManager.singleton.weapon1 = true;
    }
    
    void OnDisable()
    {
        if(gameObject.name == "SlashAbility")
        SaveManager.singleton.weapon1 = false;
    }
    
    public IEnumerator ActivateEffect()
    {
        yield return new WaitForSeconds(00.1f);
        ability.ActivateEffect(hitCollider);
        yield return null;
    }
    
    private new void Start()
    {
        base.Start();
        playerAction = caster.GetComponent<PlayerAction>();
        readyColor = GetComponent<SpriteShapeRenderer>().color;
        //Set caster to gameobject called player
        
        
        ability.Initialize(this.gameObject);
        duration = ability.duration;
        
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
                transform.right = new Vector3(playerAction.lastFacing.x, 0, playerAction.lastFacing.y);
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
        
        
        if(spawnPoint != null)
            transform.position = spawnPoint.transform.position;
        else if (!ability.isFollowingCaster)
        {
            transform.position = caster.transform.position;
            transform.right = new Vector3(playerAction.lastFacing.x, 0, playerAction.lastFacing.y);
        }
        
        
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
    }
    
    public void SetCooldown()
    {
        cooldownTime = ability.cooldownTime;
        state = AbilityState.cooldown;
    }

    

    private void Update()
    {
        
        //If Pressing c-key
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartCoroutine(ActivateEffect());
        }
        
        //Change direction of the ability according to where the player is moving
        //if (ability.isFollowingCaster && casterRB.velocity.magnitude > 0)
        //    transform.right = casterRB.velocity;
        
        if (ability.isFollowingCaster)
        {
            //Calculate angle with HelperMethods from the playerAction.lastFacing
            float angle = HelperMethods.GetAngleFromVector(playerAction.lastFacing);
            
            //Set z rotation to angle
            transform.rotation = Quaternion.Euler(0, 0, angle);
            //transform.right = new Vector3(playerAction.lastFacing.x, playerAction.lastFacing.y, 0);
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

    public override void UpgradeOption1()
    {
        base.UpgradeOption1();
        nextAbility = upgradeOption1;
        SaveManager.singleton.weapon1Upgrade1 = true;
        SaveManager.singleton.weapon1Upgrade2 = false;
    }

    public override void UpgradeOption2()
    {
        base.UpgradeOption2();
        nextAbility = upgradeOption2;
        SaveManager.singleton.weapon1Upgrade2 = true;
        SaveManager.singleton.weapon1Upgrade1 = false;
    }

    public override void Downgrade()
    {
        base.Downgrade();
        
        nextAbility = null;
    }
}
