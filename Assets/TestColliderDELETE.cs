using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestColliderDELETE : MonoBehaviour
{
    public List<GameObject> targets = new List<GameObject>();


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!targets.Contains(collision.gameObject))
        {
            targets.Add(collision.gameObject);
            Debug.Log("Added " + gameObject.name);
            Debug.Log("GameObjects in list: " + targets.Count);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (targets.Contains(collision.gameObject))
        {
            targets.Remove(collision.gameObject);
            Debug.Log("Removed " + collision.name);
            Debug.Log("GameObjects in list: " + targets.Count);
        }

    }

    private void Update()
    {
        if(Input.anyKey)
        {

            transform.Translate(new Vector3(20, 2, 0) - transform.position);
            //transform.position = new Vector3(20, 2, 0) - transform.position;
        }
    }
}
