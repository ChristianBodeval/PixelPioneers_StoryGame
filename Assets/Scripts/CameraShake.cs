using Cinemachine;
using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float duration = 0.5f;
    public bool takesDamage;
    public AnimationCurve curve;
    public AnimationCurve smallCurve;
    private bool shakeIsOnCD = false;

    private CinemachineVirtualCamera CMCam;



    private void Start()
    {
        CMCam = GameObject.Find("CM vcam").GetComponent<CinemachineVirtualCamera>();

    }
    // Update is called once per frame
    private void Update()
    {
       
    }

    public void TakesDamage()
    {
        StartCoroutine("Shaking");
        StartCoroutine("ShakeCD");
    }
    public void SmallShake()
    {
        StartCoroutine("SmallShaking");
        StartCoroutine("ShakeCD");
    }

    private IEnumerator Shaking()
    {
        CinemachineBasicMultiChannelPerlin CBMCP = CMCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (!shakeIsOnCD)
        {
            Vector3 originalPos = transform.position;

            float elapsed = 0.0f;

            while (elapsed < duration)
            {
                originalPos = transform.position;
                elapsed += Time.deltaTime;
                float strength = curve.Evaluate(elapsed / duration);
                CBMCP.m_AmplitudeGain = strength;
                yield return null;
            }

            transform.localPosition = originalPos;
        }
    }
    private IEnumerator SmallShaking()
    {
        CinemachineBasicMultiChannelPerlin CBMCP = CMCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (!shakeIsOnCD)
        {
            Vector3 originalPos = transform.position;

            float elapsed = 0.0f;

            while (elapsed < duration)
            {
                originalPos = transform.position;
                elapsed += Time.deltaTime;
                float strength = curve.Evaluate(elapsed / duration);
                CBMCP.m_AmplitudeGain = strength;
                yield return null;
            }

            transform.localPosition = originalPos;
        }
    }

    private IEnumerator ShakeCD()
    {
        shakeIsOnCD = true;
        yield return new WaitForSeconds(0.5f);
        shakeIsOnCD = false;
    }
}