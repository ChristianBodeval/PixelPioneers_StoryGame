using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private GameObject player;

    void Start()
    {
        player = GameObject.Find("Player");
    }

    void FixedUpdate()
    {
        transform.position = player.transform.position + (Vector3)Vector2.up * 8.7f;
    }
}
