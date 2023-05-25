using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour
{
    public void ReturnToPool(float duration)
    {
        Invoke("Return", duration);
    }

    private void Return()
    {
        gameObject.transform.parent = null;
        Pool.pool.ReturnToSFXPool(gameObject);
    }
}
