using System;
using System.Collections;
using UnityEngine;

public class ThrowGungnir : MonoBehaviour, IUpgradeable
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

        Vector2 movementDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
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
        TriThrow triSpearPref = Instantiate(triThrowScript, playerAction.transform.position, Quaternion.identity);
        triSpearPref.SetDirection(playerAction.lastFacing, -45f);
        TriThrow triSpearPref2 = Instantiate(triThrowScript, playerAction.transform.position, Quaternion.identity);
        triSpearPref2.SetDirection(playerAction.lastFacing, 45f);

        Vector2 lastFacing = playerAction.lastFacing;

        //triSpearPref.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(lastFacing.y, lastFacing.x) * Mathf.Rad2Deg + triSpearPref.angleOfSpear);
        //triSpearPref2.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(lastFacing.y, lastFacing.x) * Mathf.Rad2Deg + triSpearPref2.angleOfSpear);

        // Set the spear's velocity to make it move in the desired direction
        //triSpearPref.GetComponent<Rigidbody2D>().velocity = playerAction.lastFacing.normalized * triThrowScript.speed;
    }

    public void UpgradeOption1()
    {
        throw new NotImplementedException();
    }

    public void UpgradeOption2()
    {
        throw new NotImplementedException();
    }

    public void Downgrade()
    {
        throw new NotImplementedException();
    }
}