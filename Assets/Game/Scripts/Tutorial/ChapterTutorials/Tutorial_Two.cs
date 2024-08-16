using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Two : Tutorial_Base
{
    #region Variables

    [Header("Important Objects: ")]
    [SerializeField] private Character fireHero;
    [SerializeField] private Character waterHero;
    [SerializeField] private Tile whirlpoolTile;
    [SerializeField] private Character enemyToHit;
    [SerializeField] private Tile meteorMovementTile;

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
        switch (internalTutorialStep)
        {
            case 0:
                cameraController.MoveToTargetPosition(fireHero.transform.position, true);

                DisplayDialogue(turnOneDialogue, 0);
                RegainFullControl();

                endTurnButton.HideEndTurn();

                internalTutorialStep++;
                break;

            case 1:
                if (dialogueJustEnded)
                {
                    cameraController.MoveToTargetPosition(waterHero.transform.position, true);

                    DisplayDialogue(turnOneDialogue, 1);

                    internalTutorialStep++;
                }
                break;

            case 2:
                if(dialogueJustEnded)
                {
                    AllowSpecificCharacterSelection(waterHero);
                }

                if (spawnedHighlight == null)
                {
                    spawnedHighlight = TemporaryMarker.GenerateMarker(highlightEffect, waterHero.transform.position, 0f);
                }

                if (turnManager.PlayerTurn.SelectedCharacter == waterHero)
                {
                    Destroy(spawnedHighlight);

                    RegainFullControl();

                    internalTutorialStep++;
                }
                break;

            case 3:
                cameraController.MoveToTargetPosition(waterHero.transform.position, true);
                DisplayDialogue(turnOneDialogue, 2);

                internalTutorialStep++;
                break;

            case 4:
                if (dialogueJustEnded)
                {
                    AllowSpecificTileSelection(waterHero.characterTile);
                }

                if (spawnedHighlight == null)
                {
                    spawnedHighlight = TemporaryMarker.GenerateMarker(highlightEffect, waterHero.transform.position, 0f);
                }

                if (turnManager.PlayerTurn.Phase == TurnEnums.PlayerPhase.Attack)
                {
                    Destroy(spawnedHighlight);

                    RegainFullControl();

                    internalTutorialStep++;
                }
                break;

            case 5:
                DisplayDialogue(turnOneDialogue, 3);

                internalTutorialStep++;
                break;

            case 6:

                if(dialogueJustEnded)
                {
                    ChangeActiveInteractability(waterHero, true);
                }

                if(turnManager.PlayerTurn.AttackType == TurnEnums.PlayerAction.ActiveSkill)
                {
                    ChangeActiveInteractability(waterHero, false);
                    internalTutorialStep++;
                }
                
                break;

            case 7:
                cameraController.MoveToTargetPosition(whirlpoolTile.transform.position, true);
                DisplayDialogue(turnOneDialogue, 4);
                internalTutorialStep++;
                break;

            case 8:
                if(dialogueJustEnded)
                {
                    AllowSpecificTileAttack(whirlpoolTile);
                }

                if (spawnedHighlight == null)
                {
                    spawnedHighlight = TemporaryMarker.GenerateMarker(highlightEffect, whirlpoolTile.transform.position, 0f);
                }

                if (turnManager.PlayerTurn.Phase == TurnEnums.PlayerPhase.Execution)
                {
                    RegainFullControl();

                    Destroy(spawnedHighlight);

                    internalTutorialStep++;
                }
                break;

            case 9:
                if(turnManager.PlayerTurn.Phase != TurnEnums.PlayerPhase.Execution)
                {
                    cameraController.MoveToTargetPosition(fireHero.transform.position, true);
                    DisplayDialogue(turnOneDialogue, 5);
                    internalTutorialStep++;
                }
                break;

            case 10:
                if (dialogueJustEnded)
                {
                    AllowSpecificCharacterSelection(fireHero);
                    ChangeActiveInteractability(fireHero, true);
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

            case 11:
         
                AllowSpecificTileSelection(meteorMovementTile);

                if (spawnedHighlight == null)
                {
                    spawnedHighlight = TemporaryMarker.GenerateMarker(highlightEffect, meteorMovementTile.transform.position, 0f);
                }

                if (turnManager.PlayerTurn.Phase == TurnEnums.PlayerPhase.Attack)
                {
                    cameraController.MoveToTargetPosition(meteorMovementTile.transform.position, true);

                    Destroy(spawnedHighlight);

                    RegainFullControl();

                    internalTutorialStep++;
                }
                break;

            case 12:
                if (spawnedHighlight == null)
                {
                    spawnedHighlight = TemporaryMarker.GenerateMarker(highlightEffect, enemyToHit.transform.position, 0f);
                }

                if(turnManager.PlayerTurn.AttackType == TurnEnums.PlayerAction.ActiveSkill)
                {
                    AllowSpecificEnemySelection(enemyToHit);
                    turnManager.PlayerTurn.preventAttack = false;
                }

                if (turnManager.PlayerTurn.Phase == TurnEnums.PlayerPhase.Execution)
                {
                    RegainFullControl();
                    ChangeActiveInteractability(fireHero, false);

                    Destroy(spawnedHighlight);

                    internalTutorialStep++;
                }
                break;

            case 13:

                cameraController.FollowTarget(fireHero.transform, true);
                if (turnManager.enemyList.Count <= 0)
                {
                    DisplayDialogue(turnOneDialogue, 6);
                    internalTutorialStep++;
                }
                break;

            case 14:
                if(dialogueJustEnded)
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
