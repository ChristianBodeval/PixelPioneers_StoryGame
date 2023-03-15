using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeAttack : MonoBehaviour
{
    PolygonCollider2D coneAttack;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OntriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            Destroy(other.gameObject);
        }
    }
}
