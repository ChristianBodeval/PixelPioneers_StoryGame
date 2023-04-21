using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public virtual void Activate(ColliderDrawer colliderDrawer)
    {
        StartCoroutine(EffectCoroutine(colliderDrawer));
    }

    public virtual IEnumerator EffectCoroutine(ColliderDrawer colliderDrawer)
    {
        yield return null;
    }
}
