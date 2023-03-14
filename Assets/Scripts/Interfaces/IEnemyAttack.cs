using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IEnemyAttack: MonoBehaviour
{
    public float attackRange;
    public float attackCD;
    public bool attackRDY;
    public LayerMask obstacleLayer;
    protected Animator animator;
    protected GameObject player;

    public virtual bool LineOfSight(GameObject target, Animator thisEnemy)
    {
        return !Physics2D.Raycast(thisEnemy.transform.position, target.transform.position - thisEnemy.transform.position, attackRange, obstacleLayer); // Can be overridden - checks if there is no obstacles from enemy to player by default
    }

    public abstract void Attack(); // Must be overwritten

    public virtual IEnumerator AttackCD() // Can be overwritten
    {
        animator.SetBool("AttackRDY", false);

        yield return new WaitForSeconds(attackCD);

        animator.SetBool("AttackRDY", true);
    }
}
