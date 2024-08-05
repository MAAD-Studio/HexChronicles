using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerTurn), typeof(EnemyTurn), typeof(WeatherTurn))]
public class TurnManager : MonoBehaviour
{
    #region Variables

    [Header("General Information:")]
    [SerializeField] public LayerMask tileLayer;

    [Header("External Components: ")]
    [SerializeField] public Camera mainCam;
    [HideInInspector] public CameraController mainCameraController;

    [SerializeField] public Pathfinder pathfinder;
    [SerializeField] public EnemyBrain enemyBrain;

    [Header("Characters on level:")]
    [SerializeField] private GameObject heroesParent;
    [SerializeField] private GameObject enemyParent;
    [HideInInspector] public List<Character> characterList;
    [HideInInspector] public List<Enemy_Base> enemyList;

    [Header("WorldTurn Type:")]
    [SerializeField] private WorldTurnBase worldTurn;

    [Header("Tiles:")]
    [SerializeField] private GameObject gridParent;
    [HideInInspector] public List<LavaTile> lavaTiles = new List<LavaTile>();
    [HideInInspector] public  List<GrassTile> grassTiles = new List<GrassTile>();
    [HideInInspector] public List<WaterTile> waterTiles = new List<WaterTile>();

    [HideInInspector] public List<TileObject> temporaryTileObjects = new List<TileObject>();

    private PlayerTurn playerTurn;
    public PlayerTurn PlayerTurn
    {
        get { return playerTurn; }
    }

    private EnemyTurn enemyTurn;
    private WeatherTurn weatherTurn;
    private TowersTurn towersTurn;

    private StateInterface currentTurn;
    private TurnEnums.TurnState turnType;

    private int turnNumber;
    private int objectiveTurnNumber = 8;

    //TUTORIAL USES
    [Header("Tutorial Controls: ")]
    public bool isTutorial = false;
    [HideInInspector] public bool disablePlayers = false;
    [HideInInspector] public bool disableEnemies = false;
    [HideInInspector] public bool disableObjects = false;
    [HideInInspector] public bool disableEnd = false;

    public TurnEnums.TurnState TurnType
    {
        get { return turnType; }
    }
    public StateInterface CurrentTurn
    {
        get { return currentTurn; }
    }
    public int TurnNumber
    {
        get { return turnNumber; }
    }

    [HideInInspector] public static UnityEvent LevelVictory = new UnityEvent();
    [HideInInspector] public static UnityEvent LevelDefeat = new UnityEvent();
    [HideInInspector] public static UnityEvent<Character> OnCharacterDied = new UnityEvent<Character>();

    public bool pauseTurns = false;

    #endregion

    #region UnityMethods

    void Start()
    {
        Debug.Assert(mainCam != null, "TurnManager couldn't find the MainCamera Component");

        playerTurn = GetComponent<PlayerTurn>();
        Debug.Assert(playerTurn != null, "TurnManager couldn't find the PlayerTurn Component");

        enemyTurn = GetComponent<EnemyTurn>();
        Debug.Assert(playerTurn != null, "TurnManager couldn't find the EnemyTurn Component");

        weatherTurn = GetComponent<WeatherTurn>();
        Debug.Assert(weatherTurn != null, "TurnManager couldn't find the WeatherTurn Component");

        towersTurn = GetComponent<TowersTurn>();
        Debug.Assert(towersTurn != null, "TurnManager couldn't find the TowersTurn Component");

        mainCameraController = mainCam.GetComponent<CameraController>();
        Debug.Assert(mainCameraController != null, "The Camera given to TurnManager doesn't have a Camera Controller");

        Debug.Assert(gridParent != null, "TurnManager wasn't given a GridParent");
        lavaTiles = new List<LavaTile>(gridParent.GetComponentsInChildren<LavaTile>());
        grassTiles = new List<GrassTile>(gridParent.GetComponentsInChildren<GrassTile>());
        waterTiles = new List<WaterTile>(gridParent.GetComponentsInChildren<WaterTile>());

        characterList.Clear();
        enemyList.Clear();

        foreach (Character character in heroesParent.GetComponentsInChildren<Character>())
        {
            characterList.Add(character);
        }

        foreach(Enemy_Base enemy in enemyParent.GetComponentsInChildren<Enemy_Base>())
        {
            enemyList.Add(enemy);
        }

        objectiveTurnNumber = GameManager.Instance.levelDetails[GameManager.Instance.CurrentLevelIndex].limitTurns;
        turnNumber = 1;
        currentTurn = playerTurn;
        turnType = TurnEnums.TurnState.PlayerTurn;

        EventBus.Instance.Publish(new OnNewLevelStart());

        WorldTurnBase.Victory.AddListener(SceneReset);
        Tile.tileReplaced.AddListener(TileReplaced);
    }

