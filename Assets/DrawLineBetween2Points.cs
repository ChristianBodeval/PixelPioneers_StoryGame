using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLineBetween2Points : MonoBehaviour
{
    [SerializeField] private float startWidth;
    [SerializeField] private float endWidth;
    [SerializeField] private Color lineColor;



    [Header("Assign two objects and press a key while in playmode")]
    public bool isTesting;
    public GameObject testObject1;
    public GameObject testObject2;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void OnValidate()
    {
        if (lineRenderer != null)
        {
            lineRenderer.startWidth = startWidth;
            lineRenderer.endWidth = endWidth;    
        }
    }

    public void SetLine(Vector3 startPosition, Vector3 endPosition)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
    }

    public void AddLine(Vector3 startPosition, Vector3 endPosition)
    {
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, startPosition);
        lineRenderer.SetPosition(lineRenderer.positionCount, endPosition);
    }


    public void ResetLine()
    {
        lineRenderer.positionCount = 0;
    }


    private void Update()
    {
        if (Input.anyKeyDown && isTesting)
        {
            SetLine(testObject1.transform.position, testObject2.transform.position);
        }
    }




}
