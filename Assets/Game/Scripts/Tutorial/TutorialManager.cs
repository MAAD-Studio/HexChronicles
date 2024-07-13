using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
{
    [SerializeField] private TurnManager turnManager;

    public enum TutorialPhase
    {
        Phase1,
        Phase2,
        Completed
    }

    [SerializeField] private TutorialPhase currentPhase;
    private int currentStep;

    [Header("Tutorial Tips")]
    private TutorialTip[] tutorialCollection;
    private Queue<TutorialTip> tutorialTips;

    [Header("Dialogue")]
    [SerializeField] private Dialogue[] startDialogue;
    private DialogueManager dialogueManager;

    private void Start()
    {
        EventBus.Instance.Subscribe<OnTutorialStart>(OnTutorialStart);
    }

    private void OnDestroy()
    {
        EventBus.Instance.Unsubscribe<OnTutorialStart>(OnTutorialStart);
    }

    private void OnTutorialStart(object obj)
    {
        tutorialTips = new Queue<TutorialTip>();

        /*tutorialCollection = GetComponentsInChildren<TutorialTip>();
        foreach (TutorialTip tutorial in tutorialCollection)
        {
            tutorialTips.Enqueue(tutorial);
        }*/

        if (turnManager == null)
        {
            turnManager = FindObjectOfType<TurnManager>();
        }

        StartPhase1();
    }

    private void StartPhase1()
    {
        currentPhase = TutorialPhase.Phase1;

        // Start Dialogue
        dialogueManager = FindObjectOfType<DialogueManager>();
        dialogueManager.StartDialogue(startDialogue);

        currentStep = 0;
        StartStep();
    }

    private void StartStep()
    {
        switch (currentStep)
        {
            case 0:
                ShowUIBasics();
                break;
            case 1:
                TeachMovementAndAttack();
                break;
            case 2:
                TeachTileBuffs();
                break;
            case 3:
                TeachTileDebuffs();
                break;
            case 4:
                ShowPostBattleUI();
                break;
        }
    }

    public void NextStep()
    {
        currentStep++;
        StartStep();
    }

    private void ShowUIBasics()
    {
        Debug.Log("Show UI Basics");
    }

    private void TeachMovementAndAttack()
    {
        Debug.Log("Teach Movement and Attack");
    }

    private void TeachTileBuffs()
    {
        Debug.Log("Teach Tile Buffs");
    }

    private void TeachTileDebuffs()
    {
        Debug.Log("Teach Tile Debuffs");
    }

    private void ShowPostBattleUI()
    {
        Debug.Log("Show Post Battle UI");
    }

    public void CompleteTutorial()
    {
        currentPhase = TutorialPhase.Completed;
        Debug.Log("Tutorial Completed");
    }
}