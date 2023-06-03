using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAnim : MonoBehaviour
{
    [SerializeField] private float countdownDuration;
    private Animator animator;

    private void Start()
    {
        TryGetComponent<Animator>(out animator);
        StartCoroutine(DestroyAfterDuration());
    }

    private IEnumerator DestroyAfterDuration()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        Destroy(gameObject);
    }

    public void DestroyNow()
    {
        Destroy(gameObject);
    }
}
