using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


[RequireComponent(typeof(TurnManager))]
public class PlayerTurnAlternate : MonoBehaviour, StateInterface
{
    #region Variables

    private TurnManager turnManager;
    private Pathfinder pathFinder;
    private CameraController cameraController;
    private Camera mainCam;

    private Tile currentTile;
    public Tile CurrentTile
    {
        get { return currentTile; }
    }

    private Character selectedCharacter;
    public Character SelectedCharacter
    {
        get { return selectedCharacter; }
    }

    private AttackArea areaPrefab;

    [SerializeField] private GameObject selectedCharMarker;
    private GameObject spawnedSelectMarker;

    private Tile potentialMovementTile;
    private Tile[] potentialPath;

    private TurnEnums.PlayerPhase phase = TurnEnums.PlayerPhase.Movement;

    private TurnEnums.PlayerAction attackType = TurnEnums.PlayerAction.BasicAttack;

    #endregion

    #region UnityMethods

    public void Start()
    {
        turnManager = GetComponent<TurnManager>();
        Debug.Assert(turnManager != null, "PlayerTurn doesn't have a TurnManager assigned");

        pathFinder = turnManager.pathfinder;
        Debug.Assert(pathFinder != null, "Playerturn couldn't get a PathFinder from TurnManager");

        cameraController = turnManager.mainCameraController;
        Debug.Assert(cameraController != null, "Playerturn couldn't get a CameraController from TurnManager");

        mainCam = turnManager.mainCam;
        Debug.Assert(mainCam != null, "Playerturn couldn't get a Camera from TurnManager");

        Character.movementComplete.AddListener(CharacterFinishedMoving);
    }

    #endregion

    #region

    public void EnterState()
    {
        foreach (Character character in turnManager.characterList)
        {
            character.EnterNewTurn();
            if (character.characterTile != null)
            {
                character.characterTile.OnTileStay(character);
            }
        }
    }

    public void UpdateState()
    {
        ResetTile();
        KeyboardUpdate();
        MouseUpdate();
    }

    public void ExitState()
    {
        foreach (Character character in turnManager.characterList)
        {
            character.EndTurn();
        }
    }

    #endregion

    #region CustomMethods

    private void EndTurn()
    {
        FullReset();
        turnManager.SwitchState(TurnEnums.TurnState.EnemyTurn);
    }

    private void FullReset()
    {
        ResetBoard();
        phase = TurnEnums.PlayerPhase.Movement;
        cameraController.UnSelectCharacter();
        selectedCharacter = null;
        currentTile = null;
    }

    private void ResetBoard()
    {
        pathFinder.ResetPathFinder();
        DestroySelectMarker();

        potentialMovementTile = null;
        potentialPath = null;

        if (selectedCharacter != null)
        {
            selectedCharacter.characterTile.ChangeTileColor(TileEnums.TileMaterial.baseMaterial);
        }

        if(areaPrefab != null)
        {
            areaPrefab.DestroySelf();
        }
    }

    private void ResetTile()
    {
        if(selectedCharacter != null)
        {
            selectedCharacter.characterTile.ChangeTileColor(TileEnums.TileMaterial.selectedChar);
        }

        if(currentTile == null)
        {
            return;
        }
        
        if(areaPrefab != null && areaPrefab.ContainsTile(currentTile))
        {
            return;
        }
        else if(currentTile.inFrontier)
        {
            currentTile.ChangeTileColor(TileEnums.TileMaterial.frontier);
        }
        else
        {
            currentTile.ChangeTileColor(TileEnums.TileMaterial.baseMaterial);
        }
        currentTile = null;
    }

    private void KeyboardUpdate()
    {
        if(Input.GetKey(KeyCode.Alpha0))
        {
            EndTurn();
        }

        if(Input.GetKeyDown(KeyCode.Backspace) && selectedCharacter != null)
        {
            if(phase == TurnEnums.PlayerPhase.Movement)
            {
                FullReset();
            }
            else if(phase == TurnEnums.PlayerPhase.Attack)
            {
                areaPrefab.DestroySelf();
                phase = TurnEnums.PlayerPhase.Movement;
            }
        }
    }

