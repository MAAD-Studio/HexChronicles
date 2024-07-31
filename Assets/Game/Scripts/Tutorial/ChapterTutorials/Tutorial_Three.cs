using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Three : Tutorial_Base
{
    #region Variables

    [Header("Important Objects: ")]
    [SerializeField] private Character fireHero;
    [SerializeField] private Tile lavaTile;
    [SerializeField] private Tile buffDemoTile;
    [SerializeField] private Tile debuffDemoTile;
    [SerializeField] private Character enemyToHit;

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
        switch(internalTutorialStep)
        {
            case 0:
                cameraController.MoveToTargetPosition(fireHero.transform.position, true);
                DisplayDialogue(turnOneDialogue, 0, 1);
                RegainFullControl();

                enemyToHit.currentHealth += 1;
                enemyToHit.UpdateHealthBar?.Invoke();

                internalTutorialStep++;
                break;

            case 1:
                if(dialogueJustEnded)
                {
                    AllowSpecificCharacterSelection(fireHero);
                }

                if (spawnedHighlight == null)
                {
                    spawnedHighlight = TemporaryMarker.GenerateMarker(highlightEffect, fireHero.transform.position, 0f);
                }

                if (turnManager.PlayerTurn.SelectedCharacter == fireHero)
                {
                    Destroy(spawnedHighlight);

                    RegainFullControl();

                    internalTutorialStep++;
                }
                break;

            case 2:
                cameraController.MoveToTargetPosition(debuffDemoTile.transform.position, true);
                DisplayDialogue(turnOneDialogue, 2);

                internalTutorialStep++;
                break;

            case 3:
                if(dialogueJustEnded)
                {
                    cameraController.MoveToTargetPosition(buffDemoTile.transform.position, true);
                    DisplayDialogue(turnOneDialogue, 3);

                    internalTutorialStep++;
                }
                break;

            case 4:
                if (dialogueJustEnded)
                {
                    AllowSpecificTileSelection(lavaTile);
                }

                if (spawnedHighlight == null)
                {
                    spawnedHighlight = TemporaryMarker.GenerateMarker(highlightEffect, lavaTile.transform.position, 0f);
                }

                if (turnManager.PlayerTurn.Phase == TurnEnums.PlayerPhase.Attack)
                {
                    cameraController.MoveToTargetPosition(lavaTile.transform.position, true);

                    Destroy(spawnedHighlight);

                    RegainFullControl();

                    AllowSpecificEnemySelection(enemyToHit);
                    turnManager.PlayerTurn.preventAttack = false;

                    internalTutorialStep++;
                }
                break;

            case 5:
                if (spawnedHighlight == null)
                {
                    spawnedHighlight = TemporaryMarker.GenerateMarker(highlightEffect, enemyToHit.transform.position, 0f);
                }

                if (turnManager.PlayerTurn.Phase == TurnEnums.PlayerPhase.Execution)
                {
                    RegainFullControl();

                    Destroy(spawnedHighlight);

                    internalTutorialStep++;
                }
                break;

            case 6:
                if(turnManager.PlayerTurn.Phase != TurnEnums.PlayerPhase.Execution)
                {
                    DisplayDialogue(turnOneDialogue, 4);
                    turnManager.disableEnd = false;
                    internalTutorialStep++;
                }
                break;

            case 7:
                if (turnManager.enemyList.Count <= 0)
                {
                    RegainFullControl();
                    cameraController.MoveToTargetPosition(fireHero.transform.position, true);
                    DisplayDialogue(turnOneDialogue, 5, 6);
                    internalTutorialStep++;
                }
                break;

            case 8:
                if(dialogueJustEnded)
                {
                    turnManager.EndLevel();
                    internalTutorialStep++;
                }
                break;
        }
    }

    #endregion
}
