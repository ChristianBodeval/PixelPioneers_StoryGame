using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour, IUpgradeable
{
    
    //Make a enum with Upgrade1 and Upgrade2
    public enum CurrentUpgrade
    {
        Upgrade1,
        Upgrade2
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


    public void UpgradeOption1()
    {
        currentUpgrade = CurrentUpgrade.Upgrade1;
    }

    public void UpgradeOption2()
    {
        currentUpgrade = CurrentUpgrade.Upgrade2;
    }

    public void Downgrade()
    {
        throw new System.NotImplementedException();
    }
}
