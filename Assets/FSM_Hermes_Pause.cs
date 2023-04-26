using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Hermes_Pause : StateMachineBehaviour
{
    Hermes_Attack script;
    float waitDuration = 0f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        script = animator.gameObject.GetComponentInParent<Hermes_Attack>();

        waitDuration = Time.time + script.postFireDelay;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (waitDuration > Time.time) return;

        animator.SetBool("IsChargedUp", true);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("IsChargedUp", false);
        animator.SetBool("CanMove", true);
        animator.GetComponentInParent<Hermes_Pathing>().StopWaitingOnFlee();
    }
}

