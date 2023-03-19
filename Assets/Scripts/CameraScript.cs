using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private float cameraDistance = -10f;
    private Transform player;
    private float defaultZoomAmount = 5f;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
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
}
