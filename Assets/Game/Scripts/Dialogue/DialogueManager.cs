using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is taking a dialogue object and displaying it in the UI
public class DialogueManager : MonoBehaviour
{
    [SerializeField] private DialogueSpeaker[] speakers;
    [SerializeField] private GameObject dialogueWindowPrefab;
    private DialogueWindow dialogueWindow;

    private Queue<Dialogue> dialogues;

    void Start()
    {
        dialogues = new Queue<Dialogue>();
    }

    public void StartDialogue(Dialogue[] newDialogues)
    {
        dialogues.Clear();
        foreach (Dialogue dialogue in newDialogues)
        {
            dialogues.Enqueue(dialogue);
        }

        dialogueWindow = Instantiate(dialogueWindowPrefab, transform).GetComponent<DialogueWindow>();
        dialogueWindow.next.onClick.AddListener(DisplayNextSentence);
        DisplayNextSentence();
    }

    private void DisplayNextSentence()
    {
        if (dialogues.Count == 0)
        {
            EndDialogue();
            return;
        }

        Dialogue dialogue = dialogues.Dequeue();
        DialogueSpeaker speaker = speakers[dialogue.speakerID];
        dialogueWindow.DisplayDialogue(speaker, dialogue);
    }

    private void EndDialogue()
    {
        Destroy(dialogueWindow.gameObject);
        Debug.Log("End of Story");
    }
}
