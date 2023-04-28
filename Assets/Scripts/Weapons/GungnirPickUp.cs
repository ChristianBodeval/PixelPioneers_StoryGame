using System.Collections;
using UnityEngine;

public class GungnirPickUp : MonoBehaviour
{
    private Gungnir gungnir;
    private PlayerAction playerAction;
    private WeaponCDs weaponCDs;
    public Collider2D pickUpCollider;
    private void Start()
    {
        gungnir = GetComponentInParent<Gungnir>();
        playerAction = GameObject.Find("Player").GetComponent<PlayerAction>();
        weaponCDs = GameObject.Find("WeaponCDs").GetComponent<WeaponCDs>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && gungnir.canPickUp)
        {
            ResetCD();
            Destroy(gungnir.gameObject);
        }
    }

    public void ResetCD()
    {
        playerAction.StopCoroutine("GungnirCD");
        weaponCDs.StopCoroutine("GungnirCD");
        weaponCDs.ResetGungnirCD();
        playerAction.canThrowGungnir = true;
    }

    
}