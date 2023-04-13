using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverTimeEffect : Effect
{
    private float timer;
    public float tickEveryXSeconds;
    public float duration;
    
    public override void Activate(List<GameObject> targets)
    {
        StartCoroutine(EffectCoroutine(targets));
    }

    public override IEnumerator EffectCoroutine(List<GameObject> targets)
    {
        timer = Time.time + duration;
        while (Time.time < timer)
        {
            //Damage all targets
            Debug.Log("Casted");
            //TODO Set to tick pr seconds
            yield return new WaitForSeconds(tickEveryXSeconds);
        }
        yield return null;
    }
}
