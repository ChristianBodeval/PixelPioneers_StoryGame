using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyFromAnimationEvent : MonoBehaviour
{
    private Animator animator;
    private GameObject player;
    
    
    private void Awake()
    {
        player = GameObject.Find("Player");
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
        
        transform.parent.parent = player.transform;
        
    }
    
    public void DestroyNow()
    {
        Destroy(gameObject.transform.parent.gameObject);
        
    }
}
