using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class Dialogue : MonoBehaviour
{
    [Header("Dialogue")]
    public bool inDialogue = false;
    [SerializeField] private TextAsset inkFileText;
    private Story inkStory;
    private Choice[] choice;
    private bool playerInRange = false;
    private GameObject player;
    private GameObject visualCue; // Pops up when the player can interact   

    private void Awake()
    {
        visualCue = GameObject.Find("DialogueCue");
        visualCue.SetActive(false);
    }

    private void Start()
    {
        playerInRange = false;
        inkStory = new Story(inkFileText.text);
    }

    private void Update()
    {
        if (playerInRange)
        {
            visualCue.SetActive(true);
            visualCue.transform.position = gameObject.transform.position + (Vector3)Vector2.up * 0.4f;

            if (Input.GetButton("Interact") && !inDialogue)
            {
                StartDialogue();
            }
            else if (Input.GetButton("Interact") && inkStory.canContinue)
            {
                ContinueDialogue();
            }
            else if (Input.GetButton("Interact") && inkStory.currentChoices.Count > 0)
            {
                ChoiceDialogue(inkStory.currentChoices);
            }
            else if (Input.GetButton("Interact") && inDialogue)
            {
                EndDialogue();
            }
        }
        else
        {
            visualCue.SetActive(false);
        }
    }

    private void StartDialogue()
    {
        inDialogue = true;
        player.GetComponent<PlayerAction>().StopMove(); // Disallow player movement
    }

    private void ContinueDialogue()
    {
        inkStory.Continue();
    }

    private void ChoiceDialogue(List<Choice> choices)
    {
        //** Do something with choices
    }

    private void EndDialogue()
    {
        inDialogue = false;
        player.GetComponent<PlayerAction>().StartMove(); // Allow player movement
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            playerInRange = true;
            player = col.gameObject;
        }   
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
