using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCoroutineScriptDELETE : MonoBehaviour
{
    // Start is called before the first frame update
    float myFloat;

    void Start()
    {
        myFloat = Time.time + 30f;
        StartCoroutine(myCoroutine());
    }


    IEnumerator myCoroutine()
    {
        while (myFloat > Time.time)
        {
            Debug.Log("OK");
            yield return new WaitForSeconds(1f);
        }
        yield return null;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
