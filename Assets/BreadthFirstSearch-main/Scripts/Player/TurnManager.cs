using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pathfinder))]
public class TurnManager : MonoBehaviour
{
    #region Variables

    [Header("General Information:")]
    [SerializeField] public LayerMask tileLayer;
    [SerializeField] public Camera mainCam;

    [HideInInspector] public Pathfinder pathfinder;
    public List<Character> characterList;
    [HideInInspector] public List<Character> enemyList;

    StateInterface<TurnManager> currentTurn;

    #endregion

    #region UnityMethods

    void Start()
    {
        Debug.Assert(mainCam != null, "TileInteractor couldn't find the MainCamera component");

        pathfinder = gameObject.GetComponent<Pathfinder>();
        Debug.Assert(pathfinder != null, "TileInteractor couldn't find the PathFinder component");

        currentTurn = new PlayerTurn();
    }

    void Update()
    {
        currentTurn.UpdateState(this);
    }

    #endregion

    #region CustomMethods

    public void SwitchState(TurnEnums.TurnState state)
    {
        currentTurn.ExitState(this);

        switch (state)
        {
            case TurnEnums.TurnState.PlayerTurn:
                currentTurn = new PlayerTurn();
                break;

            case TurnEnums.TurnState.EnemyTurn:
                currentTurn = new EnemyTurn();  
                break;
        }

        currentTurn.EnterState(this);
    }

    #endregion
}
