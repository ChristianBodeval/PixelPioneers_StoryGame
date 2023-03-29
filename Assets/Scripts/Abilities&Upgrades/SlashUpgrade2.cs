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

    public GameObject myPrefab;

    public DrawLineBetween2Points lineDrawer;




    public void ActivateEffect(List<GameObject> targets)
    {
        //Pick random target
        for (int i = 0; i < targets.Count; i++)
        {
            Damage(targets[i]);

            if (i < targets.Count - 1)
                lineDrawer.SetLine(targets[i].transform.position, targets[i + 1].transform.position);
        }

    }

    

    public void Damage(GameObject target)
    {
        //Deal damage
        Debug.Log("Damaged " + this.damage + " to " + target.name);
    }
}
