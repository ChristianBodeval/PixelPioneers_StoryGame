using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu]
public class SlashUpgrade2 : Ability
{
    public float range;
    public float timeBetweenEachBounce;
    public int numOfBounces;

    public void ActivateEffect(List<GameObject> targets)
    {
        //Pick random target
        int randomNumber = Random.Range(1, targets.Count);

        GameObject target = targets[randomNumber];

        Debug.Log("SlashUpgrade1 activated");

        ChainDamage(target, 3f, 0.1f, 3);
    }

    IEnumerator ChainDamage(GameObject startingTarget, float range, float timeBetweenEachBounce, int bounces)
    {
        Damage(startingTarget);

        for (int i = 0; i < bounces; i++)
        {

            //Find next target
            RaycastHit2D[] newTargets = Physics2D.CircleCastAll(startingTarget.transform.position, range, startingTarget.transform.up);

            //Guard clause, stop bouncing, if no one is in range
            if (newTargets.Length == 0)
            {
                yield return null;
            }

            int randomNumber = Random.Range(1, newTargets.Length);

            startingTarget = newTargets[randomNumber].transform.gameObject;

            Damage(startingTarget);

            yield return new WaitForSeconds(timeBetweenEachBounce);
        }
        yield return null;
    }

    public void Damage(GameObject target)
    {
        //Deal damage
        Debug.Log("Damaged " + this.damage + " to " + target.name);
    }
}
