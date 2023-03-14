using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crowd_Control : MonoBehaviour
{
    private Coroutine coroutine;
    private Animator animator;
    private float remainingStunDuration = 0f;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (remainingStunDuration > 0f)
        {
            remainingStunDuration -= Time.deltaTime; // Tick down remaining duration of stun, this is for later comparison of durations
        }
    }

    public void Stun()
    {
        animator.SetBool("IsStunned", true);
    }

    public void Stun(float duration)
    {
        if (duration > remainingStunDuration) // Make sure to remove a previous stun if a new one is applied and is longer than the older one
        {
            if (coroutine != null) { StopCoroutine(coroutine); }    // Replace old stun if new is longer
            coroutine = StartCoroutine(StunDuration(duration));     // Start the stun duration
            remainingStunDuration = duration;
        }
    }

    private IEnumerator StunDuration(float duration)
    {
        animator.SetBool("IsStunned", true);

        yield return new WaitForSeconds(duration);

        animator.SetBool("IsStunned", false);
    }

    private void OnDisable()
    {
        animator.SetBool("IsStunned", false);
        coroutine = null;
    }
}
