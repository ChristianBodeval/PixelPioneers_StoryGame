using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;


[CreateAssetMenu]
public class SlashAbility : AbilitySO
{
    public GameObject prefab;
    
    public override void ActivateEffect(ColliderDrawer colliderDrawer)
    {
        Quaternion rotation = colliderDrawer.transform.rotation;
        //Add 90 degrees to the rotation
        rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y, rotation.eulerAngles.z);
        
        Vector3 position = colliderDrawer.transform.position;
        
        Instantiate(prefab, position, rotation);
        //If there is no targets, return
        if (colliderDrawer.targets.Count == 0) 
            return;


        //TODO Damage all enemy with health.
        Debug.Log(("INstantiate"));
        foreach (GameObject target in colliderDrawer.targets)
        {
            Damage(target);
            
            //spawn prefab
        }
    }

    public void Damage(GameObject target)
    {
        target.GetComponent<Health>().TakeDamage(damage);
        //Deal damage
        Debug.Log("Damaged " + this.damage + " to " + target.name);
    }

    
}
