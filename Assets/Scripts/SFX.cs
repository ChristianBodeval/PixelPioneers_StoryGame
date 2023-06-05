using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour
{
    private AudioSource audioSource;
    private float volume = 0f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        volume = audioSource.volume;
    }

    private void FixedUpdate()
    {
        if (audioSource != null) audioSource.volume = volume * SFXManager.masterVolume;
    }

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
