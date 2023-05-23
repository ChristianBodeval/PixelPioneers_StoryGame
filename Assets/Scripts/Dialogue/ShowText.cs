using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class ShowText : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    private void Start()
    {
        //dialogueText = GetComponent<TextMeshProUGUI>();
    }
    private void OnEnable()
    {
        StartCoroutine(ShowTextFromString(dialogueText.text));
    }
    private IEnumerator ShowTextFromString(string line)
    {
        //Empty the Dialogue text
        dialogueText.text = "";

        //For each letter in the dialogue
        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.02f);
        }
    }
}
