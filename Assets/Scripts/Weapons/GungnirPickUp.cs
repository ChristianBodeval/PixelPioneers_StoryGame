using System.Collections;
using UnityEngine;

public class GungnirPickUp : MonoBehaviour
{
    private Gungnir gungnir;
    private ThrowGungnir throwGungnirScript;
    private WeaponCDs weaponCDs;
    public Collider2D pickUpCollider;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveDistance;
    [SerializeField] private float consumeDistance;
    [SerializeField] private float oppositeDuration;
    private Coroutine coroutine = null;
    private GameObject player;
    private bool isMoving = false;

    private void Start()
    {
        gungnir = GetComponentInParent<Gungnir>();
        throwGungnirScript = GameObject.Find("GungnirThrow").GetComponent<ThrowGungnir>();
        weaponCDs = GameObject.Find("CDs").GetComponent<WeaponCDs>();
        player = GameObject.Find("Player");
    }

    public void ResetCD()
    {
        throwGungnirScript.StopCoroutine("GungnirCD");
        weaponCDs.StopCoroutine("GungnirCD");
        weaponCDs.ResetGungnirCD();
        throwGungnirScript.canThrowGungnir = true;
    }

    private void Update()
    {
        if (Physics2D.CircleCast(transform.position, moveDistance, Vector2.right, moveDistance, LayerMask.GetMask("Player")) && gungnir.canPickUp)
        {
            if (!isMoving)
            {
                isMoving = true;
                coroutine = StartCoroutine(MoveToPlayer());
            }

            // Pickup is close enough to player to be used
            if (Vector3.Distance(player.transform.position, transform.position) <= consumeDistance)
            {
                ResetCD();
                Destroy(gungnir.gameObject);
            }
        }
    }

    // Moves towards player
    private IEnumerator MoveToPlayer()
    {
        Vector3 dir = (player.transform.position - transform.position).normalized;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        float moveOppositeDuration = Time.time + oppositeDuration;

        // Moves away from player briefly
        while (moveOppositeDuration > Time.time)
        {
            rb.velocity = -dir * moveSpeed / 2;
            // Move opposite player dir
            yield return new WaitForSeconds(0.01f);
        }

        // Moves towards player
        while (true)
        {
            dir = player.transform.position - transform.position;
            rb.velocity = dir * moveSpeed + dir.normalized * 5f; // Moves faster when further away
            yield return new WaitForSeconds(0.01f);
        }
    }
}