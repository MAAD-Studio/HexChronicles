using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TurnManager))]
public class WorldTurnBase : MonoBehaviour, StateInterface
{
    #region Variables

    protected TurnManager turnManager;
    [SerializeField] protected WeatherManager weatherManager;

    #endregion

    #region UnityMethods

    private void Start()
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

    #endregion
}