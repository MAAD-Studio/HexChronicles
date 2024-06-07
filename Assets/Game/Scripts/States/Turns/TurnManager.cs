using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerTurn), typeof(EnemyTurn))]
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

    private PlayerTurn playerTurn;
    private EnemyTurn enemyTurn;

    private StateInterface currentTurn;

    private int turnNumber;
    private int objectiveTurnNumber = 8;

    public StateInterface CurrentTurn
    {
        get { return currentTurn; }
    }
    public int TurnNumber
    {
        get { return turnNumber; }
    }

    [HideInInspector] public static UnityEvent<TurnManager> OnLevelDefeat = new UnityEvent<TurnManager>();
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

        mainCameraController = mainCam.GetComponent<CameraController>();
        Debug.Assert(mainCameraController != null, "The Camera given to TurnManager doesn't have a Camera Controller");

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

                if (turnNumber == objectiveTurnNumber)
                {
                    OnLevelDefeat?.Invoke(this);
                }
                break;

            case TurnEnums.TurnState.EnemyTurn:
                currentTurn = enemyTurn;
                mainCameraController.controlEnabled = false;
                break;

            case TurnEnums.TurnState.WorldTurn:
                currentTurn = worldTurn;
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
            characterList.Remove(character);

            OnCharacterDied?.Invoke(character.name);

            if (characterList.Count == 0)
            {
                OnLevelDefeat?.Invoke(this);
            }
        }
        else
        {
            enemyList.Remove((Enemy_Base)character);
        }

        Destroy(character.gameObject);
    }

    #endregion
}