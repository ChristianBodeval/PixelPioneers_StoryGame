using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Hermes_Attack : StateMachineBehaviour
{
    //float waitDuration = 0.15f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    { 
        animator.gameObject.GetComponentInParent<Hermes_Attack>().ThrowWave();
        //waitDuration += Time.time;
    }

    /*
    // OnStateUpdate is called on each StateUpdate frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (waitDuration > Time.time) return;

        animator.SetBool("IsChargedUp", true);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("IsChargedUp", false);
    }
    */
}