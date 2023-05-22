using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Charge : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("CanMove", false);
        animator.gameObject.GetComponentInParent<Crowd_Control>().RemoveStun();
    }

    //OnStateUpdate is called on each StateUpdate frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!animator.GetBool("IsStunned")) animator.gameObject.GetComponentInParent<Charger_Attack>().Charge();
    }
}
