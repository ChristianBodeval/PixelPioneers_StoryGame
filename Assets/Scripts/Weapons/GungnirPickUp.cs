using System.Collections;
using UnityEngine;

public class GungnirPickUp : MonoBehaviour
{
    private Gungnir gungnir;
    private ThrowGungnir throwGungnirScript;
    private WeaponCDs weaponCDs;
    public Collider2D pickUpCollider;
    private void Start()
    {
        gungnir = GetComponentInParent<Gungnir>();
        throwGungnirScript = GameObject.Find("GungnirThrow").GetComponent<ThrowGungnir>();
        weaponCDs = GameObject.Find("CD's").GetComponent<WeaponCDs>();
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
        throwGungnirScript.StopCoroutine("GungnirCD");
        weaponCDs.StopCoroutine("GungnirCD");
        weaponCDs.ResetGungnirCD();
        throwGungnirScript.canThrowGungnir = true;
    }

    
}