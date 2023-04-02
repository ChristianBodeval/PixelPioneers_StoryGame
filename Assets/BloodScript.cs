using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodScript : MonoBehaviour
{
    public float alphaReductionInterval;
    private SpriteRenderer sr;
    private Color32 color;

    private void Start()
    {
        StartCoroutine(ReduceAlpha());
    }

    private void OnEnable()
    {
        StartCoroutine(ReduceAlpha());
    }

    private void OnDisable()
    {
        sr.color = color;
    }

    private IEnumerator ReduceAlpha()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();

        color = sr.color;
        byte alpha = color.a;

        while (alpha > 1)
        {
            yield return new WaitForSeconds(alphaReductionInterval);
            alpha -= 1;
            sr.color = new Color32(color.r, color.g, color.b, alpha);
        }

        Pool.pool.ReturnToBloodPool(gameObject);
    }
}
