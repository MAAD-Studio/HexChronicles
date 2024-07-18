using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_One : Tutorial_Base
{
    #region Variables

    [Header("Important Objects: ")]
    [SerializeField] private Character fireHero;
    [SerializeField] private Enemy_Base enemyOne;
    [SerializeField] private Tile lastTileOfFirstMove;

    [Header("Dialogue: ")]
    [SerializeField] private Dialogue[] turnOneDialogue;

    #endregion

    #region CustomMethods

    public override void ExecuteTutorial()
    {
        base.ExecuteTutorial();
        if(dialogueManager == null)
        {
            return;
        }

        switch (currentTurn)
        {
            case TutorialTurn.TurnOne:
                ExecuteTurnOne();
                break;

            case TutorialTurn.TurnTwo:
                break;

            case TutorialTurn.TurnThree: 
                break;

            case TutorialTurn.TurnFour: 
                break;

            case TutorialTurn.TurnFive: 
                break;

            case TutorialTurn.PostBattle:
                break;

            case TutorialTurn.Completed:
                tutorialManager.AdvancePhase();
                break;
        }

        if (dialogueJustEnded)
        {
            dialogueJustEnded = false;
        }
    }

    protected override void ExecuteTurnOne()
    {
        List<Dialogue> dialogueToDisplay = new List<Dialogue>();
        switch(internalTutorialStep)
        {
            case 0:
                dialogueToDisplay.Add(turnOneDialogue[0]);
                EnterDialogue(dialogueToDisplay.ToArray());

                turnManager.disablePlayers = true;
                turnManager.disableObjects = true;
                turnManager.disableEnemies = true;

                internalTutorialStep++;
                break;

            case 1:
                if(dialogueJustEnded)
                {
                    cameraController.MoveToTargetPosition(fireHero.transform.position, true);

                    dialogueToDisplay.Add(turnOneDialogue[1]);
                    dialogueToDisplay.Add(turnOneDialogue[2]);
                    EnterDialogue(dialogueToDisplay.ToArray());

                    internalTutorialStep++;
                }
                break;

            case 2:
                if(dialogueJustEnded)
                {
                    cameraController.MoveToTargetPosition(enemyOne.transform.position, true);

                    dialogueToDisplay.Add(turnOneDialogue[3]);
                    EnterDialogue(dialogueToDisplay.ToArray());

                    turnManager.disableEnemies = false;
                    turnManager.PlayerTurn.desiredEnemy = enemyOne;

                    internalTutorialStep++;
                }
                break;

            case 3:
                if(spawnedHighlight == null)
                {
                    spawnedHighlight = TemporaryMarker.GenerateMarker(highlightEffect, enemyOne.transform.position, 0f);
                }

                if(turnManager.PlayerTurn.SelectedEnemy == enemyOne)
                {
                    turnManager.disableEnemies = true;
                    turnManager.PlayerTurn.desiredEnemy = null;

                    Destroy(spawnedHighlight);

                    internalTutorialStep++;
                }
                break;

            case 4:
                dialogueToDisplay.Add(turnOneDialogue[4]);
                dialogueToDisplay.Add(turnOneDialogue[5]);
                dialogueToDisplay.Add(turnOneDialogue[6]);
                EnterDialogue(dialogueToDisplay.ToArray());

                internalTutorialStep++;
                break;

            case 5:
                if(dialogueJustEnded)
                {
                    cameraController.MoveToTargetPosition(fireHero.transform.position, true);

                    AttackPreviewer.Instance.ClearAttackArea();

                    dialogueToDisplay.Add(turnOneDialogue[7]);
                    EnterDialogue(dialogueToDisplay.ToArray());

                    turnManager.disablePlayers = false;
                    turnManager.PlayerTurn.preventPhaseBackUp = true;

                    internalTutorialStep++;
                }
                break;

            case 6:
                if (spawnedHighlight == null)
                {
                    spawnedHighlight = TemporaryMarker.GenerateMarker(highlightEffect, fireHero.transform.position, 0f);
                }

                if (turnManager.PlayerTurn.SelectedCharacter == fireHero)
                {
                    Destroy(spawnedHighlight);

                    internalTutorialStep++;
                }
                break;

            case 7:
                dialogueToDisplay.Add(turnOneDialogue[8]);
                EnterDialogue(dialogueToDisplay.ToArray());

                turnManager.PlayerTurn.desiredTile = lastTileOfFirstMove;

                internalTutorialStep++;
                break;

            case 8:
                if (spawnedHighlight == null)
                {
                    spawnedHighlight = TemporaryMarker.GenerateMarker(highlightEffect, lastTileOfFirstMove.transform.position, 0.2f);
                }

                if(turnManager.PlayerTurn.Phase == TurnEnums.PlayerPhase.Attack)
                {
                    turnManager.PlayerTurn.desiredTile = null;

                    Destroy(spawnedHighlight);

                    internalTutorialStep++;
                }
                break;

            case 9:
                dialogueToDisplay.Add(turnOneDialogue[9]);
                EnterDialogue(dialogueToDisplay.ToArray());

                turnManager.PlayerTurn.desiredEnemy = enemyOne;

                internalTutorialStep++;
                break;

            case 10:
                if (spawnedHighlight == null)
                {
                    spawnedHighlight = TemporaryMarker.GenerateMarker(highlightEffect, enemyOne.transform.position, 0f);
                }

                if (turnManager.PlayerTurn.Phase == TurnEnums.PlayerPhase.Execution)
                {
                    turnManager.disablePlayers = true;
                    turnManager.PlayerTurn.preventPhaseBackUp = false;

                    Destroy(spawnedHighlight);

                    internalTutorialStep++;
                }
                break;

            case 11:
                dialogueToDisplay.Add(turnOneDialogue[10]);
                EnterDialogue(dialogueToDisplay.ToArray());

                internalTutorialStep++;
                break;
        }
    }

    #endregion
}
