using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class PlayerTurn : StateInterface<TurnManager>
{
    #region Variables

    private Tile currentTile;
    private Character selectedCharacter;

    private TurnEnums.PathfinderTypes type;
    private TurnManager turnManager;

    //private ActiveSkill activeSkill;
    private AttackArea Attackarea;

    #endregion

    #region StateInterfaceMethods

    public void EnterState(TurnManager manager)
    {
        turnManager = manager;
    }

    public void UpdateState()
    {
        ClearTile();
        MouseUpdate();
        KeyboardInputUpdate();
    }

    public void ExitState()
    {
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
            if (type != TurnEnums.PathfinderTypes.BasicAttack)
            {
                ResetBoard();
                SwitchToBasicAttack();
            }
            else
            {
                ResetBoard();
                SwitchToMovement();
            }
        }

        //Switches between Movement and ActiveSkill **TESTING USE ONLY**
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            if(type != TurnEnums.PathfinderTypes.ActiveSkill)
            {
                ResetBoard();
                SwitchToActiveSkill();
            }
            else
            {
                ResetBoard();
                SwitchToMovement();
            }
        }
    }

    //Switches the selected Character to the BasicAttack Action
    public void SwitchToBasicAttack()
    {
        if (selectedCharacter != null)
        {
            type = TurnEnums.PathfinderTypes.BasicAttack;
            turnManager.pathfinder.type = type;


            turnManager.pathfinder.FindPaths(selectedCharacter);
        }
    }

    //Switches the selected Character to the Movement Action
    public void SwitchToMovement()
    {
        if (selectedCharacter != null)
        {
            type = TurnEnums.PathfinderTypes.Movement;
            turnManager.pathfinder.type = type;

            turnManager.pathfinder.FindPaths(selectedCharacter);
        }
    }

    //Switches the selected Character to the ActiveSkill Action
    public void SwitchToActiveSkill()
    {
        if(selectedCharacter != null)
        {
            type = TurnEnums.PathfinderTypes.ActiveSkill;
            Attackarea = AttackArea.SpawnAttackArea(selectedCharacter.newActiveTest).GetComponent<AttackArea>();
        }
    }

    //Resets any modifications to the Board and Resets the Pathfinder
    public void ResetBoard()
    {
        turnManager.pathfinder.ResetPathFinder();
        type = TurnEnums.PathfinderTypes.Movement;
        turnManager.pathfinder.type = type;

        if(Attackarea != null)
        {
            Attackarea.DestroySelf();
        }
    }

    //Ends the player turn and swaps to the enemy turn
    public void EndTurn()
    {
        ResetBoard();
        turnManager.SwitchState(TurnEnums.TurnState.EnemyTurn);
    }

    //Performs the Action for BasicAttack
    private void BasicAttackOnTile()
    {
        if (selectedCharacter == null)
        {
            return;
        }

        if (selectedCharacter.moving == true)
        {
            return;
        }

        if(Input.GetMouseButtonDown(0))
        {
            //Only works if the current tile has an enemy occupant
            if (currentTile.tileOccupied && currentTile.characterOnTile.characterType == TurnEnums.CharacterType.Enemy)
            {
                //**TESTING ONLY**
                turnManager.DestroyACharacter(currentTile.characterOnTile);
                ResetBoard();
                selectedCharacter = null;
            }
        }
    }

    //Performs the Action for ActiveSkill
    private void ActiveSkillOnTile()
    {
        if (selectedCharacter == null)
        {
            return;
        }

        if (selectedCharacter.moving == true)
        {
            return;
        }

        //Used for calculating what Tile to spawn the ActiveSkill mesh on
        Tile selectedTile = currentTile;
        float distance = 1000f;

        //Used for calculating what rotation the spawned ActiveSkill should have
        float rotation = selectedCharacter.transform.rotation.eulerAngles.y;

        //Checks all adjacent tiles to check what one to spawn on
        foreach (Tile tile in turnManager.pathfinder.FindAdjacentTiles(selectedCharacter.characterTile))
        {
            float newDistance = Vector3.Distance(currentTile.transform.position, tile.transform.position);

            //Checks if the current tile is closer to the hit point than the previous ones
            if (newDistance < distance)
            {
                selectedTile = tile;
                distance = newDistance;
            }
        }

        //Sets the ActiveSkill to the selected location
        Vector3 newPos = new Vector3(selectedTile.transform.position.x, 0, selectedTile.transform.position.z);
        Attackarea.transform.position = newPos;

        float angle = Vector3.Angle(selectedCharacter.transform.forward, (selectedTile.transform.position - selectedCharacter.transform.position));

        if(Vector3.Distance(selectedTile.transform.position, selectedCharacter.transform.position + (selectedCharacter.transform.right * 6)) < 
            Vector3.Distance(selectedTile.transform.position, selectedCharacter.transform.position + (-selectedCharacter.transform.right) * 6))
        {
            rotation += angle;
            Attackarea.transform.eulerAngles = new Vector3(0, rotation, 0);
        }
        else
        {
            rotation -= angle;
            Attackarea.transform.eulerAngles = new Vector3(0, rotation, 0);
        }

        //Rotates the ActiveSkill to the selected rotation
        //Attackarea.transform.eulerAngles = new Vector3(0, rotation, 0);

        Attackarea.DetectArea();

        if (Input.GetMouseButton(0))
        {
            //Only works if the current tile has an enemy occupant
            if (currentTile.tileOccupied && currentTile.characterOnTile.characterType == TurnEnums.CharacterType.Enemy)
            {
                //**TESTING ONLY**
                turnManager.DestroyACharacter(currentTile.characterOnTile);
                ResetBoard();
                selectedCharacter = null;
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

        if (currentTile.inFrontier)
        {
            if(type == TurnEnums.PathfinderTypes.Movement)
            {
                currentTile.ChangeTileColor(TileEnums.TileMaterial.frontier);
            }
            else
            {
                currentTile.ChangeTileColor(TileEnums.TileMaterial.attackable);
            }

            currentTile = null;
            return;
        }

        currentTile.ChangeTileColor(TileEnums.TileMaterial.baseMaterial);
        currentTile = null;
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
        if(type == TurnEnums.PathfinderTypes.Movement)
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
        else if(type == TurnEnums.PathfinderTypes.BasicAttack)
        {
            if (currentTile.tileOccupied)
            {
                InspectCharacter();
                BasicAttackOnTile();
            }
        }
        else
        {
            if(currentTile.tileOccupied)
            {
                InspectCharacter();
            }

            ActiveSkillOnTile();
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

        //Highlights Enemy Characters that are hovered over if we are in an Attack Action and the Enemy is in range
        if(type != TurnEnums.PathfinderTypes.Movement)
        {
            if(currentTile.inFrontier && selectedCharType == TurnEnums.CharacterType.Enemy)
            {
                currentTile.ChangeTileColor(TileEnums.TileMaterial.highlight);
            }
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
            ResetBoard();
            selectedCharacter = null;
        }
    }

    #endregion
}
