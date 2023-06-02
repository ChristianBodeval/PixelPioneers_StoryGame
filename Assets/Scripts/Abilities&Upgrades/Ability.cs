using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CurrentUpgrade
{
    Upgrade1,
    Upgrade2
}

public class Ability : MonoBehaviour
{
    private float dashCooldownTime;
    
    //Make a enum with Upgrade1 and Upgrade2

    protected void Start()
    {
        
        if (PlayerPrefs.GetInt(abilitySO.name+1) == 1)
        {
            //Upgrade from the childclass' IUpgradeable upgrade 1
            GetComponent<IUpgradeable>().UpgradeOption1();
        }
        
        if (PlayerPrefs.GetInt(abilitySO.name+2) == 1)
        {
            //Upgrade from the childclass' IUpgradeable upgrade 1
            GetComponent<IUpgradeable>().UpgradeOption2();
        }
    }


    private CurrentUpgrade currentUpgrade;
    
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
    

    public string GetAbilityName()
    {
        return abilitySO.name;
    }


    public void Downgrade()
    {
        throw new System.NotImplementedException();
    }
}
