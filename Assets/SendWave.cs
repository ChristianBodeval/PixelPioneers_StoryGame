using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendWave : MonoBehaviour
{
    [SerializeField] private WaveObject wave;

    private void OnTriggerEnter2D (Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Debug.Log(true);
            GameObject.Find("EnemyFactory").GetComponent<SpawnSystem>().AddWave(wave);
        }
    }
}
