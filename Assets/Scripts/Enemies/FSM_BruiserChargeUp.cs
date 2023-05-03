using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_BruiserChargeUp : StateMachineBehaviour
{
    Bruiser_Attack ba;
    float waitDuration = 0f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("CanMove", false);
        animator.SetBool("AttackRDY", false);

        ba = animator.gameObject.GetComponentInParent<Bruiser_Attack>();

        waitDuration = Time.time + ba.chargeDelay;
        // TODO Set waitduration from attack script
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (waitDuration < Time.time) animator.SetBool("IsChargedUp", true);

        bool canThrow = ba.WaveUsable(true);

        if (!canThrow)
        {
            animator.SetBool("CanMove", true);
            animator.SetBool("AttackRDY", true);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("IsChargedUp", false);
    }
}
