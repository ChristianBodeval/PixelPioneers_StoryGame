using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WeaponCDs : MonoBehaviour
{
    public Image baseMeleeCDVisual;
    public Image dashCDVisual;
    public Image mj�lnirCDVisual;
    public Image gungnirCDVisual;

    public PlayerAction playerActionScript;
    public Mj�lnir mj�lnirScript;

    private float baseMeleeCooldownRemaining;
    private float baseMeleeCooldownTime;

    private float dashCooldownRemaining;
    private float dashCooldownTime;

    private float mj�lnirCooldownRemaining;
    private float mj�lnirCooldownTime;

    private float gungnirCooldownRemaining;
    private float gungnirCooldownTime;

    private void Start()
    {
        dashCDVisual.fillAmount = 1;
        baseMeleeCDVisual.fillAmount = 1;
        mj�lnirCDVisual.fillAmount = 1;
        gungnirCDVisual.fillAmount = 1;

        baseMeleeCooldownTime = playerActionScript.baseMeleeCooldown;
        dashCooldownTime = playerActionScript.dashCooldownTime;
        mj�lnirCooldownTime = mj�lnirScript.chargeCD;
    }

    // Update is called once per frame
    private void Update()
    {
        baseMeleeCDVisual.fillAmount = baseMeleeCooldownRemaining / baseMeleeCooldownTime;
        dashCDVisual.fillAmount = dashCooldownRemaining / dashCooldownTime;
        mj�lnirCDVisual.fillAmount = mj�lnirCooldownRemaining / mj�lnirCooldownTime;
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
    }

    public IEnumerator Mj�lnirCD()
    {
        mj�lnirCooldownRemaining = mj�lnirCooldownTime; // Reset the remaining cooldown time
        while (mj�lnirCooldownRemaining > 0f) // Count down the cooldown time
        {
            mj�lnirCooldownRemaining -= Time.deltaTime;
            yield return null; // Wait for the end of the frame
        }
    }

    //TODO..
    public IEnumerator GungnirCD()
    {
        mj�lnirCooldownRemaining = mj�lnirCooldownTime; // Reset the remaining cooldown time
        while (mj�lnirCooldownRemaining > 0f) // Count down the cooldown time
        {
            mj�lnirCooldownRemaining -= Time.deltaTime;
            yield return null; // Wait for the end of the frame
        }
    }
}