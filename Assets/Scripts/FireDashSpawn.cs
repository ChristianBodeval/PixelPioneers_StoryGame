using System;
using System.Collections;
using UnityEngine;

public class FireDashSpawn : MonoBehaviour
{
    public GameObject firePrefab;
    public Transform playerTransform;
    private Dash dash;
    public float spawnDuration = 0.3f;

    private void Awake()
    {
        playerTransform = GameObject.Find("Player").transform;
    }

    // Start is called before the first frame update
    private void Start()
    {
        dash = GetComponent<Dash>();
    }

    public void SpawnFire()
    {
        if (dash.isDashing == true)
        {
            GameObject fireSpawn = Instantiate(firePrefab, playerTransform.position, Quaternion.identity);
        }
    }
}