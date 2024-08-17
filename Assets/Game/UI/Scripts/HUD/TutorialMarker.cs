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

    private Tutorial_Base tutorial_Base;
    private Tutorial_One tutorial_One;
    private Tutorial_Two tutorial_Two;
    private Tutorial_Three tutorial_Three;
    private Tutorial_Four tutorial_Four;
    private GameObject[] currentMarkers;

    #region Unity Methods
    private void Start()
    {
        EventBus.Instance.Subscribe<OnTutorialStart>(OnTutorialStart);
        TurnManager.LevelVictory.AddListener(ResetTutorialMarker);
    }

    private void OnDestroy()
    {
        EventBus.Instance.Unsubscribe<OnTutorialStart>(OnTutorialStart);
        TurnManager.LevelVictory.RemoveListener(ResetTutorialMarker);
    }
    #endregion


    #region Initialize based on Tutorial Type
    private void OnTutorialStart(object obj)
    {
        DisableEveryMarker();

        tutorial_Base = FindObjectOfType<Tutorial_Base>();
        Debug.Assert(tutorial_Base != null, "TutorialMarker couldn't find Tutorial_Base");

        switch (tutorial_Base)
        {
            case Tutorial_One tutorialOne:
                InitializeOne(tutorialOne);
                break;
            case Tutorial_Two tutorialTwo:
                InitializeTwo(tutorialTwo);
                break;
            case Tutorial_Three tutorialThree:
                InitializeThree(tutorialThree);
                break;
            case Tutorial_Four tutorialFour:
                InitializeFour(tutorialFour);
                break;
        }
    }

    private void DisableEveryMarker()
    {
        clickArrow.SetActive(false);

        foreach (GameObject marker in tutorialOneMarkersOne)
        {
            marker.SetActive(false);
        }
        foreach (GameObject marker in tutorialOneMarkersTwo)
        {
            marker.SetActive(false);
        }
    }

    private void InitializeOne(Tutorial_One tutorialOne)
    {
        Debug.Log("Tutorial One");
        tutorial_One = tutorialOne;
        SubscribeTutorialEventsOne();
    }

    private void InitializeTwo(Tutorial_Two tutorialTwo)
    {
        Debug.Log("Tutorial Two");
        tutorial_Two = tutorialTwo;
        SubscribeEventsTwo();
    }

    private void InitializeThree(Tutorial_Three tutorialThree)
    {
        Debug.Log("Tutorial Three");
        tutorial_Three = tutorialThree;
        SubscribeEventsThree();
    }

    private void InitializeFour(Tutorial_Four tutorialFour)
    {
        Debug.Log("Tutorial Four");
        tutorial_Four = tutorialFour;
        SubscribeEventsFour();
    }

    private void ResetTutorialMarker()
    {
        switch (tutorial_Base)
        {
            case Tutorial_One tutorialOne:
                UnsubscribeEventsOne();
                break;
            case Tutorial_Two tutorialTwo:
                UnsubscribeEventsTwo();
                break;
            case Tutorial_Three tutorialThree:
                UnsubscribeEventsThree();
                break;
            case Tutorial_Four tutorialFour:
                UnsubscribeEventsFour();
                break;
        }
    }
    #endregion


    #region Events One
    private void SubscribeTutorialEventsOne()
    {
        tutorial_One.ShowClickArrow += () => clickArrow.SetActive(true);
        tutorial_One.HideClickArrow += () => clickArrow.SetActive(false);

        tutorial_One.IntroduceHeroInfo += ShowTutorialOneMarkersOne;
        tutorial_One.IntroduceEnemyInfo += ShowTutorialOneMarkersTwo;
    }

    private void UnsubscribeEventsOne()
    {
        tutorial_One.ShowClickArrow -= () => clickArrow.SetActive(true);
        tutorial_One.HideClickArrow -= () => clickArrow.SetActive(false);

        tutorial_One.IntroduceHeroInfo -= ShowTutorialOneMarkersOne;
        tutorial_One.IntroduceEnemyInfo -= ShowTutorialOneMarkersTwo;
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
    #endregion


    #region Events Two
    private void SubscribeEventsTwo() { }
    private void UnsubscribeEventsTwo() { }
    #endregion


    #region Events Three
    private void SubscribeEventsThree() { }
    private void UnsubscribeEventsThree() { }
    #endregion

    #region Events Four
    private void SubscribeEventsFour() { }
    private void UnsubscribeEventsFour() { }
    #endregion


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
