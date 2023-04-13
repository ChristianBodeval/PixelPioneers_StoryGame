using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WeaponCDs : MonoBehaviour
{
    public Image baseMeleeCDVisual;
    public Image dashCDVisual;
    public Image mjølnirCDVisual;
    public Image gungnirCDVisual;

    public PlayerAction playerActionScript;
    public Mjölnir mjølnirScript;

    private float baseMeleeCooldownRemaining;
    private float baseMeleeCooldownTime;

    private float dashCooldownRemaining;
    private float dashCooldownTime;

    private float mjölnirCooldownRemaining;
    private float mjölnirCooldownTime;

    private float gungnirCooldownRemaining;
    private float gungnirCooldownTime;

    private void Start()
    {
        dashCDVisual.fillAmount = 1;
        baseMeleeCDVisual.fillAmount = 1;
        mjølnirCDVisual.fillAmount = 1;
        gungnirCDVisual.fillAmount = 1;

        baseMeleeCooldownTime = playerActionScript.baseMeleeCooldown;
        dashCooldownTime = playerActionScript.dashCooldownTime;
        mjölnirCooldownTime = mjølnirScript.chargeCD;
    }

    // Update is called once per frame
    private void Update()
    {
        baseMeleeCDVisual.fillAmount = baseMeleeCooldownRemaining / baseMeleeCooldownTime;
        dashCDVisual.fillAmount = dashCooldownRemaining / dashCooldownTime;
        mjølnirCDVisual.fillAmount = mjölnirCooldownRemaining / mjölnirCooldownTime;
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

    public IEnumerator MjölnirCD()
    {
        mjölnirCooldownRemaining = mjölnirCooldownTime; // Reset the remaining cooldown time
        while (mjölnirCooldownRemaining > 0f) // Count down the cooldown time
        {
            mjölnirCooldownRemaining -= Time.deltaTime;
            yield return null; // Wait for the end of the frame
        }
    }

    //TODO..
    public IEnumerator GungnirCD()
    {
        mjölnirCooldownRemaining = mjölnirCooldownTime; // Reset the remaining cooldown time
        while (mjölnirCooldownRemaining > 0f) // Count down the cooldown time
        {
            mjölnirCooldownRemaining -= Time.deltaTime;
            yield return null; // Wait for the end of the frame
        }
    }
}