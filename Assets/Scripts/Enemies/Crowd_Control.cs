using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;

public class Crowd_Control : MonoBehaviour
{
    public bool isStunImmune = false;
    private Coroutine coroutine;
    private Animator animator;
    private float remainingStunDuration = 0f;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
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
        if (isStunImmune) { return; } // Guard clause

        animator.SetBool("IsStunned", true);
    }

    public void RemoveStun()
    {
        animator.SetBool("IsStunned", false);
    }

    public void Stun(float duration)
    {
        if (isStunImmune) { return; } // Guard clause

        if (duration > remainingStunDuration) // Make sure to remove a previous stun if a new one is applied and is longer than the older one
        {
            if (coroutine != null) { StopCoroutine(coroutine); coroutine = null; }    // Replace old stun if new is longer
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

    public IEnumerator FreezeFrame(float duration)
    {
        animator.speed = 0f;
        animator.SetBool("CannotTransitionState", true);
        Stun();

        yield return new WaitForSeconds(duration);

        animator.SetBool("CannotTransitionState", false);
        animator.speed = 1f;
        RemoveStun();
        //animator.Play("Idle");
    }

    private void OnDisable()
    {
        animator.SetBool("CannotTransitionState", false);
        RemoveStun();
        coroutine = null;
        animator.speed = 1f;
    }
}
