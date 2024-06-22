using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(TurnManager))]
public class WorldTurnBase : MonoBehaviour, StateInterface
{
    #region Variables

    protected TurnManager turnManager;
    public static UnityEvent Victory = new UnityEvent();

    #endregion

    #region UnityMethods

    protected virtual void Start()
    {
        turnManager = GetComponent<TurnManager>();
        Debug.Assert(turnManager != null, "WorldTurn doesn't have a TurnManager");
    }

    #endregion

    #region StateInterfaceMethods

    public virtual void EnterState()
    {
        
    }

    public virtual void ExitState()
    {

    }

    public virtual void UpdateState()
    {
       
    }

    public virtual void ResetState()
    {
        
    }

    #endregion
}