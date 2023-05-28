using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    DialogueManager dialogueManager;
    private void Start()
    {
        dialogueManager = DialogueManager.dialogManager;
    }
    public void Pause()
    {
        dialogueManager.currentTimeline.Pause();
    }
    public void Resume()
    {
        dialogueManager.currentTimeline.Resume();
    }
   
}
