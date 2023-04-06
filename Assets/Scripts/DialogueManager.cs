using Ink.Runtime;
using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager dialogManager { get; private set; } //singleton

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialogueBox;

    [SerializeField] private TextMeshProUGUI dialogueText;

    [SerializeField] public Story currentStory;

    public bool isDialoguePlaying;

    private bool isButtonOnCD;

    [HideInInspector] public bool isShowingText;

    [SerializeField] private float typingSpeed = 0.1f;

    private Coroutine DisplayLineCoroutine;

    private PlayerAction playerAction;

    private Mjölnir mjölnir;

    //public bool isPlayerInRange;

    private Dialogue dialogueTarget;

    private Animator dialogBoxAnim;

    private void Awake()
    {
        if (dialogManager != null && dialogManager != this)
        {
            Destroy(this);
        }
        else
        {
            dialogManager = this;
        }
    }

    private void Start()
    {
        isDialoguePlaying = false;
        dialogueBox.SetActive(false);
        playerAction = GameObject.Find("Player").GetComponent<PlayerAction>();
        mjölnir = GameObject.Find("Mjölnir").GetComponent<Mjölnir>();
        dialogueTarget = GetComponent<Dialogue>();
        dialogBoxAnim = dialogueBox.GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Interact") && !isButtonOnCD && isDialoguePlaying)
        {
            ContinueStory();
        }

        Debug.Log("Current story is:" + currentStory.currentText);
    }

    private IEnumerator ButtonCD()
    {
        isButtonOnCD = true;

        yield return new WaitForSeconds(0.5f);

        isButtonOnCD = false;
    }

    public void EnterDialogueMode(TextAsset inkJson)
    {
        isDialoguePlaying = true;
        dialogueBox.SetActive(true);
        currentStory = new Story(inkJson.text);
        Debug.Log("Entered Dialog Mode");

        isDialoguePlaying = true;
        playerAction.StopMove();
        playerAction.enabled = false;
        mjölnir.enabled = false;
        ContinueStory();
        dialogBoxAnim.Play("FlyUp");
        
    }

    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f);
        isDialoguePlaying = false;
        //dialogueBox.SetActive(false);

        //resets the text
        dialogueText.text = "";

        playerAction.enabled = true;
        mjölnir.enabled = true;

        Debug.Log("Exited Dialog Mode");

        currentStory.ResetState();
        
        playerAction.StartMove();

        dialogueTarget.isDialoguePlaying = false;
        
        if (dialogBoxAnim != null)
        dialogBoxAnim.Play("FlyDown");
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            if (DisplayLineCoroutine != null)
            {
                StopCoroutine(DisplayLineCoroutine);
            }

            //Show the text for the current dialogue
            DisplayLineCoroutine = StartCoroutine(ShowText(currentStory.Continue()));
            StartCoroutine("ButtonCD");
            Debug.Log("Dialog can continue");
        }
        else
        {
            StartCoroutine("ExitDialogueMode");
            Debug.Log("Dialog CANT continue");
        }
    }

    private IEnumerator ShowText(string line)
    {
        isShowingText = true;
        //Empty the Dialogue text
        dialogueText.text = "";

        //For each letter in the dialogue
        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isShowingText = false;
    }
}