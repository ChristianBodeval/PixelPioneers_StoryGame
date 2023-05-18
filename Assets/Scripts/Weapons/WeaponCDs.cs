using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WeaponCDs : MonoBehaviour
{
    public Image baseMeleeCDVisual;
    public Image dashCDVisual;
    public Image mjoelnirCDVisual;
    public Image gungnirCDVisual;

    public Dash dashScript;
    public Mjoelnir mjoelnirScript;
    public Gungnir gungnirScript;

    public GameObject eyeCatcherMjoelnir;
    public GameObject eyeCatcherDash;
    public GameObject eyeCatcherGungnir;

    private float baseMeleeCooldownRemaining;
    private float baseMeleeCooldownTime;

    private float dashCooldownRemaining;
    private float dashCooldownTime;

    private float mjoelnirCooldownRemaining;
    private float mjoelnirCooldownTime;

    private float gungnirCooldownRemaining;
    private float gungnirCooldownTime;

    private void Start()
    {
        dashCDVisual.fillAmount = 1;
        baseMeleeCDVisual.fillAmount = 1;
        mjoelnirCDVisual.fillAmount = 1;
        gungnirCDVisual.fillAmount = 1;

        dashScript = GameObject.Find("Dash").GetComponent<Dash>();
        mjoelnirScript = GameObject.Find("Mjoelnir").GetComponent<Mjoelnir>();
        gungnirScript = GameObject.Find("GungnirThrow").GetComponent<Gungnir>();

        //TODO Fix this - Christian ability system (Get info from Ability SO's instead)
        //baseMeleeCooldownTime = playerActionScript.baseMeleeCooldown;
        /*Cheated for now: */
        baseMeleeCooldownTime = 4f;
        dashCooldownTime = dashScript.dashCooldownTime;
        mjoelnirCooldownTime = mjoelnirScript.chargeCD;
        gungnirCooldownTime = gungnirScript.CD;
    }

    // Update is called once per frame
    private void Update()
    {
        baseMeleeCDVisual.fillAmount = baseMeleeCooldownRemaining / baseMeleeCooldownTime;
        dashCDVisual.fillAmount = dashCooldownRemaining / dashCooldownTime;
        mjoelnirCDVisual.fillAmount = mjoelnirCooldownRemaining / mjoelnirCooldownTime;
        gungnirCDVisual.fillAmount = gungnirCooldownRemaining / gungnirCooldownTime;
    }

    public IEnumerator BaseMeleeCD()
    {
        baseMeleeCooldownRemaining = baseMeleeCooldownTime; // Reset the remaining cooldown time
        while (baseMeleeCooldownRemaining > 0f) // Count down the cooldown time
        {
            baseMeleeCooldownRemaining -= Time.deltaTime;
            yield return null; // Wait for the end of the frame
        }
    }

    public IEnumerator DashCD()
    {
        dashCooldownRemaining = dashCooldownTime; // Reset the remaining cooldown time
        while (dashCooldownRemaining > 0f) // Count down the cooldown time
        {
            dashCooldownRemaining -= Time.deltaTime;
            yield return null; // Wait for the end of the frame
        }
        if (dashCooldownRemaining <= 0f)
        {
            eyeCatcherDash.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            eyeCatcherDash.SetActive(false);

        }
    }

    public IEnumerator MjoelnirCD()
    {
        mjoelnirCooldownRemaining = mjoelnirCooldownTime; // Reset the remaining cooldown time
        while (mjoelnirCooldownRemaining > 0f) // Count down the cooldown time
        {
            mjoelnirCooldownRemaining -= Time.deltaTime;
            yield return null; // Wait for the end of the frame
        }
        if (mjoelnirCooldownRemaining <= 0f)
        {
            eyeCatcherMjoelnir.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            eyeCatcherMjoelnir.SetActive(false);

        }
    }

    public IEnumerator GungnirCD()
    {
        gungnirCooldownRemaining = gungnirCooldownTime; // Reset the remaining cooldown time
        while (gungnirCooldownRemaining > 0f) // Count down the cooldown time
        {
            gungnirCooldownRemaining -= Time.deltaTime;
            yield return null; // Wait for the end of the frame
        }
        if (gungnirCooldownRemaining <= 0f)
        {
            eyeCatcherGungnir.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            eyeCatcherGungnir.SetActive(false);

        }
    }

    public void ResetGungnirCD()
    {
        gungnirCooldownRemaining = 0f;
        gungnirCDVisual.fillAmount = 0f;
        StartCoroutine("SetGungnirEyecatcher");
    }
    private IEnumerator SetGungnirEyecatcher()
    {
        eyeCatcherGungnir.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        eyeCatcherGungnir.SetActive(false);

    }
}
