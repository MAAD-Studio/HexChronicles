using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is calling the DialogueManager to start a dialogue
public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private Dialogue[] dialogue;
    [SerializeField] private DialogueManager dialogueManager;

    public void TriggerDialogue()
    {
        dialogueManager.StartDialogue(dialogue);
    }
}
