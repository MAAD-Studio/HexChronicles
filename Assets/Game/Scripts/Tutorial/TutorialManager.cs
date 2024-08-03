using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    #region Variables

    [SerializeField] private TurnManager turnManager;

    public enum TutorialPhase
    {
        PhaseOne,
        PhaseTwo,
        Completed
    }
    private TutorialPhase currentPhase = TutorialPhase.PhaseOne;

    private int currentStep;

    [Header("Tutorial Tips")]
    private TutorialTip[] tutorialCollection;
    private Queue<TutorialTip> tutorialTips;

    [Header("Dialogue")]
    private DialogueManager dialogueManager;
    [SerializeField] private Dialogue[] turnOneDialogue;
    [SerializeField] private Dialogue[] startDialogue;

    [Header("Tutorials")]
    [SerializeField] List<Tutorial_Base> tutorials;
    Tutorial_Base selectedTutorial;
    private int tutorialIndex = 0;

    #endregion

    #region UnityMethods

    private void Start()
    {
        //EventBus.Instance.Subscribe<OnTutorialStart>(OnTutorialStart);

        if (turnManager == null)
        {
            turnManager = FindObjectOfType<TurnManager>();
        }

        if(tutorials.Count > 0)
        {
            selectedTutorial = tutorials[0];
        }
    }

    private void OnDestroy()
    {
        //EventBus.Instance.Unsubscribe<OnTutorialStart>(OnTutorialStart);
    }

    private void Update()
    {
        if(tutorials.Count > 0)
        {
            selectedTutorial.ExecuteTutorial();
        }
    }

    #endregion

    #region CustomMethods

    public void AdvancePhase()
    {
        if(tutorialIndex == tutorials.Count - 1)
        {
            tutorialIndex = 0;
        }
        else
        {
            tutorialIndex++;
        }
    }

    #endregion

   /* private void OnTutorialStart(object obj)
    {
        tutorialTips = new Queue<TutorialTip>();

        tutorialCollection = GetComponentsInChildren<TutorialTip>();
        foreach (TutorialTip tutorial in tutorialCollection)
        {
            tutorialTips.Enqueue(tutorial);
        }

        StartPhase1();
    }

    private void StartPhase1()
    {
        currentPhase = TutorialPhase.PhaseOne;

        // Start Dialogue
        //dialogueManager = FindObjectOfType<DialogueManager>();
        //dialogueManager.StartDialogue(startDialogue);

        currentStep = 0;
        StartStep();
    }*/
}