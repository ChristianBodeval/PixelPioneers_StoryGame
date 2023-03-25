using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;

[CreateAssetMenu]
public class SlashAbility : Ability
{    
    [Header("Ability specific attributes")]
    public float someVariableDELETE;

    public UnityEvent myEvent;

    public override void ActivateEffect(List<GameObject> targets)
    {
        //TODO Damage all enemy with health.
        foreach (GameObject target in targets)
        {
            Damage(target);
        }
    }

    public void Damage(GameObject target)
    {
        //Deal damage
        Debug.Log("Damaged " + this.damage + " to " + target.name);
    }
}
