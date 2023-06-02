using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SweepingDash : MonoBehaviour
{
    public GameObject slashCollider;

    public AbilityHolder abilityHolder;

    public bool isTurnedOn;

    private void Start()
    {
        //Set abilityHolder to gameobject called SlashAbility
        abilityHolder = GameObject.Find("SlashAbility").GetComponent<AbilityHolder>();
        
        
    }
    // Make a function that calls AbilityHolder.ActivateEffect()

    void ActivateMelee()
    {
        StartCoroutine(abilityHolder.ActivateEffect());
    }

    //Make a Coroutines that calls AbilityHolder.ActivateEffect() 

    public void TurnAreaOn()
    {

        if (isTurnedOn == false)
        {
            isTurnedOn = true;
            
            if(abilityHolder == null)
                abilityHolder = GameObject.Find("Player").GetComponentInChildren<AbilityHolder>();
        
            InvokeRepeating("ActivateMelee", 0f, 0.05f);
        }
    } 
    public void TurnAreaOff()
    {
        isTurnedOn = false;
        CancelInvoke();
    }
}
