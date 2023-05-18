using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineAnimator : MonoBehaviour
{
    private LineRenderer lineRenderer;
    
    [SerializeField] 
    private Texture[] textures;
    private int animationStep;
    
    [SerializeField] 
    private float animationSpeed;
    [SerializeField] 
    private float fps = 30f;

    private float fpsCounter;
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // StateUpdate is called once per frame
    void Update()
    {
        fpsCounter += Time.deltaTime;
        if(fpsCounter >= 1f / fps)
        {
            animationStep++;
            if (animationStep >= textures.Length)
                animationStep = 0;
            
            lineRenderer.material.mainTexture = textures[animationStep];
            fpsCounter = 0f;
        }
    }
}
