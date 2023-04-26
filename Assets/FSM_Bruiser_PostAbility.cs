using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Bruiser_PostAbility : StateMachineBehaviour
{
    Bruiser_Attack ba;
    float waitDuration = 0f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ba = animator.gameObject.GetComponentInParent<Bruiser_Attack>();

        waitDuration = Time.time + ba.postFireDelay;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (waitDuration < Time.time)
        {
            animator.SetBool("CanMove", true);
            animator.SetBool("AttackRDY", true);
        }
    }
}
