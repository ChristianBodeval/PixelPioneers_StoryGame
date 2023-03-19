using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Range_Attack : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetBool("IsStunned") || animator.GetBool("CannotTransitionState") || animator.GetBool("IsFleeing")) { return; } // Guard clause
        animator.SetBool("AttackRDY", false);
        //animator.SetBool("CanMove", false);
        animator.GetComponentInParent<Range_Attack>().Attack(); // Starts cooldown for the attack
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!animator.GetBool("AttackRDY") || animator.GetBool("IsStunned") || animator.GetBool("CannotTransitionState")) { return; } // Guard clause
        animator.SetBool("AttackRDY", false);
        //animator.SetBool("CanMove", false);
        animator.GetComponentInParent<Range_Attack>().Attack(); // Starts cooldown for the attack
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
