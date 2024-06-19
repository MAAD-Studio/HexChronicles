using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


[RequireComponent(typeof(TurnManager))]
public class PlayerTurn : MonoBehaviour, StateInterface
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

    private GameObject phantom;

    private Tile potentialMovementTile;
    private Tile[] potentialPath;

    private TurnEnums.PlayerPhase phase = TurnEnums.PlayerPhase.Movement;

    private TurnEnums.PlayerAction attackType = TurnEnums.PlayerAction.BasicAttack;

    bool allowSelection = true;

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

        SubscribeEvents();

        EventBus.Instance.Publish(new OnPlayerTurn());
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
    #endregion

    #region Events
    private void SubscribeEvents()
    {
        Character.movementComplete.AddListener(CharacterFinishedMoving);
        if (EventBus.Instance != null)
        {
            EventBus.Instance.Subscribe<CharacterSelected>(OnSelectCharacter);
        }
    }

    private void UnsubscribeEvents()
    {
        Character.movementComplete.RemoveListener(CharacterFinishedMoving);
        if (EventBus.Instance != null)
        {
            EventBus.Instance.Unsubscribe<CharacterSelected>(OnSelectCharacter);
        }
    }

    public void OnSelectCharacter(object obj)
    {
        CharacterSelected characterSelected = (CharacterSelected)obj;
        SelectCharacter(characterSelected.character);
    }
    #endregion

    #region InterfaceMethods

    public void EnterState()
    {
        foreach (Character character in turnManager.characterList)
        {
            character.EnterNewTurn();
            if (character.characterTile != null)
            {
                character.movementThisTurn = 0;
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
            character.hasMadeDecision = false;
        }
    }

    public void ResetState()
    {
        FullReset();
        allowSelection = true;
        attackType = TurnEnums.PlayerAction.BasicAttack;
    }

    #endregion

    #region CustomMethods

    public void EndTurn()
    {
        if(allowSelection)
        {
            FullReset();
            turnManager.SwitchState(TurnEnums.TurnState.EnemyTurn);
        }
    }

    private void FullReset()
    {
        ResetBoard();
        DestroyPhantom();
        phase = TurnEnums.PlayerPhase.Movement;
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

        if (areaPrefab != null)
        {
            areaPrefab.DestroySelf();
        }
    }

    private void ResetTile()
    {
        if (currentTile == null)
        {
            return;
        }

        currentTile.ChangeTileColor(TileEnums.TileMaterial.baseMaterial);
        currentTile = null;
    }

    private void KeyboardUpdate()
    {
        if (Input.GetKey(KeyCode.Alpha0))
        {
            EndTurn();
        }

        if (Input.GetKeyDown(KeyCode.Backspace) && selectedCharacter != null)
        {
            MoveBackAPhase();
        }
        else if(Input.GetMouseButtonDown(1) && selectedCharacter != null)
        {
            MoveBackAPhase();
        }
    }

    public void MoveBackAPhase()
    {
        if (phase == TurnEnums.PlayerPhase.Movement)
        {
            cameraController.MoveToTargetPosition(selectedCharacter.transform.position, false);
            FullReset();
        }
        else if (phase == TurnEnums.PlayerPhase.Attack)
        {
            areaPrefab.DestroySelf();
            phase = TurnEnums.PlayerPhase.Movement;
        }
    }

    private void MouseUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;

            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raycastResults);

            foreach (var result in raycastResults)
            {
                if (result.gameObject.layer == LayerMask.NameToLayer("UI"))
                {
                    return;
                }
            }
        }
        if (Physics.Raycast(turnManager.mainCam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 200f, turnManager.tileLayer))
        {
            currentTile = hit.transform.GetComponent<Tile>();
            SelectPhase();
        }
        else
        {
            pathFinder.illustrator.ClearIllustrations();
            DestroyPhantom();
        }
    }

    private void SelectPhase()
    {
        if (phase == TurnEnums.PlayerPhase.Movement && selectedCharacter != null)
        {
            MovementPhase();
        }
        else if (phase == TurnEnums.PlayerPhase.Attack)
        {
            AttackPhase();
        }

        if (currentTile.tileOccupied)
        {
            InspectCharacter();
        }
    }

    private void InspectCharacter()
    {
        Character inspectionCharacter = currentTile.characterOnTile;
        TurnEnums.CharacterType characterType = inspectionCharacter.characterType;

        if (characterType == TurnEnums.CharacterType.Player)
        {
            if (!inspectionCharacter.hasMadeDecision && allowSelection)
            {
                currentTile.ChangeTileColor(TileEnums.TileMaterial.highlight); // Hover tile highlight
            }

            if (Input.GetMouseButtonDown(0))
            {
                SelectCharacter(inspectionCharacter);
            }
        }
    }

    public void SelectCharacter(Character character)
    {
        // Get the tile from the passed character
        if (currentTile == null)
        {
            currentTile = character.characterTile;
        }

        if(allowSelection)
        {
            if (!character.hasMadeDecision)
            {
                if (selectedCharacter == null)
                {
                    GrabCharacter();
                }
                else if (character != selectedCharacter)
                {
                    ResetBoard();
                    GrabCharacter();
                    phase = TurnEnums.PlayerPhase.Movement;
                }
            }
            else
            {
                MouseTip.Instance.ShowTip(Input.mousePosition, "This hero can't move or attack anymore in this turn", true);
            }
        }
    }

    private void GrabCharacter()
    {
        selectedCharacter = currentTile.characterOnTile;
        selectedCharacter.characterTile.ChangeTileColor(TileEnums.TileMaterial.selectedChar);
        cameraController.MoveToTargetPosition(selectedCharacter.transform.position, false);

        pathFinder.FindMovementPathsCharacter(selectedCharacter, true);

        SpawnSelectMarker();
    }

    private void MovementPhase()
    {
        if (!currentTile.inFrontier || !currentTile.Reachable)
        {
            pathFinder.illustrator.ClearIllustrations();
            DestroyPhantom();
        }
        
        if (currentTile.inFrontier || currentTile.characterOnTile == selectedCharacter)
        {
            currentTile.ChangeTileColor(TileEnums.TileMaterial.highlight);

            Tile[] path = new Tile[0];
            if (currentTile.characterOnTile != selectedCharacter)
            {
                SpawnPhantom();
                path = pathFinder.PathBetween(currentTile, selectedCharacter.characterTile);
            }
            else
            {
                pathFinder.illustrator.ClearIllustrations();
                DestroyPhantom();
            }

            if (Input.GetMouseButtonDown(0))
            {
                potentialPath = path;
                potentialMovementTile = currentTile;

                cameraController.MoveToTargetPosition(potentialMovementTile.transform.position, false);

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

        if(phantom != null)
        {
            phantom.transform.LookAt(areaPrefab.transform.position);
        }

        areaPrefab.DetectArea(true, true);

        // Preview Damage on enemy healthbar
        foreach (Character character in areaPrefab.CharactersHit(TurnEnums.CharacterType.Enemy))
        {
            character.PreviewDamage(selectedCharacter.attackDamage);
        }

        foreach (TileObject tileObject in areaPrefab.ObjectsHit())
        {
            tileObject.PreviewDamage(selectedCharacter.attackDamage);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!currentTile.tileOccupied || currentTile.characterOnTile.characterType != TurnEnums.CharacterType.Player)
            {
                if (!areaPrefab.freeRange || currentTile.tileData.tileType == areaPrefab.effectedTileType)
                {
                    selectedCharacter.hasMadeDecision = true;
                    phase = TurnEnums.PlayerPhase.Execution;

                    EventBus.Instance.Publish(new CharacterHasMadeDecision { character = selectedCharacter });

                    if(pathFinder == null)
                    {
                        Debug.Log("Path Null");
                    }
                    if(turnManager == null)
                    {
                        Debug.Log("TurnManager Null");
                    }
                    if(currentTile == null)
                    {
                        Debug.Log("CurrentTile Null");
                    }

                    DestroyPhantom();

                    if(attackType == TurnEnums.PlayerAction.BasicAttack)
                    {
                        selectedCharacter.MoveAndAttack(potentialPath, currentTile, turnManager, false);
                    }
                    else
                    {
                        selectedCharacter.MoveAndAttack(potentialPath, currentTile, turnManager, true);
                    }

                    ResetBoard();
                    allowSelection = false;
                    attackType = TurnEnums.PlayerAction.BasicAttack;
                }
            }
        }
    }

    private void SpawnAreaPrefab()
    {
        if (phase == TurnEnums.PlayerPhase.Attack)
        {
            if (areaPrefab != null)
            {
                areaPrefab.DestroySelf();
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

    public void SpawnPhantom()
    {
        if(selectedCharacter != null)
        {
            if (phantom == null)
            {
                Hero selectedHero = (Hero)selectedCharacter;
                phantom = Instantiate(selectedHero.heroSO.phantomModel, currentTile.transform.position, Quaternion.identity);
            }

            phantom.transform.position = currentTile.transform.position;
        }
    }

    public void DestroyPhantom()
    {
        if(phantom != null)
        {
            Destroy(phantom.gameObject);
        }
    }

    private void CharacterFinishedMoving(Character character)
    {
        if (character == selectedCharacter)
        {
            allowSelection = true;
            FullReset();
        }
    }

    #endregion
}