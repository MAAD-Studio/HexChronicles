using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurn : StateInterface<TurnManager>
{
    #region Variables

    private TurnManager turnManager;

    #endregion

    #region StateInterfaceMethods

    public void EnterState(TurnManager manager)
    {
        turnManager = manager;
    }

    public void ExitState()
    {
        
    }

    public void UpdateState()
    {
        Debug.Log("WE ARE NOW DOING ENEMY THINGS");

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            turnManager.SwitchState(TurnEnums.TurnState.PlayerTurn);
        }
    }

    #endregion
}
