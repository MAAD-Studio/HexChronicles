using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_One : Tutorial_Base
{
    #region Variables

    [Header("Important Objects: ")]
    [SerializeField] private Character fireHero;

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
        switch(internalTutorialStep)
        {
            case 0:
                cameraController.MoveToTargetPosition(fireHero.transform.position, true);
                EnterDialogue(turnOneDialogue);
                internalTutorialStep++;
                break;

            case 1:
                break;
        }
    }

    #endregion
}
