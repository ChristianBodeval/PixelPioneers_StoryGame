using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy_Attack : MonoBehaviour
{
    public float attackRange;
    public float attackCD = 0.2f;
    public LayerMask obstacleLayer;
    protected Animator animator;
    protected GameObject player;
    [HideInInspector] public Coroutine attackCDCoroutine;

    private void FixedUpdate()
    {
        InAttackRange(player); // Player variable is inherited from IEnemyAttack
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponentInChildren<Animator>();
        animator.SetBool("AttackRDY", true); // Make sure we can attack
    }

    private void OnEnable()
    {
        if (animator != null) { animator.SetBool("AttackRDY", true); } // Resets variable when respawning
    }


    public virtual void Attack()
    {
        if (attackCDCoroutine != null) StopCoroutine(attackCDCoroutine);
        attackCDCoroutine = StartCoroutine(AttackCD()); // No cooldown was given
    }

    public virtual void Attack(float cooldown) // Default of -1
    {
        if (attackCDCoroutine != null) StopCoroutine(attackCDCoroutine);
        attackCDCoroutine = StartCoroutine(AttackCD(cooldown)); // Cooldown was given
    }

    public void StartAttackCD()
    {
        if (attackCDCoroutine != null) StopCoroutine(attackCDCoroutine);
        attackCDCoroutine = StartCoroutine(AttackCD()); // Starts cooldown for the attack
    }

    public virtual IEnumerator AttackCD(float cooldown = 0.2f) // Can be overwritten
    {
        animator.SetBool("AttackRDY", false);

        yield return new WaitForSeconds(cooldown);

        animator.SetBool("AttackRDY", true);
    }

    private bool InAttackRange(GameObject player)
    {
        bool inRange = Vector2.Distance(player.transform.position, transform.position) <= attackRange && LineOfSight(player, animator); // In attack range & los
        animator.SetBool("InAttackRange", inRange);
        return inRange;
    }

    public virtual bool LineOfSight(GameObject target, Animator thisEnemy)
    {
        return !Physics2D.Raycast(thisEnemy.transform.position, target.transform.position - thisEnemy.transform.position, attackRange, obstacleLayer); // Can be overridden - checks if there is no obstacles from enemy to player by default
    }
}
