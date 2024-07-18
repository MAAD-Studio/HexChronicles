using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_One : Tutorial_Base
{
    #region Variables

    [Header("Important Objects: ")]
    [SerializeField] private Character fireHero;
    [SerializeField] private Enemy_Base enemyOne;

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
                internalTutorialStep++;
                turnManager.disablePlayers = true;
                turnManager.disableObjects = true;
                turnManager.disableEnemies = true;
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
                    internalTutorialStep++;
                    turnManager.disableEnemies = false;
                }
                break;

            case 3:
                if(turnManager.PlayerTurn.SelectedEnemy == enemyOne)
                {
                    internalTutorialStep++;
                }
                break;

            case 4:
                dialogueToDisplay.Add(turnOneDialogue[4]);
                EnterDialogue(dialogueToDisplay.ToArray());
                internalTutorialStep++;
                break;
        }
    }

    #endregion
}
