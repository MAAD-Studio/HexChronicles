using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TurnManager))]
public class PlayerTurn : MonoBehaviour, StateInterface
{
    #region Variables
    
    private Tile currentTile;
    private Character selectedCharacter;

    private TurnEnums.PlayerAction actionType;
    private TurnManager turnManager;

    private AttackArea areaPrefab;

    public Tile CurrentTile
    {
        get { return currentTile; }
    }
    public Character SelectedCharacter
    {
        get { return selectedCharacter; }
    }

    [SerializeField] private GameObject hitMarker;

    #endregion

    #region UnityMethods

    private void Start()
    {
        turnManager = GetComponent<TurnManager>();
        Debug.Assert(turnManager != null, "PlayerTurn doesn't have a TurnManager");
    }

    #endregion

    #region StateInterfaceMethods

    public void EnterState()
    {
        foreach (Character character in turnManager.characterList)
        {
            character.EnterNewTurn();
            if(character.characterTile != null)
            {
                character.characterTile.OnTileStay(character);
            }
        }
    }

    public void UpdateState()
    {
        ClearPreviousTile();
        KeyboardUpdate();
        MouseUpdate();
    }

    public void ExitState()
    {
        ResetBoard();
        selectedCharacter = null;
        currentTile = null;

        foreach(Character character in turnManager.characterList)
        {
            character.movementThisTurn = 0;
        }
    }

    #endregion

    #region CustomMethods

    private void ClearPreviousTile()
    {
        if (currentTile == null)
        {
            return;
        }

        if (actionType == TurnEnums.PlayerAction.Movement)
        {
            if (currentTile.inFrontier)
            {
                currentTile.ChangeTileColor(TileEnums.TileMaterial.frontier);
            }
            else
            {
                currentTile.ChangeTileColor(TileEnums.TileMaterial.baseMaterial);
            }
        }
        else if (areaPrefab == null || !areaPrefab.ContainsTile(currentTile))
        {
            currentTile.ChangeTileColor(TileEnums.TileMaterial.baseMaterial);
        }

        currentTile = null;
    }

    private void MouseUpdate()
    {
        if (Physics.Raycast(turnManager.mainCam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 200f, turnManager.tileLayer))
        {
            currentTile = hit.transform.GetComponent<Tile>();
            InspectTile();
        }
        else
        {
            NoHitsUpdate();
        }
    }

    private void KeyboardUpdate()
    {

    }

    //Switches the selected Character to the BasicAttack Action
    public void SwitchToBasicAttack()
    {
        if(!selectedCharacter.canAttack || selectedCharacter.moving || actionType == TurnEnums.PlayerAction.BasicAttack)
        {
            return;
        }

        ResetBoard();
        if (selectedCharacter != null)
        {
            actionType = TurnEnums.PlayerAction.BasicAttack;
            areaPrefab = AttackArea.SpawnAttackArea(selectedCharacter.basicAttackArea).GetComponent<AttackArea>();
        }
    }

    //Switches the selected Character to the Movement Action
    public void SwitchToMovement()
    {
        if(!selectedCharacter.canMove || actionType == TurnEnums.PlayerAction.Movement)
        {
            return;
        }

        ResetBoard();
        if (selectedCharacter != null)
        {
            actionType = TurnEnums.PlayerAction.Movement;
            turnManager.pathfinder.FindPaths(selectedCharacter);
            selectedCharacter.needsToPath = false;
        }
    }

    //Switches the selected Character to the ActiveSkill Action
    public void SwitchToActiveSkill()
    {
        if (!selectedCharacter.canAttack || selectedCharacter.moving || actionType == TurnEnums.PlayerAction.ActiveSkill)
        {
            return;
        }

        ResetBoard();
        if(selectedCharacter != null)
        {
            Hero hero = (Hero)selectedCharacter;
            if (hero.currentSkillCD == 0)
            {
                actionType = TurnEnums.PlayerAction.ActiveSkill;
                areaPrefab = AttackArea.SpawnAttackArea(selectedCharacter.activeSkillArea).GetComponent<AttackArea>();
            }
            else
            {
                Debug.Log("Skill is on cooldown");
            }
        }
    }

    public void ResetBoard()
    {
        turnManager.pathfinder.ResetPathFinder();
        actionType = TurnEnums.PlayerAction.None;

        if (selectedCharacter != null && selectedCharacter.hasMoved)
        {
            selectedCharacter.canMove = false;
        }

        if (areaPrefab != null)
        {
            areaPrefab.DestroySelf();
        }
    }

    public void EndTurn()
    {
        ResetBoard();
        turnManager.mainCameraController.UnSelectCharacter();
        turnManager.SwitchState(TurnEnums.TurnState.EnemyTurn);
    }

