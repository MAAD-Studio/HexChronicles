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
            if (character.statusList.Count > 0)
            {
                character.ApplyStatus();
            }

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

        //Switchs between Movement and BasicAttack **TESTING USE ONLY**
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (actionType != TurnEnums.PlayerAction.BasicAttack)
            {
                SwitchToBasicAttack();
            }
            else
            {
                SwitchToMovement();
            }
        }

        //Switches between Movement and ActiveSkill **TESTING USE ONLY**
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            if(actionType != TurnEnums.PlayerAction.ActiveSkill)
            {
                SwitchToActiveSkill();
            }
            else
            {
                SwitchToMovement();
            }
        }
    }

    //Switches the selected Character to the BasicAttack Action
    public void SwitchToBasicAttack()
    {
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
        ResetBoard();
        if (selectedCharacter != null)
        {
            actionType = TurnEnums.PlayerAction.Movement;
            turnManager.pathfinder.FindPaths(selectedCharacter);
        }
    }

    //Switches the selected Character to the ActiveSkill Action
    public void SwitchToActiveSkill()
    {
        ResetBoard();
        if(selectedCharacter != null)
        {
            actionType = TurnEnums.PlayerAction.ActiveSkill;
            areaPrefab = AttackArea.SpawnAttackArea(selectedCharacter.activeSkillArea).GetComponent<AttackArea>();
        }
    }

    //Resets any modifications to the Board and Resets the Pathfinder
    public void ResetBoard()
    {
        turnManager.pathfinder.ResetPathFinder();
        actionType = TurnEnums.PlayerAction.Movement;

        if(areaPrefab != null)
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

    //Performs the Action for ActiveSkill
    private void AttackAreaAction()
    {
        if (selectedCharacter == null || selectedCharacter.moving == true)
        {
            return;
        }

        if(!areaPrefab.freeRange)
        {
            //Used for calculating what Tile to spawn the ActiveSkill mesh on
            Tile selectedTile = DetermineAttackAreaTilePosition();

            //Sets the ActiveSkill to the selected location
            Vector3 newPos = new Vector3(selectedTile.transform.position.x, 0, selectedTile.transform.position.z);
            areaPrefab.transform.position = newPos;

            areaPrefab.transform.eulerAngles = DetermineAttackAreaRotation(selectedTile);

            //Rotates the ActiveSkill to the selected rotation
            //Attackarea.transform.eulerAngles = new Vector3(0, rotation, 0);
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
                    turnManager.mainCameraController.UnSelectCharacter();
                    if (actionType == TurnEnums.PlayerAction.BasicAttack)
                    {
                        Debug.Log("~~** BASIC ATTACK USED **~~");
                        //selectedCharacter.PerformBasicAttack(currentTile.characterOnTile);
                        selectedCharacter.PerformBasicAttack(areaPrefab.CharactersHit(TurnEnums.CharacterType.Enemy));

                        Debug.Log("PLAYERS HIT: " + areaPrefab.CharactersHit(TurnEnums.CharacterType.Player).Count);
                        Debug.Log("ENEMIES HIT: " + areaPrefab.CharactersHit(TurnEnums.CharacterType.Enemy).Count);

                        ResetBoard();
                        selectedCharacter = null;
                    }
                    else
                    {
                        Debug.Log("~~** ACTIVE SKILL USED **~~");
                        selectedCharacter.ReleaseActiveSkill(areaPrefab.CharactersHit(TurnEnums.CharacterType.Enemy));

                        Debug.Log("PLAYERS HIT: " + areaPrefab.CharactersHit(TurnEnums.CharacterType.Player).Count);
                        Debug.Log("ENEMIES HIT: " + areaPrefab.CharactersHit(TurnEnums.CharacterType.Enemy).Count);

                        ResetBoard();
                        selectedCharacter = null;
                    }
                }
            }
        }
    }

    private Tile DetermineAttackAreaTilePosition()
    {
        Tile selectedTile = null;
        float distance = 1000f;

        foreach(Tile tile in turnManager.pathfinder.FindAdjacentTiles(selectedCharacter.characterTile, true))
        {
            float newDistance = Vector3.Distance(currentTile.transform.position, tile.transform.position);

            //Checks if the current tile is closer to the hit point than the previous ones
            if (newDistance < distance)
            {
                selectedTile = tile;
                distance = newDistance;
            }
        }

        return selectedTile;
    }

    private Vector3 DetermineAttackAreaRotation(Tile targetTile)
    {
        Transform characterTransform = selectedCharacter.transform;
        Transform tileTransform = targetTile.transform;

        float rotation = selectedCharacter.transform.eulerAngles.y;

        float angle = Vector3.Angle(characterTransform.forward, (tileTransform.position - characterTransform.position));

        if (Vector3.Distance(tileTransform.position, characterTransform.position + (characterTransform.right * 6)) <
            Vector3.Distance(tileTransform.position, characterTransform.position + (-characterTransform.right) * 6))
        {
            rotation += angle;
        }
        else
        {
            rotation -= angle;
        }

        return new Vector3(0, rotation, 0);
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
                return;
            }
        }
        
        if(areaPrefab == null || !areaPrefab.ContainsTile(currentTile))
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
    }

    private void InspectTile()
    {
        if(actionType == TurnEnums.PlayerAction.Movement)
        {
            if (currentTile.tileOccupied)
            {
                InspectCharacter();
            }
            else
            {
                NavigateToTile();
            }
        }
        else
        {
            AttackAreaAction();

            if (currentTile.tileOccupied)
            {
                InspectCharacter();
            }
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
        turnManager.pathfinder.FindPaths(selectedCharacter);
        turnManager.mainCameraController.SetCamToSelectedCharacter(selectedCharacter);
    }

    //Performs the action for Movement
    private void NavigateToTile()
    {
        if (currentTile.inFrontier)
        {
            currentTile.ChangeTileColor(TileEnums.TileMaterial.highlight);
        }

        if (selectedCharacter == null)
        {
            turnManager.pathfinder.illustrator.ClearIllustrations();
            return;
        }

        if (selectedCharacter.moving == true || currentTile.Reachable == false)
        {
            turnManager.pathfinder.illustrator.ClearIllustrations();
            return;
        }

        Tile[] path = turnManager.pathfinder.PathBetween(currentTile, selectedCharacter.characterTile);

        if (Input.GetMouseButtonDown(0))
        {
            selectedCharacter.Move(path);
            turnManager.mainCameraController.UnSelectCharacter();
            ResetBoard();
            selectedCharacter = null;
        }
    }

    #endregion
}
