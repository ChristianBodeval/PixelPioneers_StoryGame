using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
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
    
    
}
