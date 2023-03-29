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
    float cooldownTime;
    private float duration;

    private float timeCasted;
    List<GameObject> targets;

    //TODO make an event for ready,active and cooldown and make the ColliderDrawer listen for them
    public Color readyColor;
    public Color activeColor;
    public Color cooldownColor;

    public AbilityHolder nextAbility;

    public DrawLineBetween2Points lineRenderer;

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


        if (ability.followCaster)
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
        //TODO tight solution, should instead be Ability specific
        chainDamageCoroutine = null;
        effectOverTimeCoroutine = null;

        cooldownTime = ability.cooldownTime;
        state = AbilityState.cooldown;
    }


    public IEnumerator EffectOverTime()
    {
        while (Time.time < timeCasted)
        {
            ability.ActivateEffect(collider.targets);
            Debug.Log("Casted");
            //TODO Set to tick pr seconds
            yield return new WaitForSeconds(1f);
        }
        SetCooldown();
        yield return null;
    }






    GameObject myGM;
    float myradius = 0;
    public Collider2D[] mytargets;
    private void OnDrawGizmos()
    {
        if(myGM != null)
            Gizmos.DrawWireSphere(myGM.transform.position + myGM.transform.right * 0, myradius);
    }

    IEnumerator ChainDamage(GameObject startingTarget, float range, float timeBetweenEachBounce, int bounces)
    {

        myradius = range;

        Debug.Log("ChainDamge");
        ability.ActivateEffect(startingTarget);

        Collider2D[] newTargets;

        GameObject newTarget;

        int randomNumber;

        for (int i = 0; i < bounces; i++)
        {


            myGM = startingTarget;

            //Cast circle to tind targets


            newTargets = null;

            //TODO Add layermask
            newTargets = Physics2D.OverlapCircleAll(startingTarget.transform.position, range,LayerMask.GetMask("Enemy"));

            mytargets = newTargets;

            Debug.Log("Target Length: " + newTargets.Length);

            if (newTargets.Length == 0)
            {
                break;
            }




            randomNumber = Random.Range(0, newTargets.Length);
            newTarget = newTargets[randomNumber].transform.gameObject;

            //Debug.Log("Current target:" + startingTarget.GetInstanceID() + " && New target:" + newTarget.GetInstanceID());


            

            for (int counter = 0; counter < 10; counter++)
            {
                if (newTarget.GetInstanceID() != startingTarget.GetInstanceID())
                    break;
                randomNumber = Random.Range(0, newTargets.Length);
                newTarget = newTargets[randomNumber].transform.gameObject;                
            }
            

            if (startingTarget.GetInstanceID() == newTarget.GetInstanceID())
            {
                break;
                Debug.Log("THEY ARE THE SAME");
            }


            lineRenderer.SetLine(startingTarget.transform.position, newTarget.transform.position);

            ability.ActivateEffect(startingTarget);

            startingTarget = newTarget;


            yield return new WaitForSeconds(timeBetweenEachBounce);

            
        }

        lineRenderer.ResetLine();
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

                //else if(ability.triggerType == Ability.TriggerType.OnEvent && ability.activateEvent != null)
                //    SetActive();

                break;

                
            case AbilityState.active:

                Debug.Log("Activated");



                //Effect type
                if (nextAbility != null)
                {
                    nextAbility.SetActive();
                }


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

                    chainDamageCoroutine = ChainDamage(targets[randomNum], 2f, 1f, 20);
                    StartCoroutine(chainDamageCoroutine);
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
