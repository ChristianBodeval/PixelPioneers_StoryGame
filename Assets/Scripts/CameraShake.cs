using Cinemachine;
using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float duration = 0.5f;
    public bool isShakingOnDamage = true;
    public float shakeCooldown = 0.5f;
    public AnimationCurve diminishCurve;
    public AnimationCurve amplifyCurve;
    private bool shakeIsOnCD = false;
    private Coroutine shakeCoroutine;

    private CinemachineVirtualCamera CMCam;



    private void Start()
    {
        CMCam = GameObject.Find("CM vcam").GetComponent<CinemachineVirtualCamera>();

    }

    public void TakesDamage()
    {
        if (!isShakingOnDamage) return; // Guard clause

        StartCoroutine( Shaking() );
        StartCoroutine("ShakeCD");
    }

    public void ShakeCamera(bool isDiminishing = true, float shakeAmplitude = 0f)
    {
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine); 
        shakeCoroutine = StartCoroutine( Shaking(isDiminishing, shakeAmplitude) );
    }

    private IEnumerator Shaking(bool isDiminishing = true, float shakeAmplitude = 0f)
    {
        CinemachineBasicMultiChannelPerlin CBMCP = CMCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>(); // Camera shake component

        if (!shakeIsOnCD)
        {
            //Vector3 originalPos = transform.position;

            float elapsed = 0.0f;

            while (elapsed < duration)
            {
                //originalPos = transform.position;
                elapsed += Time.deltaTime;

                float strength = (isDiminishing) ? DimishingCurve(elapsed) : AmplifyingCurve(elapsed, shakeAmplitude); // Sets the curve to diminishing or amplifying in strength over time
                CBMCP.m_AmplitudeGain = strength; // Set noise amount, which is the same as a shake
                yield return null;
            }

            CBMCP.m_AmplitudeGain = 0; // Remove shake
            //transform.localPosition = originalPos;
        }
    }

    private float AmplifyingCurve(float elapsedTime, float shakeAmplitude)
    {
        float temp = amplifyCurve.Evaluate(elapsedTime);
        temp = (temp > 1f) ? 1f : temp; // If over 1 set to 1
        return temp * shakeAmplitude;
    }

    private float DimishingCurve(float elapsedTime)
    {
        float temp = diminishCurve.Evaluate(elapsedTime);
        temp = (temp > 1f) ? 1f : temp; // If over 1 set to 1
        return temp;
    }

    private IEnumerator ShakeCD()
    {
        shakeIsOnCD = true;
        yield return new WaitForSeconds(shakeCooldown);
        shakeIsOnCD = false;
    }
}