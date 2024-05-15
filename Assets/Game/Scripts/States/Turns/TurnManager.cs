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

    [Header("Characters on level:")]
    public List<Character> characterList;
    public List<Character> enemyList;

    StateInterface<TurnManager> currentTurn;

    [Header("Level Type:")]
    public TurnEnums.WorldTurns worldTurnStyle;

    #endregion

    #region UnityMethods

    void Start()
    {
        Debug.Assert(mainCam != null, "TileInteractor couldn't find the MainCamera component");

        pathfinder = gameObject.GetComponent<Pathfinder>();
        Debug.Assert(pathfinder != null, "TileInteractor couldn't find the PathFinder component");

        currentTurn = new PlayerTurn();
        currentTurn.EnterState(this);
    }

    void Update()
    {
        currentTurn.UpdateState();
    }

    #endregion

    #region CustomMethods

    public void SwitchState(TurnEnums.TurnState state)
    {
        currentTurn.ExitState();

        switch (state)
        {
            case TurnEnums.TurnState.PlayerTurn:
                currentTurn = new PlayerTurn();
                break;

            case TurnEnums.TurnState.EnemyTurn:
                currentTurn = new EnemyTurn();  
                break;

            case TurnEnums.TurnState.WorldTurn:
                currentTurn = WorldTurnChoice();
                break;
        }

        currentTurn.EnterState(this);
    }

    public StateInterface<TurnManager> WorldTurnChoice()
    {
        StateInterface<TurnManager> style;

        switch(worldTurnStyle)
        {
            case TurnEnums.WorldTurns.Towers:
                style = new TowersTurn();
                break;

            case TurnEnums.WorldTurns.NightSurvival:
                style = new NightSurvivalTurn();
                break;

            case TurnEnums.WorldTurns.RefugeeConvoy:
                style = new ConvoyTurn();
                break;

            default:
                style = new OutpostTurn();
                break;
        }

        return style;
    }

    //**DEBUG ONLY**
    public void DestroyACharacter(Character character)
    {
        character.characterTile.characterOnTile = null;
        character.characterTile.tileOccupied = false;
        characterList.Remove(character);
        Destroy(character.gameObject);
    }

    #endregion
}
