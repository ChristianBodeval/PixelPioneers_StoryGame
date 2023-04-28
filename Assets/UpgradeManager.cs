using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    
    [SerializeField] private List<IUpgradeable> upgradeableAbilities = new List<IUpgradeable>();

    

    [SerializeField] private List<GameObject> abilityGameObjects = new List<GameObject>();
    

    //TODO make this a singlton
    

    private void Awake()
    {
        abilityGameObjects.ForEach(x => upgradeableAbilities.Add(x.GetComponent<IUpgradeable>()));
    }
    
    //Write method for Get list of IUpgradeable
    private List<IUpgradeable> GetUpgradeableAbilities()
    {
        return upgradeableAbilities;
    }

    private void UpgradeAbilityOption1(IUpgradeable upgradeable)
    {
        upgradeable.UpgradeOption1();
    }
    
    private void UpgradeAbilityOption2(IUpgradeable upgradeable)
    {
        upgradeable.UpgradeOption2();
    }
    
    private void Downgrade(IUpgradeable upgradeable)
    {
        upgradeable.Downgrade();
    }


    
    
    //Only for testing
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GetUpgradeableAbilities()[1].UpgradeOption1();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GetUpgradeableAbilities()[1].UpgradeOption2();
            
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GetUpgradeableAbilities()[1].Downgrade();
        }
    }
}

