using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyFromAnimationEvent : MonoBehaviour
{
    private Animator animator;
    private GameObject player;
    
    
    private void Awake()
    {
        player = GameObject.FindWithTag("Player");

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
        else
        {
            animator = GetComponent<Animator>();
            Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
        }
        
        transform.parent = player.transform;
    }
    
    public void DestroyNow()
    {
        Destroy(gameObject);
    }
}
