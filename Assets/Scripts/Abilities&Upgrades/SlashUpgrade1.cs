using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu]
public class SlashUpgrade1 : Ability
{
    public float range;
    public float timeBetweenEachBounce;
    public int numOfBounces;

    public override void ActivateEffect(List<GameObject> targets)
    {
        Debug.Log("Targets: " + targets);

        //TODO Damage all enemy with health.
        foreach (GameObject target in targets)
        {
            Damage(target);
        }
    }

    public override void BeginCooldown(GameObject caster)
    {
        
        
    }


    public void Damage(GameObject target)
    {
        //Deal damage
        Debug.Log("Damaged " + this.damage + " to " + target.name);
    }
}
