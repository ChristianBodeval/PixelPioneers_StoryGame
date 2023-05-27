using System.Collections.Generic;
using UnityEngine;

public class TimelineManager : MonoBehaviour
{
    public static TimelineManager timelineManager { get; private set; } //singleton

    private DialogueManager dialogueManager;
    private bool timeIsScaled;
    private AudioClip rainSound;
    private float volume = 1f;

    public bool tutorialIsStarted;
    private List<(GameObject, string)> soundLoop = new List<(GameObject, string)>();

    public bool T1Done = false;
    public bool T2Done = false;
    public bool T3Done = false;
    public bool T4Done = false;
    public bool T5Done = false;
    private bool[] hasPressedWASDKeys = new bool[4];
    private bool hasPressedJ = false;

    private void Awake()
    {
        rainSound = Resources.Load<AudioClip>("SFX/Ambiance_Rain");
        if (timelineManager != null && timelineManager != this)
        {
            Destroy(this);
        }
        else
        {
            timelineManager = this;
        }
    }

    private void Start()
    {
        dialogueManager = DialogueManager.dialogManager;
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

        if (tutorialIsStarted)
            Tutorial();
    }

    public void SetSoundVolume(float volume)
    {
        this.volume = volume;
    }

    public void StartSoundLoop(AudioClip sound)
    {
        soundLoop.Add((SFXManager.singleton.PlayLoop(sound, transform.position, volume, true, null), sound.name));
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
        if (hasPressedWASDKeys[0] && hasPressedWASDKeys[1] && hasPressedWASDKeys[2] && hasPressedWASDKeys[3] && !T1Done)
        {
            T1Done = true;
            ResumeTL();
        }

        #endregion T1

        #region T2

        if (Input.GetKeyDown(KeyCode.J) && !hasPressedJ && T1Done)
        {
            hasPressedJ = true;
            ResumeTL();
            T2Done = true;
        }

        #endregion T2
        #region T3
        //Enemy dies
        #endregion T3
    }
}