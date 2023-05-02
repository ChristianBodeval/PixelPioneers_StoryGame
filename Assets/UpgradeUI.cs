using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class UpgradeUI : MonoBehaviour
{
    private List<ISelectable> currentChoises = new List<ISelectable>();

    private ISelectable currentAbility;


    private IUpgradeable selectedAbility;
    
    private int currentSelectedNumber;
    private bool isUpgradeUIOpen;
    
    public Material outlineMaterial;
    
    [SerializeField] private List<UpgradeHolder> upgrades = new List<UpgradeHolder>();
    [SerializeField] private GameObject upgradeUI;

    [SerializeField] private List<AbilityHolder_UI> abilitiesUIs = new List<AbilityHolder_UI>();
    [SerializeField] private GameObject abilityUI;
    
    
    
    List<GameObject> abilityGameObjects = new List<GameObject>();
    private bool isNextToAnvil;


    public void OpenUpgradeUI()
    {
        this.gameObject.SetActive(true);
        for (int i = 0; i < abilityGameObjects.Count; i++)
        {
            abilitiesUIs[i].abilitySO = abilityGameObjects[i].GetComponent<Ability>().GetAbilitySO();
        }
        
        abilityGameObjects.AddRange(UpgradeManager.instance.GetAbilitiesGameObjects());
        UpdateAbilityUI();
        
        
        SetIsUpgradeUIOpen(false);
        currentAbility = currentChoises[0];
        currentAbility.SetOutline(true);
    }
   
    
    void UpdateAbilityUI()
    {
        for (int i = 0; i < abilityGameObjects.Count; i++)
        {
            abilitiesUIs[i].abilitySO = abilityGameObjects[i].GetComponent<Ability>().GetAbilitySO();
        }
    }

    void SelectLeft()
    {
        currentAbility.SetOutline(false);
        currentSelectedNumber--;
        currentAbility = currentChoises[currentSelectedNumber];
        currentAbility.SetOutline(true);
    }
    
    //Select right ability
    void SelectRight()
    {
        currentAbility.SetOutline(false);
        currentSelectedNumber++; 
        currentAbility = currentChoises[currentSelectedNumber];
        currentAbility.SetOutline(true);
    }
    
    void SelectAbility()
    {
        List<UpgradeSO> abilityUpgradeSOs = abilityGameObjects[currentSelectedNumber].GetComponent<Ability>().GetUpgrades();
        
        upgrades[0].upgradeSO = abilityUpgradeSOs[0];
        upgrades[1].upgradeSO = abilityUpgradeSOs[1];

        selectedAbility = abilityGameObjects[currentSelectedNumber].GetComponent<IUpgradeable>();
        
        UpdateAbilityUI();

        SetIsUpgradeUIOpen(true);
    }

    void SelectUpgrade()
    {
        if (currentSelectedNumber == 0)
        {
            UpgradeManager.instance.UpgradeAbilityOption1(selectedAbility);
            Debug.Log("Called upgrade 1");
        }
        if (currentSelectedNumber == 1)
        {
            UpgradeManager.instance.UpgradeAbilityOption2(selectedAbility);
        }
    }
    
    void SetIsUpgradeUIOpen(bool value)
    {
        upgradeUI.SetActive(value);
        isUpgradeUIOpen = value;
        abilityUI.SetActive(!value);
        
        
        currentChoises.Clear();
        
        if(value)
        {
            currentChoises.AddRange(upgrades);
        }
        else
        {
            currentChoises.AddRange(abilitiesUIs);
        }
        
        currentSelectedNumber = 0;
        currentAbility = currentChoises[0];
        currentAbility.SetOutline(true);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && currentSelectedNumber > 0)
            SelectLeft();
        if (Input.GetKeyDown(KeyCode.D) && currentSelectedNumber < currentChoises.Count - 1)
            SelectRight();
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isUpgradeUIOpen)
                SelectAbility();
            else
                SelectUpgrade();
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isUpgradeUIOpen)
                ExitUpgrade();
            else
                ExitAnvil();
        }
    }
    
    void ExitUpgrade()
    {
        SetIsUpgradeUIOpen(false);
        currentAbility = currentChoises[0];
        currentAbility.SetOutline(true);
    }
    void ExitAnvil()
    {
        SetIsUpgradeUIOpen(false);
        abilityUI.SetActive(false);
        currentChoises.Clear();
        currentAbility.SetOutline(false);
        currentAbility = null;
        this.gameObject.SetActive(false);
    }

    
}


