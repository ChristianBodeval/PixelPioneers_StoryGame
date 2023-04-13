using Ink.Runtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    [Header("Dialogue")]
    public bool isDialoguePlaying = false;

    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkFileText;

    private Story inkStory;
    private Choice[] choice;
    [SerializeField] private bool isPlayerInRange;
    private GameObject player;
    public GameObject visualCue; // Pops up when the player can interact
    private PlayerAction playerAction;
    private DialogueManager dialogueManager;
    private TextMeshProUGUI dialogueNPCName;

    public HasTalkedTo hasTalkedTo;

    public int NPCIndex;

    private void Awake()
    {
        playerAction = GameObject.Find("Player").GetComponent<PlayerAction>();
        dialogueManager = FindObjectOfType<DialogueManager>();
        dialogueNPCName = dialogueManager.dialogueNPCName;
    }

    private void Start()
    {
        visualCue.SetActive(false);
        isPlayerInRange = false;
        inkStory = new Story(inkFileText.text);
    }

    

    private void Update()
    {
        SetNPCIndex();
        isDialoguePlaying = dialogueManager.isDialoguePlaying;

        if (isPlayerInRange && !isDialoguePlaying)
        {
            visualCue.SetActive(true);
            if (Input.GetButtonDown("Interact"))
            {
                isDialoguePlaying = true;
                dialogueManager.EnterDialogueMode(inkFileText);
                SetHasTalkedToArr();
            }
        }
        else
        {
            visualCue.SetActive(false);
        }
    }
    private void SetHasTalkedToArr()
    {
        hasTalkedTo.hasTalkedToArr[NPCIndex] = true;
    }

    private void ChoiceDialogue(List<Choice> choices)
    {
        //** Do something with choices
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            isPlayerInRange = true;
            //player = col.gameObject;
            dialogueNPCName.text = gameObject.name;

            Debug.Log("Player collided with " + gameObject.name);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            isPlayerInRange = false;
            dialogueNPCName.text = "";
        }
    }

    public void SetNPCIndex()
    {
        switch (gameObject.name)
        {
            case "NPC 1":
                NPCIndex = 1;
                break;

            case "NPC 2":
                NPCIndex = 2;
                break;

            case "NPC 3":
                NPCIndex = 3;
                break;

            case "NPC 4":
                NPCIndex = 4;
                break;
        }
    }
}