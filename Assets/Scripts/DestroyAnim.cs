using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAnim : MonoBehaviour
{
    [SerializeField] private float countdownDuration;

    
    public Animator animator;
    public GameObject player;
    
    
    private void Awake()
    {
        player = GameObject.Find("Player");
        if (GetComponent<Animator>() != null) animator = GetComponent<Animator>();

        if (animator != null)
        {
            Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
        }
    }
    
    public void DestroyNow()
    {
        Destroy(gameObject);
    }
}
