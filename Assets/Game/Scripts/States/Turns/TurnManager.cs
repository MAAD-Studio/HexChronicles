using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pathfinder), typeof(EnemyBrain))]
public class TurnManager : MonoBehaviour
{
    #region Variables

    [Header("General Information:")]
    [SerializeField] public LayerMask tileLayer;
    [SerializeField] public Camera mainCam;

    [Header("Characters on level:")]
    public List<Character> characterList;
    public List<Enemy_Base> enemyList;

    [Header("Level Type:")]
    public TurnEnums.WorldTurns worldTurnStyle;

    [HideInInspector] public Pathfinder pathfinder;
    [HideInInspector] public EnemyBrain enemyBrain;

    private StateInterface<TurnManager> playerTurn;
    private StateInterface<TurnManager> enemyTurn;
    private StateInterface<TurnManager> worldTurn;

    private StateInterface<TurnManager> currentTurn;

    #endregion

    #region UnityMethods

    void Start()
    {
        Debug.Assert(mainCam != null, "TileInteractor couldn't find the MainCamera component");

        pathfinder = gameObject.GetComponent<Pathfinder>();
        Debug.Assert(pathfinder != null, "TileInteractor couldn't find the PathFinder component");

        enemyBrain = gameObject.GetComponent<EnemyBrain>();
        Debug.Assert(enemyBrain != null, "TileInteractor couldn't find the EnemyBrain component");

        playerTurn = new PlayerTurn();
        enemyTurn = new EnemyTurn();
        worldTurn = WorldTurnChoice();

        currentTurn = playerTurn;
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
                currentTurn = playerTurn;
                break;

            case TurnEnums.TurnState.EnemyTurn:
                currentTurn = enemyTurn;
                break;

            case TurnEnums.TurnState.WorldTurn:
                currentTurn = worldTurn;
                break;
        }

        currentTurn.EnterState(this);
    }

    //Creates an instance of the selected WorldTurn type on startup
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

    //**TESTING ONLY**
    public void DestroyACharacter(Character character)
    {
        character.characterTile.characterOnTile = null;
        character.characterTile.tileOccupied = false;
        characterList.Remove(character);
        Destroy(character.gameObject);
    }

    #endregion
}
