using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WeaponCDs : MonoBehaviour
{
    public Image baseMeleeCDVisual;
    public Image dashCDVisual;
    public Image mjoelnirCDVisual;
    public Image gungnirCDVisual;

    public SlashAbility meleeScriptableObject;
    public Dash dashScript;
    public Mjoelnir mjoelnirScript;
    public ThrowGungnir gungnirScript;

    public GameObject eyeCatcherMelee;
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

    
    //TODO Der er meget kode der gaar igen her, burde laves mere generisk
    //TODO Scriptet boer tage hoejde for at der kan vaere forskellig cooldown paa hver upgrade, goer det ikke lige nu
    //TODO Scriptet boer virke pr. runtime, saa skifter vi ability bliver cooldownTime opdateret ud fra hvilken upgrade man har, goer det ikke lige nu
    
    
    // Generic singleton pattern
    public static WeaponCDs Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    

    private void Start()
    {
        dashCDVisual.fillAmount = 1;
        baseMeleeCDVisual.fillAmount = 1;
        mjoelnirCDVisual.fillAmount = 1;
        gungnirCDVisual.fillAmount = 1;
            
        //Find the SlashAbility scriptable object in the resources folder
        //meleeScriptableObject = Resources.Load<SlashAbility>("ScriptableObjects/AbilitiesSO's/MeleeAttack.asset");
        Debug.Log("MeleeSO:" + meleeScriptableObject);
        dashScript = GameObject.Find("Dash").GetComponent<Dash>();
        mjoelnirScript = GameObject.Find("Mjoelnir").GetComponent<Mjoelnir>();
        gungnirScript = GameObject.Find("GungnirThrow").GetComponent<ThrowGungnir>();
        
        baseMeleeCooldownTime = meleeScriptableObject.cooldownTime;
        dashCooldownTime = dashScript.dashCooldownTime;
        mjoelnirCooldownTime = mjoelnirScript.chargeCD;
        gungnirCooldownTime = gungnirScript.CD;
    }

    // StateUpdate is called once per frame
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
