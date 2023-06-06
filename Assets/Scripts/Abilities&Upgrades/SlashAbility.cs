using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;


[CreateAssetMenu]
public class SlashAbility : AbilitySO
{
    [Header("SFX")]
    [Range(0, 1)] public float sfxVolume = 1f;
    [SerializeField] private AudioClip slashSFX;

    public GameObject prefab;

    private bool isSwingingFromLeftToRight;
    
    
    
    public override void ActivateEffect(ColliderDrawer colliderDrawer)
    {
        isFollowingCaster = true;
        canChangeColors = true;
        
        
        //If weaponCDs is enabled
        if (WeaponCDs.Instance.gameObject.activeSelf)
            WeaponCDs.Instance.StartCoroutine(WeaponCDs.Instance.BaseMeleeCD());
        
        Quaternion rotation = colliderDrawer.transform.rotation;

        rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y, rotation.eulerAngles.z);
        
        Vector3 position = colliderDrawer.transform.position;
        GameObject instance = Instantiate(prefab, position, rotation);

        SFXManager.singleton.PlaySound(slashSFX, position, sfxVolume);
        
        Animator animator = instance.GetComponentInChildren<Animator>();
        
        if(isSwingingFromLeftToRight)
            animator.SetTrigger("RightSwing");
        else 
            animator.SetTrigger("LeftSwing");
        
        isSwingingFromLeftToRight = !isSwingingFromLeftToRight;

        //If there is no targets, return
        if (colliderDrawer.targets.Count == 0) 
            return;


        //TODO Damage all enemy with health.
        foreach (GameObject target in colliderDrawer.targets)
        {
            Damage(target);
        }
    }

    public void Damage(GameObject target)
    {
        target.GetComponent<Health>().TakeDamage(damage);
        //Deal damage
        Debug.Log("Damaged " + this.damage + " to " + target.name);
    }

    
}
