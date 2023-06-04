using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CurrentUpgrade
{
    None,
    Upgrade1,
    Upgrade2
}

public class Ability : MonoBehaviour, IUpgradeable
{
    private float dashCooldownTime;

    private CurrentUpgrade currentUpgrade;
    
    //Make a enum with Upgrade1 and Upgrade2

    protected void Start()
    {
        currentUpgrade = CurrentUpgrade.None;
        
        PlayerPrefs.DeleteAll();
        Downgrade();
        
        if (PlayerPrefs.GetInt(abilitySO.name+1) == 1)
        {
            //Upgrade from the childclass' IUpgradeable upgrade 1
            UpgradeOption1();
        }
        
        if (PlayerPrefs.GetInt(abilitySO.name+2) == 1)
        {
            //Upgrade from the childclass' IUpgradeable upgrade 1
            UpgradeOption2();
        }
    }
    
    
    void Update()
    {
        //If n key is pressed
        if (Input.GetKeyDown(KeyCode.N))
        {
            //Downgrade
            Downgrade();
        }
    }


   


    //Listen for GetComponent<IUpgradeable>().UpgradeOption1() with a UnityEvent
    
    [SerializeField] public AbilitySO abilitySO;
    [SerializeField] public List<UpgradeSO> upgrades;
    
    public List<UpgradeSO> GetUpgrades()
    {
        return upgrades;
    }
    
    public AbilitySO GetAbilitySO()
    {
        return abilitySO;
    }


    public virtual void UpgradeOption1()
    {
        currentUpgrade = CurrentUpgrade.Upgrade1;
    }

    public virtual void UpgradeOption2()
    {
        currentUpgrade = CurrentUpgrade.Upgrade2;
    }

    public string GetAbilityName()
    {
        return abilitySO.name;
    }

    
    
    public virtual void Downgrade()
    {
        currentUpgrade = CurrentUpgrade.None;
    }
}
