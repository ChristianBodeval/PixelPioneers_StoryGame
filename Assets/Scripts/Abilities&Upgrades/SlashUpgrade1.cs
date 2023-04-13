using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu]
public class SlashUpgrade1 : Ability
{
    public float tickEveryXSeconds;

    OverTimeEffect effect;

    public override void Initialize(GameObject obj)
    {
        effect = obj.GetComponent<OverTimeEffect>();
        effect.tickEveryXSeconds = tickEveryXSeconds;
        effect.duration = duration;
    }

    public override void ActivateEffect(List<GameObject> targets)
    {
        effect.Activate(targets);
    }
}
