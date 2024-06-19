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
    public List<LavaTile> lavaTiles = new List<LavaTile>();
    public  List<GrassTile> grassTiles = new List<GrassTile>();
    public List<WaterTile> waterTiles = new List<WaterTile>();

    private PlayerTurn playerTurn;
    private EnemyTurn enemyTurn;
    private WeatherTurn weatherTurn;

    private StateInterface currentTurn;

    private int turnNumber;
    public int objectiveTurnNumber = 8;

    public StateInterface CurrentTurn
    {
        get { return currentTurn; }
    }
    public int TurnNumber
    {
        get { return turnNumber; }
    }

    [HideInInspector] public static UnityEvent LevelDefeat = new UnityEvent();
    [HideInInspector] public static UnityEvent<string> OnCharacterDied = new UnityEvent<string>();

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

        turnNumber = 1;
        currentTurn = playerTurn;

        EventBus.Instance.Publish(new OnNewLevelStart());

        WorldTurnBase.Victory.AddListener(SceneReset);
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
        mainCameraController.MoveToDefault(true);

        switch (state)
        {
            case TurnEnums.TurnState.PlayerTurn:
                turnNumber++;
                mainCameraController.controlEnabled = true;
                currentTurn = playerTurn;
                EventBus.Instance.Publish(new OnPlayerTurn());

                if (turnNumber == objectiveTurnNumber + 1)
                {
                    LevelDefeat?.Invoke();
                }
                break;

            case TurnEnums.TurnState.EnemyTurn:
                currentTurn = enemyTurn;
                EventBus.Instance.Publish(new OnEnemyTurn());
                mainCameraController.controlEnabled = false;
                break;

            case TurnEnums.TurnState.WorldTurn:
                currentTurn = worldTurn;
                mainCameraController.controlEnabled = false;
                break;

            case TurnEnums.TurnState.WeatherTurn:
                currentTurn = weatherTurn;
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

            OnCharacterDied?.Invoke(character.name);

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

    #endregion
}