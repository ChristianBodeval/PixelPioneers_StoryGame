using Ink.Runtime;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager dialogManager { get; private set; } //singleton

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialogueBox;

    [SerializeField] private GameObject Speechbubble1;
    [SerializeField] private GameObject Speechbubble2;

    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI dialogueTextHolder;
    [SerializeField] private TextMeshProUGUI SpeechBubbleTextZeus;
    [SerializeField] private TextMeshProUGUI SpeechBubbleTextOdin;
    private bool usesSpeechBubble;

    [SerializeField] public Story currentStory;

    public TextMeshProUGUI dialogueNPCName;

    public bool isDialoguePlaying;

    private bool isButtonOnCD;

    [HideInInspector] public bool isShowingText;

    [SerializeField] private float typingSpeed = 0.1f;

    private Coroutine DisplayLineCoroutine;

    private PlayerAction playerAction;

    //private Mjoelnir mjoelnir;

    //public bool isPlayerInRange;

    private GameObject[] dialogueTarget;

    public Image continueButton;

    private Animator dialogBoxAnim;

    private PlayableDirector ingridAndAstridTL;
    private PlayableDirector swordPickUpTL;
    private PlayableDirector tutorialTL;

    private PlayableDirector currentTimeline;

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
        //mjoelnir = GameObject.Find("Mjoelnir").GetComponent<Mjoelnir>();
        dialogueTarget = GameObject.FindGameObjectsWithTag("NPC");
        dialogBoxAnim = dialogueBox.GetComponent<Animator>();

        if (SceneManager.GetActiveScene().name == "Village")
        {
            Debug.Log("Current scene is Village");
            ingridAndAstridTL = GameObject.Find("AstridAndIngridTL").GetComponent<PlayableDirector>();
            swordPickUpTL = GameObject.Find("SwordPickUpTL").GetComponent<PlayableDirector>();
            tutorialTL = GameObject.Find("TutorialTL").GetComponent<PlayableDirector>();
        }
    }

    public void SpeechBubbleSwitch()
    {
        if (Speechbubble2.activeSelf)
        {
            Speechbubble1.SetActive(true);
            Speechbubble2.SetActive(false);
        }
        if (Speechbubble1.activeSelf)
        {
            Speechbubble1.SetActive(false);
            Speechbubble2.SetActive(true);
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Interact") && !isButtonOnCD && isDialoguePlaying)
        {
            ContinueStory();
        }

        //Debug.Log("Current story is:" + currentStory.currentText);
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
        //mjoelnir.enabled = false;
        ContinueStory();
        dialogBoxAnim.Play("FlyUp");
    }

    public void ExitDialogueModeMethod()
    {
        StartCoroutine("ExitDialogueMode");
    }

    public IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f);
        isDialoguePlaying = false;
        //dialogueBox.SetActive(false);

        //resets the text
        dialogueText.text = "";

        playerAction.enabled = true;
        //mjoelnir.enabled = true;

        Debug.Log("Exited Dialog Mode");

        currentStory.ResetState();

        playerAction.StartMove();

        foreach (GameObject i in dialogueTarget)
        {
            i.GetComponent<Dialogue>().isDialoguePlaying = false;
        }

        if (dialogBoxAnim != null)
            dialogBoxAnim.Play("FlyDown");

        switch (currentTimeline.name)
        {
            case "StartTL":
                ingridAndAstridTL.Play();
                break;

            case "AstridAndIngridTL":
                swordPickUpTL.Play();
                break;

            case "SwordPickUpTL":
                tutorialTL.Play();
                break;

            case "TutorialTL":
                break;

            default:
                break;
        }
    }

    public void UsesSpeechBubble()
    {
        usesSpeechBubble = true;
    }

    public void DoesntUsesSpeechBubble()
    {
        usesSpeechBubble = false;
    }

    public void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            continueButton.enabled = false;

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
        continueButton.enabled = true;
    }

    public void StartAstridAndIngridTL()
    {
        ingridAndAstridTL.Play();
        Debug.Log("Astrid and Ingrid TL started");
    }

    public void StartSwordPickUpTL()
    {
        swordPickUpTL.Play();
        Debug.Log("SwordPickUp TL started");
    }

    public void StartTutorialTL()
    {
        tutorialTL.Play();
        Debug.Log("Tutorial TL started");
    }

    public void SetCurrentTimeline(PlayableDirector timeline)
    {
        currentTimeline = timeline;
    }
}