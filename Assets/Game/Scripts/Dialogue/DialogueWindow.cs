using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueWindow : MonoBehaviour
{
    public Button next;
    [SerializeField] private Image avatar;
    [SerializeField] private Image element;
    [SerializeField] private TextMeshProUGUI speakerName;
    [SerializeField] private TextMeshProUGUI speach;

    public void DisplayDialogue(DialogueSpeaker speaker, Dialogue dialogue)
    {
        avatar.sprite = speaker.speakerSprite;
        element.sprite = speaker.speakerElement;
        speakerName.text = speaker.speakerName;

        StopAllCoroutines();
        StartCoroutine(TypeSentence(dialogue.sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        speach.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            speach.text += letter;
            yield return null;
            //yield return new WaitForSeconds(0.05f);
        }
    }
}
