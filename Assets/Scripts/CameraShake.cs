using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float duration = 0.5f;
    public bool takesDamage;
    public AnimationCurve curve;
    private bool shakeIsOnCD = false;

    // Update is called once per frame
    private void Update()
    {
       
    }

    public void TakesDamage()
    {
        StartCoroutine("Shaking");
        StartCoroutine("ShakeCD");
    }

    private IEnumerator Shaking()
    {
        if (!shakeIsOnCD)
        {
            Vector3 originalPos = transform.position;

            float elapsed = 0.0f;

            while (elapsed < duration)
            {
                originalPos = transform.position;
                elapsed += Time.deltaTime;
                float strength = curve.Evaluate(elapsed / duration);
                transform.position = originalPos + Random.insideUnitSphere * strength;
                yield return null;
            }

            transform.localPosition = originalPos;
        }
    }

    private IEnumerator ShakeCD()
    {
        shakeIsOnCD = true;
        yield return new WaitForSeconds(1f);
        shakeIsOnCD = false;
    }
}