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
    [HideInInspector] public Pathfinder pathfinder;
    [HideInInspector] public EnemyBrain enemyBrain;

    private StateInterface<TurnManager> playerTurn;
    private StateInterface<TurnManager> enemyTurn;
    [SerializeField] private WorldTurnBase worldTurn;

    private StateInterface<TurnManager> currentTurn;

    private int turnNumber;
    public int TurnNumber
    {
        get { return turnNumber; }
    }

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
        turnNumber = 1;

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
                turnNumber++;
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

    //**TESTING ONLY**
    public void DestroyACharacter(Character character)
    {
        character.characterTile.characterOnTile = null;
        character.characterTile.tileOccupied = false;

        if (character.characterType == TurnEnums.CharacterType.Player)
        {
            characterList.Remove(character);
        }
        else
        {
            enemyList.Remove((Enemy_Base)character);
        }

        Destroy(character.gameObject);
    }

    #endregion
}