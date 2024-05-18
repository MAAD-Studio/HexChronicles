using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTurnBase : StateInterface<TurnManager>
{
    #region Variables

    private TurnManager turnManager;

    #endregion

    #region StateInterfaceMethods

    public virtual void EnterState(TurnManager manager)
    {
        turnManager = manager;
    }

    public virtual void ExitState()
    {

    }

    public virtual void UpdateState()
    {

    }

    #endregion
}
