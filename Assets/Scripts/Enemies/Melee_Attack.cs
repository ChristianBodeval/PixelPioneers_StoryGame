using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee_Attack : Enemy_Attack
{
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

    public override void Attack()
    {
        StartCoroutine(AttackCD()); // Starts cooldown for the attack

        // ** Dmg player function - potentially after the attack anim is done to give more reaction time to the player
    }

    public override IEnumerator AttackCD() // Can be overwritten
    {
        animator.SetBool("AttackRDY", false);

        yield return new WaitForSeconds(attackCD);

        animator.SetBool("AttackRDY", true);
    }

    private bool InAttackRange(GameObject player)
    {
        bool inRange = Vector2.Distance(player.transform.position, transform.position) <= attackRange && LineOfSight(player, animator); // In attack range & los
        animator.SetBool("InAttackRange", inRange);
        return inRange;
    }
}