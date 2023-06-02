using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAnim : MonoBehaviour
{
    [SerializeField] private float countdownDuration;

    private void Start()
    {
        player = GameObject.Find("Player");
        if (GetComponent<Animator>() != null) animator = GetComponent<Animator>();

        if (animator != null)
        {
            Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
        }
    }

    private IEnumerator DestroyAfterDuration()
    {
        yield return new WaitForSeconds(countdownDuration);

        Destroy(gameObject);
    }
}
