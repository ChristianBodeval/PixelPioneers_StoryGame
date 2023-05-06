using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu]
public class SlashUpgrade1 : AbilitySO
{
    public float tickEveryXSeconds;

    OverTimeEffect effect;

    public override void Initialize(GameObject obj)
    {
        effect = obj.GetComponent<OverTimeEffect>();
        effect.tickEveryXSeconds = tickEveryXSeconds;
        effect.duration = duration;
        effect.damagePrTick = damage;
    }

    public override void ActivateEffect(ColliderDrawer colliderDrawer)
    {
        effect.Activate(colliderDrawer);
    }
}
