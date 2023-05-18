using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeAttack : MonoBehaviour
{
    private void Start()
    {
        Invoke("Destroy", 0.15f);
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}
