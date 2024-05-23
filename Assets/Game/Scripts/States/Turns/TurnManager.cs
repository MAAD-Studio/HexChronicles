using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public List<Character> characterList;
    public List<Enemy_Base> enemyList;

    [Header("WorldTurn Type:")]
    [SerializeField] private WorldTurnBase worldTurn;

    private PlayerTurn playerTurn;
    private EnemyTurn enemyTurn;

    private StateInterface currentTurn;

    private int turnNumber;
    public int TurnNumber
    {
        get { return turnNumber; }
    }

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

        switch (state)
        {
            case TurnEnums.TurnState.PlayerTurn:
                turnNumber++;
                mainCameraController.allowControl = true;
                currentTurn = playerTurn;
                break;

            case TurnEnums.TurnState.EnemyTurn:
                currentTurn = enemyTurn;
                mainCameraController.SetCamToDefault();
                mainCameraController.allowControl = false;
                break;

            case TurnEnums.TurnState.WorldTurn:
                currentTurn = worldTurn;
                mainCameraController.SetCamToDefault();
                mainCameraController.allowControl = false;
                break;
        }

        currentTurn.EnterState();
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