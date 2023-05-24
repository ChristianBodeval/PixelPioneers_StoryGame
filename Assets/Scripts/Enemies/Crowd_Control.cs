using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;

public class Crowd_Control : MonoBehaviour
{
    public bool isStunImmune = false;
    private Coroutine stunCoroutine;
    private Coroutine freezeCoroutine;
    private Animator animator;
    private float remainingStunDuration = 0f;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        if (animator == null) return;

        animator.SetBool("CannotTransitionState", false);
        animator.SetBool("IsStunned", false);
        animator.SetBool("CanMove", true);
        animator.SetBool("AttackRDY", true);
    }

    private void Update()
    {
        if (remainingStunDuration > 0f)
        {
            remainingStunDuration -= Time.deltaTime; // Tick down remaining duration of stun, this is for later comparison of durations
        }
    }

    public void Die()
    {
        animator.SetBool("CannotTransitionState", true);
        animator.SetBool("IsStunned", true);
        animator.Play("Base Layer.Stunned");
    }

    public void Stun()
    {
        if (isStunImmune) { return; } // Guard clause

        if (stunCoroutine != null) StopCoroutine(stunCoroutine);
        if (freezeCoroutine != null) StopCoroutine(freezeCoroutine);

        animator.SetBool("CannotTransitionState", true);
        animator.SetBool("IsStunned", true);
        animator.Play("Base Layer.Stunned");
    }

    public void RemoveStun()
    {
        if (stunCoroutine != null) StopCoroutine(stunCoroutine); // Replace old stun if new is longer
        if (freezeCoroutine != null) StopCoroutine(freezeCoroutine);
        animator.SetBool("CannotTransitionState", false);
        animator.SetBool("IsStunned", false);
        animator.SetBool("AttackRDY", true);
        animator.SetBool("CanMove", true);
    }

    public void Stun(float duration)
    {
        if (isStunImmune) { return; } // Guard clause

        if (freezeCoroutine != null) StopCoroutine(freezeCoroutine);

        if (duration > remainingStunDuration) // Make sure to remove a previous stun if a new one is applied and is longer than the older one
        {
            if (stunCoroutine != null) StopCoroutine(stunCoroutine); // Replace old stun if new is longer
            stunCoroutine = StartCoroutine(StunDuration(duration)); // Start the stun duration
            remainingStunDuration = duration;
        }
    }

    private IEnumerator StunDuration(float duration)
    {
        animator.SetBool("CannotTransitionState", true);
        animator.SetBool("IsStunned", true);
        animator.Play("Base Layer.Stunned");

        yield return new WaitForSeconds(duration);

        animator.SetBool("CannotTransitionState", false);
        animator.SetBool("IsStunned", false);
    }

    public void FreezeFrame(float duration)
    {
        if (isStunImmune) return;
        if (remainingStunDuration < duration) RemoveStun();
        if (freezeCoroutine != null) StopCoroutine(freezeCoroutine);
        freezeCoroutine = StartCoroutine(FreezeFrameCoroutine(duration));
    }

    public IEnumerator FreezeFrameCoroutine(float duration)
    {
        if (isStunImmune) yield break; // Guard clause

        bool alreadyStunned = animator.GetBool("IsStunned");

        animator.speed = 0f;
        if (!alreadyStunned)
        {
            Stun();
        }

        yield return new WaitForSeconds(duration);

        if (!alreadyStunned)
        {
            RemoveStun();
        }
        animator.speed = 1f;
    }

    private void OnDisable()
    {
        animator.SetBool("CannotTransitionState", false);
        RemoveStun();
        if (stunCoroutine != null) StopCoroutine(stunCoroutine);
        if (freezeCoroutine != null) StopCoroutine(freezeCoroutine);
        animator.speed = 1f;
    }
}
