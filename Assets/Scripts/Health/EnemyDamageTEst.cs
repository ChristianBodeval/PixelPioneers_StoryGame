using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageTEst : MonoBehaviour
{
    public float EnemyDamage = 10f;
    public GameObject playerHealth;

    // Start is called before the first frame update
    void Start()
    {

    }

    // StateUpdate is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerHealth.GetComponent<Health>().TakeDamage(EnemyDamage);
        }
    }
}
