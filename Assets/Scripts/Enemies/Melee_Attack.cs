using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee_Attack : Enemy_Attack
{
    [Header("SFX")]
    [Range(0f, 1f)] [SerializeField] private float volume;
    [SerializeField] private AudioClip swingSFX;

    [SerializeField] private float attackDMG;
    [SerializeField] private float attackTelegraphTime;

    private void FixedUpdate()
    {
        IsInAttackRange(player); // Player variable is inherited from IEnemyAttack
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

    public override void Attack() // Called from animator
    {
        StartCoroutine(AttackCD(attackCD)); // Starts cooldown for the attack
        StartCoroutine(TelegraphAttack());
    }

    private IEnumerator TelegraphAttack()
    {
        SFXManager.singleton.PlaySound(swingSFX, transform.position, volume);
        yield return new WaitForSeconds(attackTelegraphTime);
        if (Vector3.Distance(player.transform.position, transform.position) <= attackRange) player.GetComponent<PlayerHealth>().TakeDamage(attackDMG); // Deal damage
    }

    private bool IsInAttackRange(GameObject player)
    {
        bool inRange = Vector2.Distance(player.transform.position, transform.position) <= attackRange && IsInLineOfSight(player, animator); // In attack range & los
        animator.SetBool("InAttackRange", inRange);
        return inRange;
    }
}