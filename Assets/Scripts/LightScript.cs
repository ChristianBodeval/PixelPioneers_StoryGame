using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightScript : MonoBehaviour
{
    public float duration = 4f;
    public void SetLighDuration(float duration)
    {
        this.duration = duration;
    }
    public void SetLightIntensityOverTime(float targetIntensity)
    {
        StartCoroutine(ChangeLightIntensity(targetIntensity, duration));
        
    }

    private IEnumerator ChangeLightIntensity(float targetIntensity, float duration)
    {
        float startIntensity = GetComponent<Light2D>().intensity;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            GetComponent<Light2D>().intensity = Mathf.Lerp(startIntensity, targetIntensity, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        GetComponent<Light2D>().intensity = targetIntensity; // Ensure the final intensity is set accurately
    }
}