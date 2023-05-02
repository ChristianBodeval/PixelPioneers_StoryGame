using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    
    [SerializeField] private List<IUpgradeable> upgradeableAbilities = new List<IUpgradeable>();

    

    [SerializeField] private List<GameObject> abilityGameObjects = new List<GameObject>();
    [SerializeField] private UpgradeUI upgradeUI;

    public static UpgradeManager instance { get; private set; }
    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        
        abilityGameObjects.ForEach(abilityGameObject =>
        {
            upgradeableAbilities.Add(abilityGameObject.GetComponent<IUpgradeable>());
        });
    }
    
    //Write method for Get list of IUpgradeable
    public List<IUpgradeable> GetUpgradeableAbilities()
    {
        return upgradeableAbilities;
    }
    public List<GameObject> GetAbilitiesGameObjects()
    {
        return abilityGameObjects;
    }
    


    public void UpgradeAbilityOption1(IUpgradeable upgradeable)
    {
        upgradeable.UpgradeOption1();
    }
    
    public void UpgradeAbilityOption2(IUpgradeable upgradeable)
    {
        upgradeable.UpgradeOption2();
    }
    
    
    
    public void Downgrade(IUpgradeable upgradeable)
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

    public void OpenUpgradeUI()
    {
        upgradeUI.OpenUpgradeUI();
    }
}

