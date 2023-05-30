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
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }
    
    public void DestroyNow()
    {
        Destroy(gameObject);
    }
}
