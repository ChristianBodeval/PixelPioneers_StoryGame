using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu]
public class SlashUpgrade2 : AbilitySO
{
    [Header("Ability Specific")]
    public float range;
    public float timeBetweenEachBounce;
    public int bounces;
    private ChainEffect chainEffect;
    

    public override void Initialize(GameObject obj)
    {
        chainEffect = obj.GetComponent<ChainEffect>();
        chainEffect.range = range;
        chainEffect.timeBetweenEachBounce = timeBetweenEachBounce;
        chainEffect.bounces = bounces;
        chainEffect.damage = damage;
    }

    public override void ActivateEffect(ColliderDrawer colliderDrawer)
    {
        //Pick random target
        //Todo make this an Generic ScriptableObject function
        //Random target

        chainEffect.Activate(colliderDrawer);


    }

}
