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
    private GameObject dialogueBox;

    private GameObject Speechbubble1;
    private GameObject Speechbubble2;

    private TextMeshProUGUI dialogueText;
    private TextMeshProUGUI dialogueTextHolder;
    private TextMeshProUGUI SpeechBubbleTextZeus;
    private TextMeshProUGUI SpeechBubbleTextOdin;

    [SerializeField] public Story currentStory;

    //public TextMeshProUGUI dialogueNPCName;

    public bool isDialoguePlaying;

    private bool isButtonOnCD;

    [HideInInspector] public bool isShowingText;

    [SerializeField] private float typingSpeed = 0.1f;

    private Coroutine DisplayLineCoroutine;

    private PlayerAction playerAction;

    private GameObject[] dialogueTarget;

    private Image continueButton;

    private GameObject UI;
    private Animator dialogBoxAnim;
    private Sprite brokkrFace;
    private Sprite scryerFace;
    private Sprite lokiFace;
    private Sprite fatherFace;

    private Image potraitLeft;
    private Image potraitRight;

    private PlayableDirector ingridAndAstridTL;
    private PlayableDirector swordPickUpTL;
    private PlayableDirector tutorialTL;
    [HideInInspector] public PlayableDirector endTL;

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
            Debug.Log("DialogueManager destroy");
            Destroy(this);
        }
        else
        {
            dialogManager = this;
        }

        SceneManager.activeSceneChanged += FindVariables;
    }

    private void Start()
    {
        brokkrFace = Resources.Load<Sprite>("Sprites/Brokkr");
        scryerFace = Resources.Load<Sprite>("Sprites/ScryerFace");
        lokiFace = Resources.Load<Sprite>("Sprites/LokiFace");
        fatherFace = Resources.Load<Sprite>("Sprites/FatherFace");

        FindVariables(SceneManager.GetActiveScene(), SceneManager.GetActiveScene());
    }

    private void FindVariables(Scene previousScene, Scene newScene)
    {
        UI = GameObject.Find("UI");
        dialogueBox = UI.transform.Find("DialogueBox").gameObject;
        if (dialogueBox != null) dialogueBox.SetActive(false);
        dialogBoxAnim = dialogueBox.GetComponent<Animator>();

        Speechbubble1 = GameObject.Find("SpeechBubble1");
        if (Speechbubble1 != null) Speechbubble1.SetActive(false);

        Speechbubble2 = GameObject.Find("SpeechBubble2");
        if (Speechbubble2 != null) Speechbubble2.SetActive(false);

        dialogueText = dialogueBox.transform.Find("DialogueText").GetComponent<TextMeshProUGUI>();
        continueButton = dialogueBox.transform.Find("ContinueArrow").GetComponent<Image>();
        potraitLeft = dialogueBox.transform.Find("PotraitLeft").GetComponent<Image>();
        potraitRight = dialogueBox.transform.Find("PotraitRight").GetComponent<Image>();
        playerAction = GameObject.FindWithTag("Player").GetComponent<PlayerAction>();
        dialogueTarget = GameObject.FindGameObjectsWithTag("NPC");

        isDialoguePlaying = false;

        if (GameObject.Find("EndTL") != null) endTL = GameObject.Find("EndTL").GetComponent<PlayableDirector>();

        if (SceneManager.GetActiveScene().name == "Village" || SceneManager.GetActiveScene().name == "VillageWithTL")
        {
            Debug.Log("Current scene is Village");
            ingridAndAstridTL = GameObject.Find("AstridAndIngridTL").GetComponent<PlayableDirector>();
            swordPickUpTL = GameObject.Find("SwordPickUpTL").GetComponent<PlayableDirector>();
            tutorialTL = GameObject.Find("TutorialTL").GetComponent<PlayableDirector>();
            endTL = GameObject.Find("EndTL").GetComponent<PlayableDirector>();

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
        //Time.timeScale = 0f;

        isDialoguePlaying = true;
        dialogueBox.SetActive(true);
        currentStory = new Story(inkJson.text);
        Debug.Log("Entered Dialog Mode");

        isDialoguePlaying = true;
        playerAction.StopMove();
        playerAction.enabled = false;
        ContinueStory();
        dialogBoxAnim.Play("FlyUp");
    }

    public void ExitDialogueModeMethod()
    {
        if (gameObject != null && isActiveAndEnabled)
        {
            StartCoroutine(ExitDialogueMode());
        }
    }

    public IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f);

        if (dialogManager == null) yield break;

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

        if (currentTimeline != null)
        {
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

                case "EndVillage":
                    TimelineManager.timelineManager.tutorialIsStarted = false;
                    break;

                default:
                    break;
            }
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

                    //T5.GetComponent<SendWave>().SendWaves();
                    //StartCoroutine(TutorialStateCoroutine());

                    break;

                default:
                    break;
            }
        }

        // Activate Hermes
        GameObject hermes = GameObject.FindWithTag("Boss");
        hermes.GetComponent<WeaponAbility>().enabled = true;
        hermes.GetComponent<Hermes_Pathing>().enabled = true;
        hermes.GetComponent<Hermes_Attack>().enabled = true;

        Time.timeScale = 1f;
    }

    private IEnumerator TutorialStateCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        TimelineManager.timelineManager.AddToCurrentTutorialState();
    }

    private IEnumerator SetReadyToSpawn()
    {
        yield return new WaitForSeconds(0.5f);
        readyToSpawn = true;
    }

    public void ContinueStory()
    {
        if (currentStory.canContinue && gameObject != null && isActiveAndEnabled)
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
        else if (line.StartsWith("Father"))
        {
            potraitRight.enabled = true;
            potraitRight.sprite = fatherFace;
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

    public void SetCurrentTimeline(PlayableDirector timeline)
    {
        currentTimeline = timeline;
    }
}