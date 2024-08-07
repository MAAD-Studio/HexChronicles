using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Tutorial_Base : MonoBehaviour
{
    #region Variables

    [SerializeField] protected GameObject highlightEffect;
    protected GameObject spawnedHighlight;

    [SerializeField] protected GameObject tilesParentObject;
    protected List<Tile> mapTiles = new List<Tile>();

    protected TurnManager turnManager;
    protected TutorialManager tutorialManager;
    protected DialogueManager dialogueManager;
    protected CameraController cameraController;

    protected TutorialTurn currentTurn = TutorialTurn.TurnOne;
    protected int internalTutorialStep = 0;
    protected bool dialogueJustEnded = false;

    public static UnityEvent TutorialFullControl = new UnityEvent();

    #endregion

    #region UnityMethods

    private void Start()
    {
        EventBus.Instance.Subscribe<OnDialogueEnded>(OnDialogueEnd);

        turnManager = FindObjectOfType<TurnManager>();
        Debug.Assert(turnManager != null, "Tutorial failed to locate a TurnManager");
  
        tutorialManager = FindObjectOfType<TutorialManager>();
        Debug.Assert(tutorialManager != null, "Tutorial failed to locate a TutorialManager");
        
        dialogueManager = FindObjectOfType<DialogueManager>();

        cameraController = FindObjectOfType<CameraController>();
        Debug.Assert(cameraController != null, "Tutorial failed to locate a CameraController");

        mapTiles = tilesParentObject.GetComponentsInChildren<Tile>().ToList();
    }

    #endregion

    #region CustomMethods

    public virtual void ExecuteTutorial()
    {
        if (dialogueManager == null)
        {
            dialogueManager = FindObjectOfType<DialogueManager>();
        }
    }

    protected virtual void ExecuteTurnOne()
    {
    }

    protected virtual void ExecuteTurnTwo()
    {
    }

    protected virtual void ExecuteTurnThree()
    {
    }

    protected virtual void ExecuteTurnFour()
    {
    }

    protected virtual void ExecuteTurnFive()
    {
    }

    protected virtual void ExecuteTurnSix()
    {
    }

    protected virtual void ExecutePostBattle()
    {
    }

    protected void EnterDialogue(Dialogue[] dialogue)
    {
        cameraController.controlEnabled = false;
        turnManager.pauseTurns = true;
        dialogueManager.StartDialogue(dialogue);
    }

    protected void OnDialogueEnd(object obj)
    {
        dialogueJustEnded = true;
        turnManager.pauseTurns = false;
        cameraController.controlEnabled = true;
    }

    protected void RegainFullControl()
    {
        TutorialFullControl?.Invoke();
        turnManager.PlayerTurn.preventPhaseBackUp = true;
        turnManager.disablePlayers = true;
        turnManager.disableObjects = true;
        turnManager.disableEnemies = true;
        turnManager.disableEnd = true;
        turnManager.PlayerTurn.preventAttack = true;

        turnManager.PlayerTurn.ResetTutorialSelects();
    }

    protected void DisplayDialogue(Dialogue[] dialogue, params int[] dialogueIndexes)
    {
        List<Dialogue> dialogueToDisplay = new List<Dialogue>();
        foreach (int index in dialogueIndexes)
        {
            dialogueToDisplay.Add(dialogue[index]);
        }
        EnterDialogue(dialogueToDisplay.ToArray());
    }

    protected void AllowSpecificCharacterSelection(Character character)
    {
        turnManager.PlayerTurn.desiredCharacter = character;
        turnManager.disablePlayers = false;
    }

    protected void AllowSpecificTileSelection(Tile tile)
    {
        turnManager.PlayerTurn.desiredTile = tile;
    }

    protected void AllowSpecificTileAttack(Tile tile)
    {
        turnManager.PlayerTurn.preventAttack = false;
        turnManager.PlayerTurn.desiredAttackTile = tile;
    }

    protected void AllowSpecificEnemySelection(Character character)
    {
        turnManager.PlayerTurn.desiredEnemy = character;
        turnManager.disableEnemies = false;
    }

    protected void ChangeActiveInteractability(Character character, bool enable)
    {
        ChangeActiveInteract activeData = new ChangeActiveInteract();
        activeData.character = character;
        activeData.enable =enable;
        EventBus.Instance.Publish(activeData);
    }

    #endregion
}
