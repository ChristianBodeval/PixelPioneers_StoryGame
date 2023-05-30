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
    private bool isAbilityUIOpen;
    
    
    public Material outlineMaterial;
    
    [SerializeField] private List<UpgradeHolder> upgrades = new List<UpgradeHolder>();
    [SerializeField] private GameObject upgradeUI;

    [FormerlySerializedAs("abilitiesUIs")] [SerializeField] private List<AbilityHolder_UI> abilitiesUis = new List<AbilityHolder_UI>();
    [SerializeField] private GameObject abilityUI;
    
    [SerializeField] private PlayerAction playerActions;
    [SerializeField] private Rigidbody2D playerRigidbody;
    
    public List<GameObject> abilityGameObjects = new List<GameObject>();

    public int progressNumber;
    public GameObject playerDash;

    public void Awake()
    {
        SetAbilitiesProgess(this.progressNumber);
        ExitAnvil();
    }
    
    //Set all abilities to the scriptable object unknown which is above a int. The int can maximum be 4.
    public void SetAbilitiesProgess(int progressNumber)
    {
        this.progressNumber = progressNumber;
        for (int i = 0; i < abilityGameObjects.Count; i++)
        {
            abilitiesUis[i].SetActive(i < progressNumber);
        }
        
        
    }

    public void OpenUpgradeUI()
    {
        
        
        this.gameObject.SetActive(true);
        for (int i = 0; i < abilityGameObjects.Count; i++)
        {
            abilitiesUis[i].abilitySO = abilityGameObjects[i].GetComponent<Ability>().GetAbilitySO();
        }
        
        abilityGameObjects.AddRange(UpgradeManager.instance.GetAbilitiesGameObjects());
        //UpdateAbilityUI();
        SetAbilitiesProgess(progressNumber);
        playerActions.enabled = false;
        playerDash.SetActive(false);
        
        
        playerRigidbody.velocity = Vector2.zero;
        playerRigidbody.bodyType = RigidbodyType2D.Kinematic;

        SetIsUpgradeUIOpen(false);
        currentAbility = currentChoises[0];
        currentAbility.SetOutline(true);
        
        foreach (var abilityGameObject in abilityGameObjects)
        {
            abilityGameObject.SetActive(false);
        }
        
    }
    
    void DeselectAll()
    {
        foreach (var choise in currentChoises)
        {
            choise.SetOutline(false);
        }
    }
   
    
    void UpdateAbilityUI()
    {
        for (int i = 0; i < abilityGameObjects.Count; i++)
        {
            GameObject abilityGO = abilityGameObjects[i];
            Ability script = abilityGO.GetComponent<Ability>();
            AbilitySO scriptableObjects = script.GetAbilitySO();
            
            abilitiesUis[i].abilitySO = scriptableObjects;
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
        Debug.Log("Called select ability");
        List<UpgradeSO> abilityUpgradeSOs = abilityGameObjects[currentSelectedNumber].GetComponent<Ability>().GetUpgrades();
        
        upgrades[0].upgradeSO = abilityUpgradeSOs[0];
        upgrades[1].upgradeSO = abilityUpgradeSOs[1];
    
        selectedAbility = abilityGameObjects[currentSelectedNumber].GetComponent<IUpgradeable>();
        
        
        
        UpdateAbilityUI();

        SetIsUpgradeUIOpen(true);
    }
    
    

    void SelectUpgrade()
    {
        Debug.Log("Called select update");

        if (currentSelectedNumber == 0)
        {
            UpgradeManager.instance.UpgradeAbilityOption1(selectedAbility);
            Debug.Log("Called upgrade 1");
        }
        if (currentSelectedNumber == 1)
        {
            UpgradeManager.instance.UpgradeAbilityOption2(selectedAbility);
        }
        
        ExitAnvil();
    }
    
    void SetIsUpgradeUIOpen(bool value)
    {
        upgradeUI.SetActive(value);
        isUpgradeUIOpen = value;
        abilityUI.SetActive(!value);
        
        DeselectAll();
        currentChoises.Clear();
        
        if(value)
        {
            
            currentChoises.AddRange(upgrades);
            
            
            
        }
        else
        {
            currentChoises.AddRange(abilitiesUis);
        }
        /*
        if(currentChoises.Count == 0)
            return;*/
        
        currentSelectedNumber = 0;
        currentAbility = currentChoises[0];
        currentAbility.SetOutline(true);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && currentSelectedNumber > 0)
            SelectLeft();
        if (Input.GetKeyDown(KeyCode.D))
            {
                if(currentSelectedNumber < progressNumber - 1 && currentSelectedNumber < currentChoises.Count - 1)
                    SelectRight();
                else if (isUpgradeUIOpen && currentSelectedNumber < currentChoises.Count - 1 && progressNumber == 1)
                    SelectRight();
            }

        if(currentSelectedNumber != 0)
            currentChoises[0].SetOutline(false);
        
        
        if (Input.GetKeyDown(KeyCode.E) )
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
        DeselectAll();
        currentAbility.SetOutline(true);
    }
    void ExitAnvil()
    {
        //Enable all ability gameobjects
        foreach (var abilityGameObject in abilityGameObjects)
        {
            abilityGameObject.SetActive(true);
        }
        
        DeselectAll();
        SetIsUpgradeUIOpen(false);
        abilityUI.SetActive(false);
        
        //Deselect all abilities
        foreach (var choise in currentChoises)
        {
            choise.SetOutline(false);
        }
        
        
        
        currentChoises.Clear();
        abilityGameObjects.Clear();
        currentAbility.SetOutline(false);
        currentAbility = null;
        
        
        
        playerActions.enabled = true;
        playerRigidbody.isKinematic = false;
        
        playerRigidbody.bodyType = RigidbodyType2D.Dynamic;
        playerRigidbody.velocity = Vector2.zero;
        //Set the playerRigidbody bodytype to dynamic
        
        playerDash.SetActive(true);

        this.gameObject.SetActive(false);
    }

    
}


