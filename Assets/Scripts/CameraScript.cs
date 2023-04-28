using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private float cameraDistance = -10f;
    private CinemachineVirtualCamera cinemachineCam;
    private GameObject player;
    private GameObject dash;
    private float defaultZoomAmount = 5f;
    private bool isLaggingBehind = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        dash = GameObject.Find("Dash");


        cinemachineCam = GameObject.FindGameObjectWithTag("CMCamera").GetComponent<CinemachineVirtualCamera>();
        defaultZoomAmount = cinemachineCam.m_Lens.OrthographicSize;
    }

    public void SetZoomAmount(float zoom)
    {
        cinemachineCam.m_Lens.OrthographicSize = zoom;
    }

    public float GetZoomAmount()
    {
        return cinemachineCam.m_Lens.OrthographicSize;
    }


    public void ResetZoom()
    {
        cinemachineCam.m_Lens.OrthographicSize = defaultZoomAmount;
    }

    // Starts coroutine
    public void StartLagBehindPlayer()
    {
        // Only start if it isnt already started
        if (!isLaggingBehind)
        {
            isLaggingBehind = true;
        }
        else
        {
            return;
        }

        StartCoroutine(LagBehind());
    }

    // Stops coroutine
    public void StopLagBehindPlayer()
    {
        isLaggingBehind = false;
    }

    // Eases the camera to from dash start to dash end location
    public IEnumerator LagBehind()
    {
        Vector3 prevPos = transform.position;
        float dashDuration = dash.GetComponent<Dash>().dashDuration;
        float t = 0f;

        while (isLaggingBehind)
        {
            float newX = EaseOutQuad(transform.position.x, player.transform.position.x, t);
            float newY = EaseOutQuad(transform.position.y, player.transform.position.y - 10, t);

            transform.position = new Vector3(newX, newY, cameraDistance);
            prevPos = transform.position;
            t += 0.1f;

            yield return new WaitForSeconds(dashDuration / 30);
        }
    }

    public static float EaseOutQuad(float start, float end, float value)
    {
        end -= start;
        return -end * value * (value - 2) + start;
    }
}
