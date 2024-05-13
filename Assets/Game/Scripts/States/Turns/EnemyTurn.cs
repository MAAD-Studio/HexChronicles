using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurn : StateInterface<TurnManager>
{
    #region Variables

    #endregion

    #region StateInterfaceMethods

    public void EnterState(TurnManager manager)
    {
        
    }

    public void ExitState(TurnManager manager)
    {
        
    }

    public void UpdateState(TurnManager manager)
    {
        Debug.Log("WE ARE NOW DOING ENEMY THINGS");

        if (Input.GetKeyDown(KeyCode.R))
        {
            manager.SwitchState(TurnEnums.TurnState.PlayerTurn);
        }
    }

    #endregion
}