    private void MouseUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 200f, turnManager.tileLayer))
        {
            currentTile = hit.transform.GetComponent<Tile>();
            SelectPhase();
        }
    }

    private void SelectPhase()
    {
        if(phase == TurnEnums.PlayerPhase.Movement && selectedCharacter != null)
        {
            MovementPhase();
        }
        else if(phase == TurnEnums.PlayerPhase.Attack)
        {
            AttackPhase();
        }
        else
        {
            return;
        }

        if(currentTile.tileOccupied)
        {
            InspectCharacter();
        }
    }

    private void InspectCharacter()
    {
        Character inspectionCharacter = currentTile.characterOnTile;
        TurnEnums.CharacterType characterType = inspectionCharacter.characterType;

        if(characterType == TurnEnums.CharacterType.Player && !inspectionCharacter.hasMadeDecision)
        {
            currentTile.ChangeTileColor(TileEnums.TileMaterial.highlight);

            if(Input.GetMouseButtonDown(0))
            {
                if(selectedCharacter == null)
                {
                    GrabCharacter();
                }
                else if(inspectionCharacter != selectedCharacter)
                {
                    ResetBoard();
                    GrabCharacter();
                }
            }
        }
    }

    private void GrabCharacter()
    {
        selectedCharacter = currentTile.characterOnTile;

        pathFinder.FindPaths(selectedCharacter);

        cameraController.SetCamToSelectedCharacter(selectedCharacter);
        SpawnSelectMarker();
    }

    private void MovementPhase()
    {
        if(!currentTile.inFrontier || !currentTile.Reachable)
        {
            pathFinder.illustrator.ClearIllustrations();
        }
        else if(currentTile.inFrontier || currentTile.characterOnTile == selectedCharacter)
        {
            currentTile.ChangeTileColor(TileEnums.TileMaterial.highlight);

            Tile[] path = new Tile[0];
            if (currentTile.characterOnTile != selectedCharacter)
            {
                path = pathFinder.PathBetween(currentTile, selectedCharacter.characterTile);
            }
            else
            {
                pathFinder.illustrator.ClearIllustrations();
            }

            if (Input.GetMouseButtonDown(0))
            {
                potentialPath = path;
                potentialMovementTile = currentTile;

                phase = TurnEnums.PlayerPhase.Attack;
                SpawnAreaPrefab();
            }
        }
    }

    private void AttackPhase()
    {
        if (!areaPrefab.freeRange)
        {
            areaPrefab.PositionAndRotateAroundCharacter(pathFinder, potentialMovementTile, currentTile);
        }
        else
        {
            areaPrefab.transform.position = currentTile.transform.position;
        }

        areaPrefab.DetectArea(true, true);

        if (Input.GetMouseButton(0))
        {
            if (!currentTile.tileOccupied || currentTile.characterOnTile.characterType != TurnEnums.CharacterType.Player)
            {
                if (!areaPrefab.freeRange || currentTile.tileData.tileType == areaPrefab.effectedTileType)
                {
                    ResetBoard();
                    selectedCharacter.hasMadeDecision = true;
                    phase = TurnEnums.PlayerPhase.Execution;

                    selectedCharacter.ExecuteCharacterAction(potentialPath, turnManager, currentTile);
                }
            }
        }
    }

    private void SpawnAreaPrefab()
    {
        if(phase == TurnEnums.PlayerPhase.Attack)
        {
            if (areaPrefab != null)
            {
                Destroy(areaPrefab);
            }

            if (attackType == TurnEnums.PlayerAction.BasicAttack)
            {
                areaPrefab = Instantiate(selectedCharacter.basicAttackArea);
            }
            else
            {
                areaPrefab = Instantiate(selectedCharacter.activeSkillArea);
            }
        }
    }

    public void SwitchToBasicAttack()
    {
        attackType = TurnEnums.PlayerAction.BasicAttack;
        SpawnAreaPrefab();
    }

    public void SwitchToSpecialAttack()
    {
        attackType = TurnEnums.PlayerAction.ActiveSkill;
        SpawnAreaPrefab();
    }

    public void SpawnSelectMarker()
    {
        if (spawnedSelectMarker == null)
        {
            spawnedSelectMarker = TemporaryMarker.GenerateMarker(selectedCharMarker, selectedCharacter.transform.position, 2.5f);
        }
    }

    public void DestroySelectMarker()
    {
        if (spawnedSelectMarker != null)
        {
            Destroy(spawnedSelectMarker.gameObject);
        }
    }

    private void CharacterFinishedMoving(Character character)
    {
        if(character == selectedCharacter)
        {
            FullReset();
        }
    }

    #endregion
}