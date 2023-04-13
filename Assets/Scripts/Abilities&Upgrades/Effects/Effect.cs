using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public virtual void Activate(List<GameObject> targets)
    {
        StartCoroutine(EffectCoroutine(targets));
    }

    public virtual IEnumerator EffectCoroutine(List<GameObject> targets)
    {
        yield return null;
    }
}
