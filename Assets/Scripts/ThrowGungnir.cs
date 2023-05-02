using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowGungnir : MonoBehaviour
{
    [Header("Gungnir")]
    public GameObject gungnir;

    public IEnumerator gungnirCDCoroutine;
    private Gungnir gungnirScript;
    [HideInInspector] public bool canThrowGungnir = true;
    private float gungnirCD;
    private WeaponCDs weaponCDVisual;
    private PlayerAction playerAction;

    [Header("TriThrow Upgrade")]
    public GameObject triThrow;

    private TriThrow triThrowScript;
    public bool hasUpgrade1;
    public Transform spawn1;
    public Transform spawn2;

    // Start is called before the first frame update
    private void Start()
    {
        gungnirCDCoroutine = GungnirCD();
        gungnirScript = gungnir.GetComponent<Gungnir>();
        triThrowScript = triThrow.GetComponent<TriThrow>();

        weaponCDVisual = GameObject.Find("WeaponCDs").GetComponent<WeaponCDs>();
        playerAction = GameObject.Find("Player").GetComponent<PlayerAction>();
    }

    // Update is called once per frame
    private void Update()
    {
        StartThrow();
        gungnirCD = gungnirScript.CD;
    }

    private void StartThrow()
    {
        if (Input.GetButtonDown("Fire3") && canThrowGungnir)
        {
            Gungnir spear = Instantiate(gungnirScript, playerAction.transform.position, Quaternion.identity);
            spear.SetDirection(playerAction.lastFacing);
            StartCoroutine("GungnirCD");
            Debug.Log("Threw gungnir");
            weaponCDVisual.StartCoroutine("GungnirCD");

            if (hasUpgrade1)
            {
                ThrowTriSpear();
            }
        }
    }

    public IEnumerator GungnirCD()
    {
        canThrowGungnir = false;
        yield return new WaitForSeconds(gungnirCD);
        canThrowGungnir = true;
    }

    public void ThrowTriSpear()
    {
        TriThrow triSpearPref = Instantiate(triThrowScript, playerAction.transform.position, spawn1.rotation);
        triSpearPref.SetDirection1(playerAction.lastFacing);
        TriThrow triSpearPref2 = Instantiate(triThrowScript, playerAction.transform.position, spawn2.rotation);
        triSpearPref2.SetDirection2(playerAction.lastFacing);
    }
}