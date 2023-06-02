using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueBoxAnim : MonoBehaviour
{
    [HideInInspector] public DialogueManager dialogueManager;

    private Animator anim;

    private void Awake()
    {
        dialogueManager = GameObject.Find("GameManager").GetComponent<DialogueManager>();
    }
    
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        PlayDialogueBoxAnim();
    }

    public void PlayDialogueBoxAnim()
    {
        if (dialogueManager.isDialoguePlaying)
        {
            anim.SetBool("isDialoguePlaying", true);
        }
        else
        {
        anim.Play("FlyDown");
            anim.SetBool("isDialoguePlaying", false);
        }
    }
}
