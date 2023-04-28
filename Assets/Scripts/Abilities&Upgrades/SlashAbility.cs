using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu]

public class SlashAbility : Ability
{
    public override void ActivateEffect(AbilityHolder ability, List<GameObject> targets)
    {
        //TODO Damage all enemy with health.
        foreach (GameObject target in targets)
        {
            
            Damage(target);
        }
    }

    public void Damage(GameObject target)
    {
        target.GetComponent<Health>().TakeDamage(damage);
        //Deal damage
        Debug.Log("Damaged " + this.damage + " to " + target.name);
    }
}
