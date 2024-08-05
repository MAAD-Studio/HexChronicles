using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMarker : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    private int markerIndex = 0;

    [SerializeField] private GameObject clickArrow;
    [SerializeField] private GameObject[] tutorialOneMarkersOne;
    [SerializeField] private GameObject[] tutorialOneMarkersTwo;

    private Tutorial_One tutorial_One;
    private GameObject[] currentMarkers;

    private void Start()
    {
        EventBus.Instance.Subscribe<OnTutorialStart>(OnTutorialStart);

        foreach (GameObject marker in tutorialOneMarkersOne)
        {
            marker.SetActive(false);
        }
        foreach (GameObject marker in tutorialOneMarkersTwo)
        {
            marker.SetActive(false);
        }
        clickArrow.SetActive(false);
    }

    private void OnDestroy()
    {
        EventBus.Instance.Unsubscribe<OnTutorialStart>(OnTutorialStart);
    }

    private void OnTutorialStart(object obj)
    {
        tutorial_One = FindObjectOfType<Tutorial_One>();
        Debug.Assert(tutorial_One != null, "TutorialMarker couldn't find Tutorial_One");

        tutorial_One.ShowClickArrow += () => clickArrow.SetActive(true);
        tutorial_One.HideClickArrow += () => clickArrow.SetActive(false);

        tutorial_One.IntroduceHeroInfo += ShowTutorialOneMarkersOne;
        tutorial_One.IntroduceEnemyInfo += ShowTutorialOneMarkersTwo;
    }

    private void ShowTutorialOneMarkersOne()
    {
        dialogueManager.NextDialogue += NextMarker;
        currentMarkers = tutorialOneMarkersOne;
        markerIndex = 0;
    }

    private void ShowTutorialOneMarkersTwo()
    {
        dialogueManager.NextDialogue += NextMarker;
        currentMarkers = tutorialOneMarkersTwo;
        markerIndex = 0;
    }

    private void NextMarker()
    {
        if (markerIndex < currentMarkers.Length)
        {
            if (markerIndex > 0)
            {
                currentMarkers[markerIndex - 1].SetActive(false);
            }
            currentMarkers[markerIndex].SetActive(true);
            markerIndex++;
        }
        else
        {
            currentMarkers[markerIndex - 1].SetActive(false);
            dialogueManager.NextDialogue -= NextMarker;
        }
    }
}
