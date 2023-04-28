using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverTimeEffect : Effect
{
    private float timer;
    public float tickEveryXSeconds;
    public float duration;
    public float damagePrTick;
    
    public override void Activate(ColliderDrawer colliderDrawer)
    {
        StartCoroutine(EffectCoroutine(colliderDrawer));
    }

    private List<GameObject> targets;
    public override IEnumerator EffectCoroutine(ColliderDrawer colliderDrawer)
    {
        timer = Time.time + duration;
        
        while (Time.time < timer)
        {
            //Damage all targets
            Debug.Log("Casted 1 tick");
            //TODO Set to tick pr seconds

            targets = colliderDrawer.targets;
            
            foreach (GameObject target in targets)
            {
                target.GetComponent<Health>().TakeDamage(damagePrTick);
            }
            yield return new WaitForSeconds(tickEveryXSeconds);
        }
        yield return null;
    }
}
