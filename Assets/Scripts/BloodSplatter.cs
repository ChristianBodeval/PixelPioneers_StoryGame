using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSplatter : MonoBehaviour
{
    ParticleSystem ps;

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
        ps.Play();
        Invoke("ReturnToPool", ps.main.duration);
    }

    private void OnEnable()
    {
        if (ps == null) return; // Guard clause
        ps.Play();
        Invoke("ReturnToPool", ps.main.duration);
    }

    private void ReturnToPool()
    {
        Pool.pool.ReturnToBloodSplatterPool(gameObject);
    }
}
