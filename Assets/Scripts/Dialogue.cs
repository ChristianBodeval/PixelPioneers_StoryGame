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
        isDialoguePlaying = dialogueManager.isDialoguePlaying;
        
        if (isPlayerInRange && !isDialoguePlaying)
        {
            visualCue.SetActive(true);
            if (Input.GetButtonDown("Interact"))
            {
                isDialoguePlaying = true;
                dialogueManager.EnterDialogueMode(inkFileText);
            }
        }
        else
        {
            visualCue.SetActive(false);
        }
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
}