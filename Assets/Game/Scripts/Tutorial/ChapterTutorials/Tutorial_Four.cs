using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Four : Tutorial_Base
{
    #region Variables

    [Header("Important Objects: ")]
    [SerializeField] private Character fireHero;
    [SerializeField] private Tile waterTile;

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

        if (endTurnButton == null)
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
        switch(internalTutorialStep)
        {
            case 0:

                cameraController.MoveToTargetPosition(fireHero.transform.position, true);
                DisplayDialogue(turnOneDialogue, 0);
                RegainFullControl();

                endTurnButton.DisableButton();

                internalTutorialStep++;

                break;

            case 1:
                if(dialogueJustEnded)
                {
                    turnManager.disableEnd = false;
                    endTurnButton.EnableButton();
                    internalTutorialStep++;
                }
                break;

            case 2:
                if(turnManager.TurnType != TurnEnums.TurnState.PlayerTurn)
                {
                    internalTutorialStep++;
                    endTurnButton.DisableButton();
                }
                break;

            case 3:
                if(turnManager.TurnType == TurnEnums.TurnState.PlayerTurn)
                {
                    cameraController.MoveToTargetPosition(waterTile.transform.position, true);
                    DisplayDialogue(turnOneDialogue, 1, 2);
                    RegainFullControl();
                    internalTutorialStep++;
                }
                break;

            case 4:
                if (dialogueJustEnded)
                {
                    turnManager.disableEnd = false;
                    endTurnButton.EnableButton();
                    internalTutorialStep++;
                }
                break;

            case 5:
                if (turnManager.TurnType != TurnEnums.TurnState.PlayerTurn)
                {
                    internalTutorialStep++;
                    endTurnButton.DisableButton();
                }
                break;

            case 6:
                if (turnManager.TurnType == TurnEnums.TurnState.PlayerTurn)
                {
                    cameraController.MoveToTargetPosition(fireHero.transform.position, true);
                    DisplayDialogue(turnOneDialogue, 3);
                    RegainFullControl();
                    internalTutorialStep++;
                }
                break;

            case 7:
                if(dialogueJustEnded)
                {
                    cameraController.MoveToTargetPosition(waterTile.transform.position, true);
                    DisplayDialogue(turnOneDialogue, 4, 5);
                    internalTutorialStep++;
                }
                break;

            case 8:
                if (dialogueJustEnded)
                {
                    EventBus.Instance.Publish(new OnTutorialEnd());
                    turnManager.Victory();
                    internalTutorialStep++;
                }
                break;
        }
    }

    #endregion
}
