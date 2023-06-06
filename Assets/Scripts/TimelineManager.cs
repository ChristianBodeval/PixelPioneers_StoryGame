using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimelineManager : MonoBehaviour
{
    public static TimelineManager timelineManager { get; private set; } //singleton

    private DialogueManager dialogueManager;
    private bool timeIsScaled;
    private AudioClip rainSound;
    private float volume = 1f;

    public bool tutorialIsStarted;
    private List<(GameObject, string)> soundLoop = new List<(GameObject, string)>();

    private bool[] hasPressedWASDKeys = new bool[4];
    private bool hasPressedJ = false;
    public int currentTutorialState = 0;

    private bool canContinue = true;

    private GameObject lokiSmol;
    private bool endOfGame = false;

    private void Awake()
    {
        rainSound = Resources.Load<AudioClip>("SFX/Ambiance_Rain");
    }

    private void Start()
    {
        timelineManager = this;
        dialogueManager = GameObject.Find("GameManager").GetComponent<DialogueManager>();
    }

    #region Pre Tutorial

    public void Pause()
    {
        dialogueManager.currentTimeline.Pause();
    }

    public void ResumeTL()
    {
        dialogueManager.currentTimeline.Resume();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && !timeIsScaled)
        {
            Time.timeScale = 8;
            timeIsScaled = true;
        }
        else if (Input.GetKeyDown(KeyCode.T) && timeIsScaled)
        {
            Time.timeScale = 1;
            timeIsScaled = false;
        }

        // If the tutorial is started we resume the timeline
        if (tutorialIsStarted)
            Tutorial();

        if (endOfGame)
        {
            lokiSmol = GameObject.Find("Loki_smol(Clone)");
            
            
            
            
            
            lokiSmol.GetComponent<HermesPathingSmol>().SetEndTarget();
        }
    }

    public void SetEndOfGameTarget()
    {
        endOfGame = true;
    }

    public void StartScene(SceneAsset scene)
    {
        SceneManager.LoadScene(scene.name);
    }

    private IEnumerator ResumeTLCoroutine()
    {
        canContinue = false;

        yield return new WaitForSeconds(1f);
        //ResumeTL();
        dialogueManager.ResumeTimeline();
        canContinue = true; ;
    }

    public void SetSoundVolume(float volume)
    {
        this.volume = volume;
    }

    public void StartSoundLoop(AudioClip sound)
    {
        soundLoop.Add((SFXManager.singleton.PlayLoop(sound, transform.position, volume, true, null), sound.name));
        //SFXManager.singleton.PlayLoop(sound, transform.position, volume, true, null);
    }

    public void StopSoundLoop(AudioClip sound)
    {
        foreach ((GameObject soundGameObject, string soundName) in soundLoop)
        {
            if (soundName == sound.name)
            {
                Pool.pool.ReturnToSFXPool(soundGameObject);
            }
        }
    }

    #endregion Pre Tutorial

    public void AddToCurrentTutorialState()
    {
        currentTutorialState++;
    }

    //TUTORIAL
    public void StartTutorial()
    {
        tutorialIsStarted = true;
    }

    private void Tutorial()
    {
        #region T1

        if (Input.GetKeyDown(KeyCode.W))
        {
            hasPressedWASDKeys[0] = true; // Set W key state to true
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            hasPressedWASDKeys[1] = true; // Set A key state to true
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            hasPressedWASDKeys[2] = true; // Set S key state to true
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            hasPressedWASDKeys[3] = true; // Set D key state to true
        }

        // Example: Check if all WASD keys are pressed
        if (hasPressedWASDKeys[0] && hasPressedWASDKeys[1] && hasPressedWASDKeys[2] && hasPressedWASDKeys[3] && currentTutorialState == 0)
        {
            dialogueManager.ResumeTimeline();
            AddToCurrentTutorialState();
        }

        #endregion T1

        #region T2

        if (Input.GetKeyDown(KeyCode.J) && !hasPressedJ && currentTutorialState == 1)
        {
            hasPressedJ = true;
            ResumeTL();
            AddToCurrentTutorialState();
        }

        #endregion T2

        #region T3

        if (tutorialIsStarted && !SpawnSystem.waveAlive && SpawnSystem.totalWaves < 1)
        {
            Debug.Log("Wave is done");

            switch (currentTutorialState)
            {
                case 3:

                    if (canContinue)
                    {
                        StartCoroutine(ResumeTLCoroutine());
                    }
                    break;

                case 4:
                    if (canContinue)
                    {
                        AddToCurrentTutorialState();
                        //StartCoroutine(ResumeTLCoroutine());
                    }
                    break;

                case 5:
                    Debug.Log("Tutorial is done");
                    tutorialIsStarted = false;
                    dialogueManager.endTL.Play();
                    break;

                default:
                    break;
            }
        }
        Debug.Log("Current tutorial: " + currentTutorialState);
        //Enemy dies

        #endregion T3
    }
}