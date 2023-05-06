using UnityEngine;

public class UpgradeSystemVisual : MonoBehaviour
{
    public GameObject slashAbilityUpgrades;
    public GameObject dashUpgrades;
    public GameObject mjoelnirUpgrades;
    public GameObject gungnirUpgrades;

    public WeaponType currentWeapon = WeaponType.Dash;

    public enum WeaponType
    {
        SlashAbility,
        Dash,
        Mjoelnir,
        Gungnir
    }

    public void StartUpgradeVisual()
    {
        switch (currentWeapon)
        {
            case WeaponType.SlashAbility:
                slashAbilityUpgrades.SetActive(true);
                break;

            case WeaponType.Dash:
                dashUpgrades.SetActive(true);
                break;

            case WeaponType.Mjoelnir:
                break;

            case WeaponType.Gungnir:
                break;
        }
    }
}