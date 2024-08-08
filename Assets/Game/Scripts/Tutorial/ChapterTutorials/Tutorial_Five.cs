using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Five : Tutorial_Base
{
    #region Variables

    [Header("Important Objects: ")]
    [SerializeField] private Character fireHero;
    [SerializeField] private TileObject tower;

    [Header("Dialogue: ")]
    [SerializeField] private Dialogue[] turnOneDialogue;

    #endregion

    #region CustomMethods

    public override void ExecuteTutorial()
    {
        base.ExecuteTutorial();
        if (dialogueManager == null)
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
        switch (internalTutorialStep)
        {
            case 0:

                cameraController.MoveToTargetPosition(tower.transform.position, true);
                DisplayDialogue(turnOneDialogue, 0);
                RegainFullControl();

                internalTutorialStep++;

                break;

            case 1:
                if(dialogueJustEnded)
                {
                    AllowSpecificObjectSelection(tower);
                }

                if (spawnedHighlight == null)
                {
                    spawnedHighlight = TemporaryMarker.GenerateMarker(highlightEffect, tower.transform.position, 0f);
                }

                if (turnManager.PlayerTurn.SelectedObject == tower)
                {
                    Destroy(spawnedHighlight);

                    DisplayDialogue(turnOneDialogue, 1);
                    RegainFullControl();
                    internalTutorialStep++;
                }

                break;

            case 2:
                if(dialogueJustEnded)
                {
                    turnManager.disableEnd = false;
                    internalTutorialStep++;
                }
                break;

            case 3:
                if (turnManager.TurnType != TurnEnums.TurnState.PlayerTurn)
                {
                    internalTutorialStep++;
                }
                break;

            case 4:
                if (turnManager.TurnType == TurnEnums.TurnState.PlayerTurn)
                {
                    cameraController.MoveToTargetPosition(fireHero.transform.position, true);
                    DisplayDialogue(turnOneDialogue, 2);
                    RegainFullControl();
                    internalTutorialStep++;
                }
                break;

            case 5:
                if (dialogueJustEnded)
                {
                    turnManager.disableEnd = false;
                    internalTutorialStep++;
                }
                break;

            case 6:
                if (turnManager.TurnType != TurnEnums.TurnState.PlayerTurn)
                {
                    internalTutorialStep++;
                }
                break;

            case 7:
                if (turnManager.TurnType == TurnEnums.TurnState.PlayerTurn)
                {
                    cameraController.MoveToTargetPosition(fireHero.transform.position, true);
                    DisplayDialogue(turnOneDialogue, 3);
                    RegainFullControl();
                    internalTutorialStep++;
                }
                break;

            case 8:
                if ((dialogueJustEnded))
                {
                    cameraController.MoveToTargetPosition(tower.transform.position, true);
                    DisplayDialogue(turnOneDialogue, 4, 5);
                    internalTutorialStep++;
                }
                break;

            case 9:
                if (dialogueJustEnded)
                {
                    turnManager.EndLevel();
                    internalTutorialStep++;
                }
                break;
        }

    }

    #endregion
}
