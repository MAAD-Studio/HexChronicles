using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(TurnManager))]
public class EnemyTurn : MonoBehaviour, StateInterface
{
    #region Variables

    private TurnManager turnManager;

    bool RunEnemyAI = true;

    #endregion

    #region UnityMethods

    private void Start()
    {
        turnManager = GetComponent<TurnManager>();
        Debug.Assert(turnManager != null, "EnemyTurn doesn't have a TurnManager");
    }

    #endregion

    #region StateInterfaceMethods

    public void EnterState()
    {
        RunEnemyAI = true;

        foreach (Enemy_Base enemy in turnManager.enemyList)
        {
            enemy.EnterNewTurn();
        }
        EventBus.Instance.Publish(new OnEnemyTurn());
    }

    public void ExitState()
    {
        foreach (Character enemy in turnManager.enemyList)
        {
            enemy.movementThisTurn = 0;
        }
    }

    public void UpdateState()
    {
        if (RunEnemyAI)
        {
            turnManager.enemyBrain.CalculateEnemyTurns();
            RunEnemyAI = false;
        }

        if (turnManager.enemyBrain.DecisionMakingFinished)
        {
            turnManager.mainCameraController.StopFollowingTarget();
            turnManager.SwitchState(TurnEnums.TurnState.WorldTurn);
        }
    }

    public void ResetState()
    {

    }

    #endregion
}