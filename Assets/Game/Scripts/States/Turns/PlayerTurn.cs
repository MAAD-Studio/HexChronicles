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
    private Tile previousTile;

    public Tile CurrentTile
    {
        get { return currentTile; }
    }

    private Character selectedCharacter;
    public Character SelectedCharacter
    {
        get { return selectedCharacter; }
    }

    private Character selectedEnemy;
    public Character SelectedEnemy
    {
        get { return selectedEnemy; }
    }

    private TileObject selectedTileObject;

    private AttackArea areaPrefab;

    [SerializeField] private GameObject selectedCharMarker;
    private GameObject spawnedSelectMarker;

    private GameObject phantom;

    private Tile potentialMovementTile;
    private Tile[] potentialPath;

    private TurnEnums.PlayerPhase phase = TurnEnums.PlayerPhase.Movement;
    public TurnEnums.PlayerPhase Phase
    {
        get { return phase; }
    }

    private TurnEnums.PlayerAction attackType = TurnEnums.PlayerAction.BasicAttack;
    public TurnEnums.PlayerAction AttackType
    {
        get { return attackType;  }
    }

    bool allowSelection = true;
    public bool AllowSelection { get { return allowSelection; } }

    //OPTIMIZATION BOOLS
    private bool moveVFXSpawned = false;

    //TUTORIAL
    public Tile desiredTile = null;
    public Tile desiredAttackTile = null;
    public Character desiredEnemy = null;
    public Character desiredCharacter = null;
    public bool preventPhaseBackUp = false;
    public bool preventAttack = false;

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

        if(!turnManager.disablePlayers)
        {
            SelectCharacter(characterSelected.character);
        }
    }
    #endregion

    #region InterfaceMethods

    public void EnterState()
    {
        foreach (Character character in turnManager.characterList)
        {
            if (character.characterTile != null)
            {
                character.movementThisTurn = 0;
                character.characterTile.OnTileStay(character);
            }
            character.EnterNewTurn();
        }

        cameraController.controlEnabled = true;
    }

    public void UpdateState()
    {
        ResetTile();
        MouseUpdate();
        KeyboardUpdate();
    }

    public void ExitState()
    {
        UndoManager.Instance.ClearData();

        foreach (Character character in turnManager.characterList)
        {
            character.EndTurn();
            character.hasMadeDecision = false;
        }

        cameraController.controlEnabled = false;
    }

    public void ResetState()
    {
        FullReset();
        allowSelection = true;
        attackType = TurnEnums.PlayerAction.BasicAttack;
    }

    #endregion

    #region CustomMethods

    public void UndoAction()
    {
        if(allowSelection)
        {
            if (UndoManager.Instance.DataStored)
            {
                UndoManager.Instance.RestoreState();
            }
            else
            {
                Debug.Log("No Data Stored In UndoManager");
            }
        }
    }

    public void ResetTutorialSelects()
    {
        desiredCharacter = null;
        desiredEnemy = null;
        desiredTile = null;
        desiredAttackTile = null;
    }

    public void EndTurn()
    {
        if(turnManager.isTutorial && turnManager.disableEnd)
        {
            return;
        }

        if(allowSelection)
        {
            FullReset();
            turnManager.SwitchState(TurnEnums.TurnState.EnemyTurn);
        }
    }

    private void FullReset()
    {
        if (currentTile != null)
        {
            currentTile.ChangeTileTop(TileEnums.TileTops.highlight, false);
        }

        if(selectedEnemy != null)
        {
            selectedEnemy = null;
            AttackPreviewer.Instance.ClearAttackArea();
        }

        if(selectedTileObject != null)
        {
            selectedTileObject = null;
            AttackPreviewer.Instance.ClearAttackAreaTower();
        }

        moveVFXSpawned = false;

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
            Tile.UnHighlightTilesOfType(selectedCharacter.elementType);
            Tile.DestroyTileVFX(selectedCharacter.elementType);
        }

        if (areaPrefab != null)
        {
            areaPrefab.DestroySelf();
        }
    }

    private void ResetTile()
    {
        previousTile = currentTile;

        if (currentTile == null)
        {
            return;
        }

        currentTile.ChangeTileTop(TileEnums.TileTops.highlight, false);
        currentTile = null;
    }

    private void KeyboardUpdate()
    {
        if (Input.GetKey(KeyCode.Alpha0))
        {
            EndTurn();
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            MoveBackAPhase();
        }
        else if(Input.GetMouseButtonDown(1))
        {
            MoveBackAPhase();
        }

        if(Input.GetKeyDown(KeyCode.Keypad8))
        {
            UndoAction();
        }

        if(Input.GetKeyDown(KeyCode.Keypad0))
        {
            SwitchToSpecialAttack();
        }

        if(Input.GetKeyDown(KeyCode.Keypad9))
        {
            GameManager.Instance.IncreaseGameSpeed();
        }

        if(Input.GetKeyDown(KeyCode.Keypad6))
        {
            GameManager.Instance.DecreaseGameSpeed();
        }
    }

    public void MoveBackAPhase()
    {
        if(turnManager.isTutorial && preventPhaseBackUp == true)
        {
            return;
        }

        if (selectedTileObject != null)
        {
            if(selectedTileObject.objectType == ObjectType.Tower)
            {
                selectedTileObject = null;
                AttackPreviewer.Instance.ClearAttackAreaTower();
                return;
            }
            selectedTileObject = null;
        }

        if (selectedEnemy != null)
        {
            selectedEnemy = null;
            AttackPreviewer.Instance.ClearAttackArea();
            return;
        }

        if (selectedCharacter != null)
        {
            if (phase == TurnEnums.PlayerPhase.Movement)
            {
                cameraController.MoveToTargetPosition(selectedCharacter.transform.position, false);
                Tile.DestroyTileVFX(selectedCharacter.elementType);
                if (selectedCharacter.characterTile.tileData.tileType == ElementType.Base)
                {
                    selectedCharacter.DestroyTileVFX();
                }
                FullReset();
            }
            else if (phase == TurnEnums.PlayerPhase.Attack)
            {
                Tile.UnHighlightTilesOfType(selectedCharacter.elementType);
                Tile.DestroyTileVFX(selectedCharacter.elementType);
                if (selectedCharacter.characterTile.tileData.tileType == ElementType.Base)
                {
                    selectedCharacter.DestroyTileVFX();
                }
                pathFinder.CreateIllustration();

                moveVFXSpawned = false;

                areaPrefab.DestroySelf();
                potentialMovementTile = null;
                potentialPath = null;
                phase = TurnEnums.PlayerPhase.Movement;
                EventBus.Instance.Publish(new OnMovementPhase());
            }
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
            if(currentTile.tileOccupied && currentTile.characterOnTile.hasMadeDecision)
            {
                //DO NOTHING
            }
            else
            {
                currentTile.ChangeTileTop(TileEnums.TileTops.highlight, true);
            }

            SelectPhase();
        }
        else if(potentialMovementTile == null)
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
        else if(currentTile.tileHasObject)
        {
            if (turnManager.isTutorial && turnManager.disableObjects)
            {
                return;
            }

            InspectTileObject();
        }
    }

    private void InspectCharacter()
    {
        Character inspectionCharacter = currentTile.characterOnTile;
        TurnEnums.CharacterType characterType = inspectionCharacter.characterType;

        if (characterType == TurnEnums.CharacterType.Player)
        {
            if(turnManager.isTutorial && turnManager.disablePlayers)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                SelectCharacter(inspectionCharacter);
            }
        }
        else
        {
            if (turnManager.isTutorial && turnManager.disableEnemies)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                SelectEnemy(inspectionCharacter);
                if (selectedTileObject != null)
                {
                    selectedTileObject = null;
                    AttackPreviewer.Instance.ClearAttackAreaTower();
                }
            }
        }
    }

    private void InspectTileObject()
    {
        if(Input.GetMouseButtonDown(0))
        {
            SelectTileObject();
            if (selectedEnemy != null)
            {
                selectedEnemy = null;
                AttackPreviewer.Instance.ClearAttackArea();
            }
        }
    }

    public void SelectEnemy(Character character)
    {
        if(turnManager.isTutorial && character != null && character != desiredEnemy)
        {
            return;
        }

        if(currentTile == null)
        {
            currentTile = character.characterTile;
        }

        if(allowSelection)
        {
            if(selectedEnemy == null)
            {
                GrabEnemy((Enemy_Base)character);
            }
            else if(selectedEnemy != character)
            {
                AttackPreviewer.Instance.ClearAttackArea();
                GrabEnemy((Enemy_Base)character);
            }
            else
            {
                selectedEnemy = null;
                AttackPreviewer.Instance.ClearAttackArea();
            }
        }
    }

    public void GrabEnemy(Enemy_Base enemy)
    {
        selectedEnemy = currentTile.characterOnTile;
        AttackPreviewer.Instance.PreviewMoveAttackArea(enemy);
    }

    public void SelectTileObject()
    {
        if(allowSelection)
        {
            if(selectedTileObject == null)
            {
                GrabTileObject();
            }
            else if(selectedTileObject != currentTile.objectOnTile)
            {
                AttackPreviewer.Instance.ClearAttackAreaTower();
                GrabTileObject();
            }
            else
            {
                selectedTileObject = null;
                AttackPreviewer.Instance.ClearAttackAreaTower();
            }
        }
    }

    public void GrabTileObject()
    {
        selectedTileObject = currentTile.objectOnTile;

        if(selectedTileObject.objectType == ObjectType.Tower)
        {
            AttackPreviewer.Instance.PreviewAttackAreaTower((Tower)selectedTileObject);
        }
    }

    public void SelectCharacter(Character character)
    {
        if (turnManager.isTutorial && character != null && character != desiredCharacter)
        {
            return;
        }

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
                    EventBus.Instance.Publish(new OnMovementPhase());
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
        if(turnManager.isTutorial && turnManager.disablePlayers)
        {
            return;
        }

        selectedCharacter = currentTile.characterOnTile;
        selectedCharacter.characterTile.ChangeTileColor(TileEnums.TileMaterial.selectedChar);
        cameraController.MoveToTargetPosition(selectedCharacter.transform.position, false);

        pathFinder.FindMovementPathsCharacter(selectedCharacter, true);

        SpawnSelectMarker();
    }

    private void MovementPhase()
    {
        //Clears illustrations if the player moves out of their movement range
        if (!currentTile.inFrontier || !currentTile.Reachable)
        {
            pathFinder.illustrator.ClearIllustrations();
            DestroyPhantom();
        }

        //Spawns in Tile VFX
        if (!moveVFXSpawned)
        {
            Tile.SpawnTileVFX(selectedCharacter.elementType);

            if (currentTile != null && currentTile.tileData.tileType == selectedCharacter.elementType
                && selectedCharacter.elementType == ElementType.Fire)
            {
                Tile.HighlightTilesOfType(selectedCharacter.elementType);
            }
            else
            {
                Tile.UnHighlightTilesOfType(selectedCharacter.elementType);
            }

            moveVFXSpawned = true;
        }

        if (currentTile.inFrontier || currentTile.characterOnTile == selectedCharacter)
        {
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
                if (turnManager.isTutorial && (desiredTile == null || desiredTile != currentTile))
                {
                    return;
                }

                Tile.DestroyTileVFX(selectedCharacter.elementType);

                potentialPath = path;
                potentialMovementTile = currentTile;

                cameraController.MoveToTargetPosition(potentialMovementTile.transform.position, false);

                phase = TurnEnums.PlayerPhase.Attack;
                EventBus.Instance.Publish(new OnAttackPhase());
                SpawnAreaPrefab();

                pathFinder.ClearIllustration();
            }
        }
    }

    private void AttackPhase()
    {
        if (selectedEnemy != null)
        {
            selectedEnemy = null;
            AttackPreviewer.Instance.ClearAttackArea();
        }

        if (selectedTileObject != null)
        {
            selectedTileObject = null;
            AttackPreviewer.Instance.ClearAttackAreaTower();
        }

        if (potentialMovementTile.tileData.tileType == selectedCharacter.elementType)
        {
            selectedCharacter.SpawnTileVFX(potentialMovementTile.transform.position, true);
            
            if (selectedCharacter.elementType == ElementType.Fire)
            {
                Tile.SpawnFireBurn(potentialMovementTile);
            }
        }
        else if (potentialMovementTile.tileData.tileType != ElementType.Base)
        {
            selectedCharacter.SpawnTileVFX(potentialMovementTile.transform.position, false);
        }

        if (!areaPrefab.freeRange)
        {
            areaPrefab.PositionAndRotateAroundCharacter(pathFinder, potentialMovementTile, currentTile);
        }
        else
        {
            areaPrefab.transform.position = currentTile.transform.position;
        }
        areaPrefab.DetectArea(true, true);
        

        if (phantom != null)
        {
            phantom.transform.LookAt(areaPrefab.transform.position);
        }

        // Preview Damage and Status
        foreach (Character character in areaPrefab.CharactersHit(TurnEnums.CharacterType.Enemy))
        {
            character.PreviewDamage(selectedCharacter);
        }
        foreach (TileObject tileObject in areaPrefab.ObjectsHit())
        {
            tileObject.PreviewDamage(selectedCharacter.attackDamage);
        }

        if(selectedCharacter.elementType == potentialMovementTile.tileData.tileType)
        {
            selectedCharacter.PredictTargetsStatus(areaPrefab.CharactersHit(TurnEnums.CharacterType.Enemy), potentialMovementTile);
        }


        if (Input.GetMouseButtonDown(0))
        {
            if (turnManager.isTutorial && desiredEnemy != null && !areaPrefab.CharactersHit(TurnEnums.CharacterType.Enemy).Contains(desiredEnemy))
            {
                return;
            }

            if(turnManager.isTutorial && desiredAttackTile != null && !areaPrefab.TilesHit().Contains(desiredAttackTile))
            {
                return;
            }

            if(turnManager.isTutorial && preventAttack)
            {
                return;
            }    

            if (!currentTile.tileOccupied || currentTile.characterOnTile.characterType != TurnEnums.CharacterType.Player)
            {
                if (!areaPrefab.freeRange || areaPrefab.onlySingleTileType && currentTile.tileData.tileType == areaPrefab.effectedTileType)
                {
                    StoreAttackData();

                    selectedCharacter.hasMadeDecision = true;
                    phase = TurnEnums.PlayerPhase.Execution;

                    EventBus.Instance.Publish(new UpdateCharacterDecision { character = selectedCharacter, hasMadeDecision = true });

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

                    if (attackType == TurnEnums.PlayerAction.BasicAttack)
                    {
                        selectedCharacter.MoveAndAttack(potentialPath, currentTile, turnManager, false, Vector3.zero);
                    }
                    else
                    {
                        selectedCharacter.MoveAndAttack(potentialPath, currentTile, turnManager, true, currentTile.transform.position);
                    }

                    Tile.UnHighlightTilesOfType(selectedCharacter.elementType);

                    ResetBoard();
                    allowSelection = false;
                    attackType = TurnEnums.PlayerAction.BasicAttack;
                }
            }
        }
    }

    private void StoreAttackData()
    {
        UndoManager.Instance.ClearData();
        UndoManager.Instance.StoreHero((Hero)selectedCharacter);

        foreach (Enemy_Base enemy in areaPrefab.CharactersHit(TurnEnums.CharacterType.Enemy))
        {
            UndoManager.Instance.StoreEnemy(enemy, false);
        }

        foreach(TileObject tileObj in areaPrefab.ObjectsHit())
        {
            UndoManager.Instance.StoreTileObject(tileObj, false);
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

            Vector3 position;
            if (phantom != null)
            {
                position = phantom.transform.position;
            }
            else
            {
                position = selectedCharacter.transform.position;
            }

            if (attackType == TurnEnums.PlayerAction.BasicAttack)
            {
                areaPrefab = AttackArea.SpawnAttackArea(selectedCharacter.basicAttackArea, position);
            }
            else
            {
                areaPrefab = AttackArea.SpawnAttackArea(selectedCharacter.activeSkillArea, position);
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

                TileEffect tileEffect = phantom.GetComponent<TileEffect>();
                tileEffect.SetEffect(selectedCharacter.elementType, currentTile.tileData.tileType);
            }

            phantom.transform.position = currentTile.transform.position;

            if(currentTile != null)
            {
                if(selectedCharacter.elementType == currentTile.tileData.tileType)
                {
                    selectedCharacter.SpawnBuffPreview(phantom.transform.position, 0.5f);
                }
                else
                {
                    selectedCharacter.DestroyBuffPreview();
                }
            }
        }
    }

    public void DestroyPhantom()
    {
        if(phantom != null)
        {
            Destroy(phantom.gameObject);
        }

        if(selectedCharacter != null)
        {
            selectedCharacter.DestroyBuffPreview();
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