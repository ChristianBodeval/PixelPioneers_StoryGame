using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ChainEffect : Effect
{
    public float damage;
    public float range;
    public float timeBetweenEachBounce;
    public int bounces;

    //For testing
    GameObject myGM;
    float myradius = 0;
    private Collider2D[] mytargets;
    //TODO Make line renderer
    //private DrawLineBetween2Points lineRenderer;

    IEnumerator enumerator;

    private void Awake()
    {
        //lineRenderer = GetComponent<DrawLineBetween2Points>();   
    }

    private void OnDrawGizmos()
    {
        if (myGM != null)
            Gizmos.DrawWireSphere(myGM.transform.position + myGM.transform.right * 0, myradius);
    }

    public override void Activate(List<GameObject> targets)
    {
        GameObject target = targets[Random.Range(0, targets.Count)];
        enumerator = ChainDamage(target);
        StartCoroutine(enumerator);
    }
        

    public IEnumerator ChainDamage(GameObject startingTarget)
    {
        myradius = range;
        Debug.Log("ChainDamge");
        //TODO Deal damage to target
        Collider2D[] newTargets;
        GameObject newTarget;
        int randomNumber;

        for (int i = 0; i < bounces; i++)
        {
            myGM = startingTarget;

            newTargets = null;
            newTargets = Physics2D.OverlapCircleAll(startingTarget.transform.position, range, LayerMask.GetMask("Enemy"));

            mytargets = newTargets;

            if (newTargets.Length == 0)
            {
                break;
            }

            randomNumber = Random.Range(0, newTargets.Length);
            newTarget = newTargets[randomNumber].transform.gameObject;

            for (int counter = 0; counter < 10; counter++)
            {
                if (newTarget.GetInstanceID() != startingTarget.GetInstanceID())
                    break;
                randomNumber = Random.Range(0, newTargets.Length);
                newTarget = newTargets[randomNumber].transform.gameObject;
            }

            //TODO make this an event
            //lineRenderer.SetLine(startingTarget.transform.position, newTarget.transform.position);


            //TODO Deal damage here too

            startingTarget = newTarget;

            yield return new WaitForSeconds(timeBetweenEachBounce);

        }
        //lineRenderer.ResetLine();

        yield return null;
    }
}