    void Update()
    {
        if(pauseTurns == true)
        {
            return;
        }

        currentTurn.UpdateState();
    }

    #endregion

    #region CustomMethods

    public void SwitchState(TurnEnums.TurnState state)
    {
        currentTurn.ExitState();
        mainCameraController.MoveToDefault(true);

        switch (state)
        {
            case TurnEnums.TurnState.PlayerTurn:
                turnNumber++;
                currentTurn = playerTurn;
                turnType = TurnEnums.TurnState.PlayerTurn;

                CheckTemporaryObjects();
                mainCameraController.controlEnabled = true;

                if (turnNumber == objectiveTurnNumber + 1)
                {
                    LevelDefeat?.Invoke();
                }
                break;

            case TurnEnums.TurnState.EnemyTurn:
                currentTurn = enemyTurn;
                turnType = TurnEnums.TurnState.EnemyTurn;
                mainCameraController.controlEnabled = false;
                break;

            case TurnEnums.TurnState.WorldTurn:
                currentTurn = worldTurn;
                turnType = TurnEnums.TurnState.WorldTurn;
                mainCameraController.controlEnabled = false;
                break;

            case TurnEnums.TurnState.WeatherTurn:
                currentTurn = weatherTurn;
                turnType = TurnEnums.TurnState.WeatherTurn;
                mainCameraController.controlEnabled = false;
                break;
        }

        currentTurn.EnterState();
    }

    public void DestroyACharacter(Character character)
    {
        character.characterTile.characterOnTile = null;
        character.characterTile.tileOccupied = false;

        if (character.characterType == TurnEnums.CharacterType.Player)
        {
            if(!characterList.Contains(character))
            {
                return;
            }

            characterList.Remove(character);

            OnCharacterDied?.Invoke(character);

            if (characterList.Count == 0)
            {
                LevelDefeat?.Invoke();
            }
        }
        else
        {
            if(!enemyList.Contains((Enemy_Base)character))
            {
                return;
            }

            enemyList.Remove((Enemy_Base)character);

            if (enemyList.Count == 0 && towersTurn.HasTowers == false && !isTutorial)
            {
                LevelVictory?.Invoke();
            }
        }

        Destroy(character.gameObject);
    }

    private void SceneReset()
    {
        turnNumber = 1;
        currentTurn = playerTurn;
        WorldTurnBase.Victory.RemoveListener(SceneReset);

        playerTurn.ResetState();
        enemyTurn.ResetState();
        worldTurn.ResetState();
        weatherTurn.ResetState();
    }

    private void TileReplaced(Tile oldTile, Tile newTile)
    {
        if (oldTile.tileData.tileType == ElementType.Fire)
        {
            lavaTiles.Remove((LavaTile)oldTile);
        }
        else if (oldTile.tileData.tileType == ElementType.Water)
        {
            waterTiles.Remove((WaterTile)oldTile);
        }
        else if (oldTile.tileData.tileType == ElementType.Grass)
        {
            grassTiles.Remove((GrassTile)oldTile);
        }

        if (newTile.tileData.tileType == ElementType.Fire)
        {
            lavaTiles.Add((LavaTile)newTile);
        }
        else if (newTile.tileData.tileType == ElementType.Water)
        {
            waterTiles.Add((WaterTile)newTile);
        }
        else if (newTile.tileData.tileType == ElementType.Grass)
        {
            grassTiles.Add((GrassTile)newTile);
        }
    }

    private void CheckTemporaryObjects()
    {
        List<TileObject> tileObjectsToDestroy = new List<TileObject>();
        foreach (TileObject tileObj in temporaryTileObjects)
        {
            if (tileObj.CheckDestruction())
            {
                tileObjectsToDestroy.Add(tileObj);
            }
        }

        foreach (TileObject tileObj in tileObjectsToDestroy)
        {
            temporaryTileObjects.Remove(tileObj);
            Destroy(tileObj.gameObject);
        }
        tileObjectsToDestroy.Clear();
    }

    public void EndLevel()
    {
        LevelVictory?.Invoke();
    }

    #endregion
}