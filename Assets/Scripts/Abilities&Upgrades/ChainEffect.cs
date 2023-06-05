using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ChainEffect : Effect
{
    [Header("SFX")]
    [Range(0, 1)] public float sfxVolume = 1f;
    [SerializeField] private AudioClip chainSFX;

    [HideInInspector] public float damage;
    [HideInInspector] public float range;
    [HideInInspector] public float timeBetweenEachBounce;
    [HideInInspector] public int bounces;
    
    private DrawLineBetween2Points lineRenderer;

    IEnumerator enumerator;

    private void Awake()
    {
        lineRenderer = GetComponent<DrawLineBetween2Points>();   
    }

    public override void Activate(ColliderDrawer colliderDrawer)
    {
        GameObject target = colliderDrawer.targets[Random.Range(0, colliderDrawer.targets.Count)];
        enumerator = ChainDamage(target);
        StartCoroutine(enumerator);
    }
        

    public IEnumerator ChainDamage(GameObject startingTarget)
    {
        Collider2D[] newTargets;
        GameObject newTarget;
        int randomNumber;
        
        
        
        for (int i = 0; i < bounces; i++)
        {
            newTargets = Physics2D.OverlapCircleAll(startingTarget.transform.position, range, LayerMask.GetMask("Enemy"));
            
            startingTarget.GetComponent<Health>().TakeDamage(damage);
            SFXManager.singleton.PlaySound(chainSFX, startingTarget.transform.position, sfxVolume);

            if (newTargets.Length == 0)
            {
                break;
            }

            randomNumber = Random.Range(0, newTargets.Length);
            
            
            
            
            newTarget = newTargets[randomNumber].transform.gameObject;

            
            
            for (int counter = 0; counter < 10; counter++)
            {
                if (newTarget == null) {
                    continue;
                }
                if (newTarget.GetInstanceID() != startingTarget.GetInstanceID())
                    break;
                randomNumber = Random.Range(0, newTargets.Length);
                
                newTarget = newTargets[randomNumber].transform.gameObject;
            }
            
            lineRenderer.SetLine(startingTarget.transform.position, newTarget.transform.position);

            if (startingTarget.Equals(newTarget))
                break;
                
            startingTarget = newTarget;

            yield return new WaitForSeconds(timeBetweenEachBounce);

        }
        
        lineRenderer.ResetLine();
    
        yield return null;
    }
}
