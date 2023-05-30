using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine.Utility;
using UnityEngine;

public class OverTimeEffect : Effect
{
    private float timer;
    public float tickEveryXSeconds;
    public float duration;
    public float damagePrTick;
    
    public GameObject lightningEffect;
    
    public override void Activate(ColliderDrawer colliderDrawer)
    {
        StartCoroutine(EffectCoroutine(colliderDrawer));
        
        /*
        Quaternion rotation = colliderDrawer.transform.rotation;
        //Add 90 degrees to the rotation
        rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y, rotation.eulerAngles.z);
        
        Vector3 position = colliderDrawer.transform.position;

        
        transform.gameObject.transform.rotation = rotation;
        */
        //Instantiate(lightningEffect, position, rotation);
    }


    private void Update()
    {
        //TODO This is a quick fix needs to be updated
        Quaternion transformRotation = transform.gameObject.transform.rotation;
        
        Debug.Log(transformRotation.y);
        if (transformRotation.y == 1)
            transformRotation.eulerAngles = new Vector3(0, 0, 180);
        else if (transformRotation.y > 0 && transformRotation.y < 90)
            transformRotation.eulerAngles = new Vector3(0, 0, 270);
        else if (transformRotation.y < 0)
            transformRotation.eulerAngles = new Vector3(0, 0, 90);
        
        
        transform.gameObject.transform.rotation = transformRotation;
    }

    private List<GameObject> targets;
    public override IEnumerator EffectCoroutine(ColliderDrawer colliderDrawer)
    {
        timer = Time.time + duration;
        
        lightningEffect.SetActive(true);
        
        while (Time.time < timer)
        {
            //Damage all targets
            Debug.Log("Casted 1 tick");
            //TODO Set to tick pr seconds

            targets = colliderDrawer.targets;
            
            foreach (GameObject target in targets)
            {
                target.GetComponent<Health>().TakeDamage(damagePrTick);
            }
            yield return new WaitForSeconds(tickEveryXSeconds);
        }
        
        lightningEffect.SetActive(false);
        yield return null;
    }
}
