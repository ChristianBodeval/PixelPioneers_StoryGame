using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private float cameraDistance = -10f;
    private Transform player;
    private float defaultZoomAmount = 5f;
    private bool isLaggingBehind = false;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        defaultZoomAmount = Camera.main.orthographicSize; 
    }

    private void Update()
    {
        SetPositionOnPlayer(); // Camera follows player by default
    }

    private void SetPositionOnPlayer()
    {
        if (isLaggingBehind) return;
        transform.position = new Vector3(player.position.x, player.position.y - 10, cameraDistance); // Follows the player
    }

    public void SetZoomAmount(float zoom)
    {
        Camera.main.orthographicSize = zoom;
    }

    public float GetZoomAmount()
    {
        return Camera.main.orthographicSize;
    }


    public void ResetZoom()
    {
        Camera.main.orthographicSize = defaultZoomAmount;
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
        float dashDuration = player.GetComponent<PlayerAction>().dashDuration;
        float t = 0f;

        while (isLaggingBehind)
        {
            float newX = EaseOutQuad(transform.position.x, player.transform.position.x, t);
            float newY = EaseOutQuad(transform.position.y, player.transform.position.y - 10, t);

            transform.position = new Vector3(newX, newY, cameraDistance);
            prevPos = transform.position;
            t += 0.05f;

            Debug.Log(t);

            yield return new WaitForSeconds(dashDuration / 30);
        }
    }

    public static float EaseOutQuad(float start, float end, float value)
    {
        end -= start;
        return -end * value * (value - 2) + start;
    }
}
