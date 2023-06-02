using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;


[CreateAssetMenu]
public class SlashAbility : AbilitySO
{
    public GameObject prefab;
    
    private WeaponCDs weaponCDs;

    private bool isSwingingFromLeftToRight;
    
    public override void ActivateEffect(ColliderDrawer colliderDrawer)
    {
        
        WeaponCDs.Instance.StartCoroutine(WeaponCDs.Instance.BaseMeleeCD());
        
        
        
        Quaternion rotation = colliderDrawer.transform.rotation;
        //Add 90 degrees to the rotation
        rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y, rotation.eulerAngles.z);
        
        Vector3 position = colliderDrawer.transform.position;

        
        
        GameObject instance = Instantiate(prefab, position, rotation);
        
        Animator animator = instance.GetComponentInChildren<Animator>();
        
        //animator.Play("Cone_Attack", -1, 1f);
        
        //animator.Play("Cone_Attack", -1, -1f);
        
        //Play it reversed
        
        
        //Stop current animation playing

        
        //
        
        
        //Play the animation called "Slash_RightToLeft"
        // animator.Play("Slash_RightToLeft", -1, 1f);
        
        if(isSwingingFromLeftToRight)
            animator.SetTrigger("RightSwing");
        else 
            animator.SetTrigger("LeftSwing");
        
        isSwingingFromLeftToRight = !isSwingingFromLeftToRight;
        
        /*
        //Play the animation called "Slash_RightToLeft"
        if (isSwingingFromLeftToRight)
        {
            animator.Play("Slash_RightToLeft", -1, 1f);
            
        }

        else
        {
            animator.Play("Cone_Attack", -1, 1f);
        }
        isSwingingFromLeftToRight = !isSwingingFromLeftToRight;
        
        */
        //animator.speed = -1f;
        
        //If there is no targets, return
        if (colliderDrawer.targets.Count == 0) 
            return;


        //TODO Damage all enemy with health.
        Debug.Log(("Instantiate"));
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
