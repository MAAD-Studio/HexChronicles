using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
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
    [SerializeField] private Tutorial_Base phaseOneTutorial;
    [SerializeField] private Tutorial_Base phaseTwoTutorial;

    #endregion

    #region UnityMethods

    private void Start()
    {
        //EventBus.Instance.Subscribe<OnTutorialStart>(OnTutorialStart);

        if (turnManager == null)
        {
            turnManager = FindObjectOfType<TurnManager>();
        }

        Debug.Assert(phaseOneTutorial != null, "TutorialManager has not been provided a Phase One Tutorial");
        Debug.Assert(phaseTwoTutorial != null, "TutorialManager has not been provided a Phase Two Tutorial");
    }

    private void OnDestroy()
    {
        //EventBus.Instance.Unsubscribe<OnTutorialStart>(OnTutorialStart);
    }

    private void Update()
    {
       switch(currentPhase)
       {
            case TutorialPhase.PhaseOne:
                phaseOneTutorial.ExecuteTutorial();
                break;

            case TutorialPhase.PhaseTwo:
                phaseTwoTutorial.ExecuteTutorial();
                break;
       } 
    }

    #endregion

    #region CustomMethods

    public void AdvancePhase()
    {
        if(currentPhase == TutorialPhase.PhaseOne)
        {
            currentPhase = TutorialPhase.PhaseTwo;
        }
        else if(currentPhase == TutorialPhase.PhaseTwo)
        {
            currentPhase = TutorialPhase.Completed;
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