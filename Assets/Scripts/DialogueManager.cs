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
    private Sprite brokkrFace;
    private Sprite scryerFace;
    private Sprite lokiFace;

    public Image potraitLeft;
    public Image potraitRight;

    private PlayableDirector ingridAndAstridTL;
    private PlayableDirector swordPickUpTL;
    private PlayableDirector tutorialTL;

    private GameObject T3;
    private GameObject T4;
    private GameObject T5;

    public static bool readyToSpawn = false;

    private SpawnSystem spawnSystem;

    public PlayableDirector currentTimeline;

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
        dialogueBox = GameObject.Find("DialogueBox");
        dialogueBox.SetActive(false);
        playerAction = GameObject.Find("Player").GetComponent<PlayerAction>();
        //mjoelnir = GameObject.Find("Mjoelnir").GetComponent<Mjoelnir>();
        dialogueTarget = GameObject.FindGameObjectsWithTag("NPC");
        dialogBoxAnim = dialogueBox.GetComponent<Animator>();

        brokkrFace = Resources.Load<Sprite>("Sprites/Brokkr");
        scryerFace = Resources.Load<Sprite>("Sprites/ScryerFace");
        lokiFace = Resources.Load<Sprite>("Sprites/LokiFace");

        if (SceneManager.GetActiveScene().name == "Village" || SceneManager.GetActiveScene().name == "VillageWithTL")
        {
            Debug.Log("Current scene is Village");
            ingridAndAstridTL = GameObject.Find("AstridAndIngridTL").GetComponent<PlayableDirector>();
            swordPickUpTL = GameObject.Find("SwordPickUpTL").GetComponent<PlayableDirector>();
            tutorialTL = GameObject.Find("TutorialTL").GetComponent<PlayableDirector>();

            T3 = GameObject.Find("T3");
            T4 = GameObject.Find("T4");
            T5 = GameObject.Find("T5");
            spawnSystem = GameObject.Find("GameManager").GetComponent<SpawnSystem>();
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
        Time.timeScale = 0f;

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
        if (currentTimeline != null && !TimelineManager.timelineManager.tutorialIsStarted)
        {
            TimelineManager.timelineManager.ResumeTL();
            Debug.Log("Resumed timeline but not in tutorial");
        }

        StartCoroutine(SetReadyToSpawn());

        if (TimelineManager.timelineManager.tutorialIsStarted && !SpawnSystem.waveAlive && SpawnSystem.totalWaves < 1)
        {
            Debug.Log("End of dialog in tutorial");
            switch (TimelineManager.timelineManager.currentTutorialState)
            {
                case 2:
                    T3.GetComponent<SendWave>().SendWaves();
                    StartCoroutine(TutorialStateCoroutine());
                    break;

                case 3:
                    T4.GetComponent<SendWave>().SendWaves();
                    StartCoroutine(TutorialStateCoroutine());

                    break;

                case 4:
                    T5.GetComponent<SendWave>().SendWaves();
                    StartCoroutine(TutorialStateCoroutine());

                    break;

                default:
                    break;
            }
        }

        Time.timeScale = 1f;
    }

    private IEnumerator TutorialStateCoroutine()
    {
        yield return new WaitForSeconds(1f);
        TimelineManager.timelineManager.AddToCurrentTutorialState();
    }

    private IEnumerator SetReadyToSpawn()
    {
        yield return new WaitForSeconds(0.5f);
        readyToSpawn = true;
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
        if (line.StartsWith("Brokkr"))
        {
            potraitLeft.enabled = true;
            potraitLeft.sprite = brokkrFace;
            potraitRight.enabled = false;
        }
        else if (line.StartsWith("Scryer"))
        {
            potraitRight.enabled = true;
            potraitRight.sprite = scryerFace;
            potraitLeft.enabled = false;
        }
        else if (line.StartsWith("Loki"))
        {
            potraitRight.enabled = true;
            potraitRight.sprite = lokiFace;
            potraitLeft.enabled = false;
        }
        else
        {
            potraitLeft.enabled = false;
            potraitRight.enabled = false;
        }
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