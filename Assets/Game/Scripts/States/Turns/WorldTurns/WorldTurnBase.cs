using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTurnBase : MonoBehaviour, StateInterface<TurnManager>
{
    #region Variables

    protected TurnManager turnManager;

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