using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SweepingDash : MonoBehaviour
{
    public GameObject slashCollider;

    private AbilityHolder abilityHolder;

    private bool isTurnedOn;

    [SerializeField] private float slashEachSeconds = 0.1f;
    private void Start()
    {
        abilityHolder = GameObject.Find("SlashAbility").GetComponent<AbilityHolder>();
    }
    
    void ActivateMelee()
    {
        StartCoroutine(abilityHolder.ActivateEffect());
    }
    
    public void TurnAreaOn()
    {

        if (isTurnedOn == false)
        {
            isTurnedOn = true;
            
            if(abilityHolder == null)
                abilityHolder = GameObject.Find("Player").GetComponentInChildren<AbilityHolder>();
        
            InvokeRepeating("ActivateMelee", 0f, slashEachSeconds);
        }
    } 
    public void TurnAreaOff()
    {
        isTurnedOn = false;
        CancelInvoke();
    }
}
