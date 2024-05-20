using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyTurn : StateInterface<TurnManager>
{
    #region Variables

    private TurnManager turnManager;

    bool RunEnemyAI = true;

    #endregion

    #region StateInterfaceMethods

    public void EnterState(TurnManager manager)
    {
        turnManager = manager;

        RunEnemyAI = true;

        // Apply Enemy's Status
        foreach (Enemy_Base enemy in turnManager.enemyList)
        {
            if (enemy.statusList.Count > 0)
            {
                enemy.ApplyStatus();
            }
        }
    }

    public void ExitState()
    {
        foreach(Character enemy in turnManager.enemyList)
        {
            enemy.movementThisTurn = 0;
        }
    }

    public void UpdateState()
    {
        if(RunEnemyAI)
        {
            turnManager.enemyBrain.CalculateEnemyTurns();
            RunEnemyAI = false;
        }

        if(turnManager.enemyBrain.DecisionMakingFinished)
        {
            turnManager.SwitchState(TurnEnums.TurnState.PlayerTurn);
        }
    }

    #endregion
}
