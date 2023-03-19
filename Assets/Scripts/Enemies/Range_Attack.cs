using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Range_Attack : Enemy_Attack
{
    public GameObject projectile;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float attackDMG;
    public new float attackCD;

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

    /// <summary>
    /// 
    /// </summary>
    public override void Attack()
    {
        StartCoroutine(AttackCD(attackCD)); // Starts cooldown for the attack

        Vector3 direction = player.transform.position - transform.position; // Direction of player
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Angle for pointing to player

        GameObject newProjectile = Instantiate(projectile, transform.position + (direction).normalized * 0.5f, Quaternion.AngleAxis(angle, Vector3.forward)); // Spawn projectile

        newProjectile.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed; // Make projectile move
    }

    private bool InAttackRange(GameObject player)
    {
        bool inRange = Vector2.Distance(player.transform.position, transform.position) <= attackRange && LineOfSight(player, animator); // In attack range & los
        animator.SetBool("InAttackRange", inRange);
        return inRange;
    }
}
