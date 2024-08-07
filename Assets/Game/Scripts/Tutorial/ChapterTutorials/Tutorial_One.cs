using System;
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

    public event Action ShowClickArrow;
    public event Action HideClickArrow;

    public event Action IntroduceHeroInfo;
    public event Action IntroduceEnemyInfo;

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

                DisplayDialogue(turnOneDialogue, 0);

                RegainFullControl();

                internalTutorialStep++;
                break;

            case 1:
                if(dialogueJustEnded)
                {
                    DisplayDialogue(turnOneDialogue, 1, 2, 3, 4);
                    IntroduceHeroInfo?.Invoke();

                    internalTutorialStep++;
                }
                break;

            case 2:
                if(dialogueJustEnded)
                {
                    cameraController.MoveToTargetPosition(enemyOne.transform.position, true);

                    DisplayDialogue(turnOneDialogue, 5);

                    internalTutorialStep++;
                }
                break;

            case 3:
                if(dialogueJustEnded)
                {
                    AllowSpecificEnemySelection(enemyOne);
                    ShowClickArrow?.Invoke();
                }

                if(spawnedHighlight == null)
                {
                    spawnedHighlight = TemporaryMarker.GenerateMarker(highlightEffect, enemyOne.transform.position, 0f);
                }

                if(turnManager.PlayerTurn.SelectedEnemy == enemyOne)
                {
                    RegainFullControl();

                    Destroy(spawnedHighlight);

                    HideClickArrow?.Invoke();
                    internalTutorialStep++;
                }
                break;

            case 4:
                cameraController.MoveToTargetPosition(enemyOne.transform.position, true);
                IntroduceEnemyInfo?.Invoke();
                DisplayDialogue(turnOneDialogue, 6, 7, 8, 9);

                internalTutorialStep++;
                break;

            case 5:
                if(dialogueJustEnded)
                {
                    cameraController.MoveToTargetPosition(fireHero.transform.position, true);

                    AttackPreviewer.Instance.ClearAttackArea();

                    DisplayDialogue(turnOneDialogue, 10);

                    internalTutorialStep++;
                }
                break;

            case 6:
                if(dialogueJustEnded)
                {
                    AllowSpecificCharacterSelection(fireHero);
                    ShowClickArrow?.Invoke();
                }

                if (spawnedHighlight == null)
                {
                    spawnedHighlight = TemporaryMarker.GenerateMarker(highlightEffect, fireHero.transform.position, 0f);
                }

                if (turnManager.PlayerTurn.SelectedCharacter == fireHero)
                {
                    Destroy(spawnedHighlight);
                    RegainFullControl();

                    HideClickArrow?.Invoke();
                    internalTutorialStep++;
                }
                break;

            case 7:
                DisplayDialogue(turnOneDialogue, 11, 12);
                cameraController.controlEnabled = true;

                internalTutorialStep++;
                break;

            case 8:
                if(dialogueJustEnded)
                {
                    AllowSpecificTileSelection(lastTileOfFirstMove);
                }

                if (spawnedHighlight == null)
                {
                    spawnedHighlight = TemporaryMarker.GenerateMarker(highlightEffect, lastTileOfFirstMove.transform.position, 0.2f);
                }

                if(turnManager.PlayerTurn.Phase == TurnEnums.PlayerPhase.Attack)
                {
                    RegainFullControl();

                    Destroy(spawnedHighlight);

                    internalTutorialStep++;
                }
                break;

            case 9:
                DisplayDialogue(turnOneDialogue, 13);

                AllowSpecificEnemySelection(enemyOne);

                internalTutorialStep++;
                break;

            case 10:
                if(dialogueJustEnded)
                {
                    turnManager.PlayerTurn.preventAttack = false;
                }

                if (spawnedHighlight == null)
                {
                    spawnedHighlight = TemporaryMarker.GenerateMarker(highlightEffect, enemyOne.transform.position, 0f);
                }

                if (turnManager.PlayerTurn.Phase == TurnEnums.PlayerPhase.Execution)
                {
                    RegainFullControl();

                    Destroy(spawnedHighlight);

                    internalTutorialStep++;
                }
                break;

            case 11:
                if(turnManager.PlayerTurn.Phase != TurnEnums.PlayerPhase.Execution)
                {
                    DisplayDialogue(turnOneDialogue, 14);

                    internalTutorialStep++;
                }
                break;

            case 12:

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