    private void NoHitsUpdate()
    {
        if (selectedCharacter != null)
        {
            if (actionType == TurnEnums.PlayerAction.Movement)
            {
                if (!selectedCharacter.moving && selectedCharacter.needsToPath)
                {
                    turnManager.pathfinder.FindPaths(selectedCharacter);
                    selectedCharacter.needsToPath = false;
                }
            }
            else if (areaPrefab != null)
            {
                areaPrefab.DetectArea(true, true);
            }
        }
    }

    private void InspectTile()
    {
        if (selectedCharacter != null && actionType == TurnEnums.PlayerAction.Movement)
        {
            NavigateToTile();
        }
        else if (areaPrefab != null)
        {
            AttackAreaAction();
        }

        if (currentTile.tileOccupied)
        {
            InspectCharacter();
        }
    }

    private void InspectCharacter()
    {
        Character hovererdCharacter = currentTile.characterOnTile;
        TurnEnums.CharacterType selectedCharType = hovererdCharacter.characterType;

        if (selectedCharType == TurnEnums.CharacterType.Player && !hovererdCharacter.moving)
        {
            currentTile.ChangeTileColor(TileEnums.TileMaterial.highlight);

            if (Input.GetMouseButtonDown(0))
            {
                //If no Character is selected grab the Character
                if (selectedCharacter == null)
                {
                    GrabCharacter();
                }
                else
                {
                    ResetBoard();

                    //If the Character selected is [DIFFERENT] from the previous one grab the Character
                    if (selectedCharacter != hovererdCharacter)
                    {
                        GrabCharacter();
                    }
                    //If the Character selected is the [SAME] as the previous one deselect the Character
                    else
                    {
                        turnManager.mainCameraController.UnSelectCharacter();
                        selectedCharacter = null;
                    }
                }
            }
        }
    }

    private void GrabCharacter()
    {
        selectedCharacter = currentTile.characterOnTile;
        if (selectedCharacter.canMove)
        {
            turnManager.mainCameraController.SetCamToSelectedCharacter(selectedCharacter);
        }
    }

    private void AttackAreaAction()
    {
        //Determines how to position the AttackArea based on its freeRange setting
        if(!areaPrefab.freeRange)
        {
            areaPrefab.PositionAndRotateAroundCharacter(turnManager.pathfinder, selectedCharacter.characterTile, currentTile);
        }
        else
        {
            areaPrefab.transform.position = currentTile.transform.position;
        }

        areaPrefab.DetectArea(true, true);

        if (Input.GetMouseButton(0))
        {
            //Won't trigger if the occupant of the hovered over tile is a Player Character
            if (!currentTile.tileOccupied || currentTile.characterOnTile.characterType != TurnEnums.CharacterType.Player)
            {
                if (!areaPrefab.freeRange || currentTile.tileData.tileType == areaPrefab.effectedTileType)
                {
                    SpawnHitMarkers();

                    if (actionType == TurnEnums.PlayerAction.BasicAttack)
                    {
                        Debug.Log("~~** BASIC ATTACK USED **~~");
                        selectedCharacter.PerformBasicAttack(areaPrefab.CharactersHit(TurnEnums.CharacterType.Enemy));
                    }
                    else
                    {
                        Debug.Log("~~** ACTIVE SKILL USED **~~");
                        if(!areaPrefab.freeRange)
                        {
                            selectedCharacter.ReleaseActiveSkill(areaPrefab.CharactersHit(TurnEnums.CharacterType.Enemy));
                        }
                        else
                        {
                            Debug.Log("OTHER METHOD");
                        }
                    }

                    ResetBoard();
                    selectedCharacter.canAttack = false;
                }
            }
        }
    }

    public void SpawnHitMarkers()
    {
        foreach (Character character in areaPrefab.CharactersHit(TurnEnums.CharacterType.Enemy))
        {
            Vector3 hitPos = character.transform.position;
            hitPos.y += 2;
            Instantiate(hitMarker, hitPos, Quaternion.identity);
        }
    }

    //Performs the action for Movement
    private void NavigateToTile()
    {
        if (currentTile.inFrontier)
        {
            currentTile.ChangeTileColor(TileEnums.TileMaterial.highlight);
        }

        if (selectedCharacter.moving)
        {
            return;
        }

        if (selectedCharacter.needsToPath)
        {
            turnManager.pathfinder.FindPaths(selectedCharacter);
            selectedCharacter.needsToPath = false;
        }

        if (currentTile.Reachable == false)
        {
            turnManager.pathfinder.illustrator.ClearIllustrations();
            if (selectedCharacter.movementThisTurn >= selectedCharacter.moveDistance)
            {
                ResetBoard();
                selectedCharacter.canMove = false;
            }

            return;
        }

        Tile[] path = turnManager.pathfinder.PathBetween(currentTile, selectedCharacter.characterTile);

        if (Input.GetMouseButtonDown(0))
        {
            selectedCharacter.Move(path);
            turnManager.pathfinder.ResetPathFinder();
            selectedCharacter.hasMoved = true;
            selectedCharacter.needsToPath = true;
        }
    }

    #endregion
}
