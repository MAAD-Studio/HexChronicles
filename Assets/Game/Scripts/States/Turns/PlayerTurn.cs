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

    //DEBUG DEBUG DEBUG
    [SerializeField] GameObject hitMarker;

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
        // Apply Character Status in this turn
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
        ClearTile();
        KeyboardInputUpdate();
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

    private void KeyboardInputUpdate()
    {
        //Changes Turn **TESTING USE ONLY**
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            EndTurn();
        }
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

    //Resets any modifications to the Board and Resets the Pathfinder
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

    //Ends the player turn and swaps to the enemy turn
    public void EndTurn()
    {
        ResetBoard();
        turnManager.mainCameraController.UnSelectCharacter();
        turnManager.SwitchState(TurnEnums.TurnState.EnemyTurn);
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
                    if (actionType == TurnEnums.PlayerAction.BasicAttack)
                    {
                        Debug.Log("~~** BASIC ATTACK USED **~~");

                        foreach (Character character in areaPrefab.CharactersHit(TurnEnums.CharacterType.Enemy))
                        {
                            Vector3 hitPos = character.transform.position;
                            hitPos.y += 2;
                            Instantiate(hitMarker, hitPos, Quaternion.identity);
                        }

                        selectedCharacter.PerformBasicAttack(areaPrefab.CharactersHit(TurnEnums.CharacterType.Enemy));
                    }
                    else
                    {
                        Debug.Log("~~** ACTIVE SKILL USED **~~");
                        if(!areaPrefab.freeRange)
                        {
                            foreach (Character character in areaPrefab.CharactersHit(TurnEnums.CharacterType.Enemy))
                            {
                                Vector3 hitPos = character.transform.position;
                                hitPos.y += 2;
                                Instantiate(hitMarker, hitPos, Quaternion.identity);
                            }

                            selectedCharacter.ReleaseActiveSkill(areaPrefab.CharactersHit(TurnEnums.CharacterType.Enemy));
                        }
                        else
                        {
                            foreach (Character character in areaPrefab.CharactersHit(TurnEnums.CharacterType.Enemy))
                            {
                                Vector3 hitPos = character.transform.position;
                                hitPos.y += 2;
                                Instantiate(hitMarker, hitPos, Quaternion.identity);
                            }

                            //NEW METHOD IN ATTACKAREA PROVIDING A LIST OF EFFECTED TILES
                            Debug.Log("OTHER METHOD");
                        }
                    }

                    ResetBoard();
                    selectedCharacter.canAttack = false;
                }
            }
        }
    }

    private void LimitedUpdate()
    {
        if(selectedCharacter != null)
        {
            if(actionType == TurnEnums.PlayerAction.Movement)
            {
                if (!selectedCharacter.moving && selectedCharacter.needsToPath)
                {
                    turnManager.pathfinder.FindPaths(selectedCharacter);
                    selectedCharacter.needsToPath = false;
                }
            }
            else if(areaPrefab != null)
            {
                areaPrefab.DetectArea(true, true);
            }
        }
    }

    #endregion

    #region BreadthFirstMethods

    //Changes the previously selected Tile back to its previous material
    private void ClearTile()
    {
        if (currentTile == null)
        {
            return;
        }

        if (actionType == TurnEnums.PlayerAction.Movement)
        {
            if(currentTile.inFrontier)
            {
                currentTile.ChangeTileColor(TileEnums.TileMaterial.frontier);

                currentTile = null;
            }
            else
            {
                currentTile.ChangeTileColor(TileEnums.TileMaterial.baseMaterial);

                currentTile = null;
            }
        }
        else if(areaPrefab == null || !areaPrefab.ContainsTile(currentTile))
        {
            currentTile.ChangeTileColor(TileEnums.TileMaterial.baseMaterial);
            currentTile = null;
        }
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
            LimitedUpdate();
        }
    }

    private void InspectTile()
    {
        if(selectedCharacter != null && actionType == TurnEnums.PlayerAction.Movement)
        {
            NavigateToTile();
        }
        else if(areaPrefab != null)
        {
            AttackAreaAction();
        }

        if(currentTile.tileOccupied)
        {
            InspectCharacter();
        }
    }

    private void InspectCharacter()
    {
        Character hovererdCharacter = currentTile.characterOnTile;
        TurnEnums.CharacterType selectedCharType = hovererdCharacter.characterType;

        //Highlights Player Characters that are hovered over and can still move this turn
        if (selectedCharType == TurnEnums.CharacterType.Player && hovererdCharacter.movementThisTurn < hovererdCharacter.moveDistance)
        {
            currentTile.ChangeTileColor(TileEnums.TileMaterial.highlight);
        }
 
        //Checks the Character we are trying to grab isn't an Enemy and isn't a Character in motion
        if (!hovererdCharacter.moving && selectedCharType != TurnEnums.CharacterType.Enemy)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //If no Character is selected
                if (selectedCharacter == null)
                {
                    GrabCharacter();
                }
                else
                {
                    ResetBoard();

                    //If the Character we selected is different from the current is switches the selection over to the new one
                    if (selectedCharacter != hovererdCharacter)
                    {
                        GrabCharacter();
                    }
                    //If they are the same we deselect the Character
                    else
                    {
                        turnManager.mainCameraController.UnSelectCharacter();
                        selectedCharacter = null;
                    }
                }
            }
        }
    }

    //Grabs the information for the selected Character and determines where they can travel or attack
    private void GrabCharacter()
    {
        selectedCharacter = currentTile.characterOnTile;
        if (selectedCharacter.canMove)
        {
            turnManager.mainCameraController.SetCamToSelectedCharacter(selectedCharacter);
        }
    }

    //Performs the action for Movement
    private void NavigateToTile()
    {
        if (currentTile.inFrontier)
        {
            currentTile.ChangeTileColor(TileEnums.TileMaterial.highlight);
        }

        if(selectedCharacter.moving)
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
