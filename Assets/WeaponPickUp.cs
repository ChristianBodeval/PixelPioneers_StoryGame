using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class WeaponPickUp : MonoBehaviour
{
    [SerializeField] private TextAsset[] pickUpNotification = new TextAsset[3];

    [Header("SFX")]
    [Range(0f, 1f)] [SerializeField] private float volumeSFX;
    [SerializeField] private AudioClip pickUpSFX;

    public CaveManager caveManager;
    public static int stoneConvoPrepped = 0;
    public static bool isConvoPrepped = false;

    private void Start()
    {
        caveManager = FindObjectOfType<CaveManager>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        SetAbilities();

        SFXManager.singleton.PlaySound(pickUpSFX, transform.position, volumeSFX);

        caveManager.EndCave();

        Destroy(gameObject);
    }

    private void SetAbilities()
    {
        switch (ExtractNumberFromName(SceneManager.GetActiveScene().name))
        {
            case 1:
                stoneConvoPrepped++;
                isConvoPrepped = true;
                SaveManager.singleton.weapon1 = true;
                SaveManager.singleton.weapon2 = true;
                SaveManager.singleton.weapon3 = false;
                SaveManager.singleton.weapon4 = false;
                DialogueManager.dialogManager.EnterDialogueMode(pickUpNotification[0]);
                break;
            case 2:
                stoneConvoPrepped++;
                isConvoPrepped = true;
                SaveManager.singleton.weapon1 = true;
                SaveManager.singleton.weapon2 = true;
                SaveManager.singleton.weapon3 = true;
                SaveManager.singleton.weapon4 = false;
                DialogueManager.dialogManager.EnterDialogueMode(pickUpNotification[1]);
                break;
            case 3:
                stoneConvoPrepped++;
                isConvoPrepped = true;
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
