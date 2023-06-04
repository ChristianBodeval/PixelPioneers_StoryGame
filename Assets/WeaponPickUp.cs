using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class WeaponPickUp : MonoBehaviour
{
    [SerializeField] private TextAsset[] pickUpNotification = new TextAsset[3];

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        SetAbilities();
        
        CaveManager.instance.EndCave();

        Destroy(gameObject);
    }

    private void SetAbilities()
    {
        switch (ExtractNumberFromName(SceneManager.GetActiveScene().name))
        {
            case 1:
                SaveManager.singleton.weapon1 = true;
                SaveManager.singleton.weapon2 = true;
                SaveManager.singleton.weapon3 = false;
                SaveManager.singleton.weapon4 = false;
                DialogueManager.dialogManager.EnterDialogueMode(pickUpNotification[0]);
                break;
            case 2:
                SaveManager.singleton.weapon1 = true;
                SaveManager.singleton.weapon2 = true;
                SaveManager.singleton.weapon3 = true;
                SaveManager.singleton.weapon4 = false;
                DialogueManager.dialogManager.EnterDialogueMode(pickUpNotification[1]);
                break;
            case 3:
                SaveManager.singleton.weapon1 = true;
                SaveManager.singleton.weapon2 = true;
                SaveManager.singleton.weapon3 = true;
                SaveManager.singleton.weapon4 = true;
                DialogueManager.dialogManager.EnterDialogueMode(pickUpNotification[2]);
                break;
            default:
                break;
        }
    }

    private int ExtractNumberFromName(string name)
    {
        Match match = Regex.Match(name, @"[1-9]");
        if (match.Success)
        {
            string numberString = match.Value;
            int number;
            if (int.TryParse(numberString, out number))
            {
                return number;
            }
        }
        Debug.LogWarning("Failed to extract number from name: " + name);
        return -1; // Return a default value or handle the error as needed
    }
}
