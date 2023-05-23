using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Range_Attack : Enemy_Attack
{
    [Header("SFX")]
    [Range(0f, 1f)] [SerializeField] private float volume;
    [SerializeField] private AudioClip fireSFX;

    public GameObject projectile;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float attackDMG;

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
        SFXManager.singleton.PlaySound(fireSFX, transform.position, volume);

        StartCoroutine(AttackCD(attackCD)); // Starts cooldown for the attack

        Vector3 direction = (player.transform.position - transform.position).normalized; // Direction of player
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Angle for pointing to player

        GameObject newProjectile = Pool.pool.DrawFromProjectilePool();
        newProjectile.transform.position = transform.position + 0.2f * direction;
        newProjectile.transform.eulerAngles = new Vector3(-45f, newProjectile.transform.eulerAngles.y, newProjectile.transform.eulerAngles.z); // Angle -45 degrees on x axis and point towards camera

        newProjectile.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed; // Make projectile move
    }

    private bool InAttackRange(GameObject player)
    {
        bool inRange = Vector2.Distance(player.transform.position, transform.position) <= attackRange && IsInLineOfSight(player, animator); // In attack range & los
        animator.SetBool("InAttackRange", inRange);
        return inRange;
    }
}
