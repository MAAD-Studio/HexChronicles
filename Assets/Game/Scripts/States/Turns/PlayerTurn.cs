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
    private ActiveSkill activeSkill;
    private List<Tile> adjacentTiles;

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
        InputUpdate();
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

    //Checks for Keyboard input and performs the required functions
    private void InputUpdate()
    {
        //Changes Turn **DEBUG USE ONLY**
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            EndTurn();
        }

        //Switchs between movement and attacl **DEBUG USE ONLY**
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (type != TurnEnums.PathfinderTypes.Attack)
            {
                ResetBoard();
                SwitchToAttack();
            }
            else
            {
                ResetBoard();
                SwitchToMovement();
            }
        }

        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            if(type != TurnEnums.PathfinderTypes.Skill)
            {
                ResetBoard();
                SwitchToSkill();
            }
            else
            {
                ResetBoard();
                SwitchToMovement();
            }
        }
    }

    //Switches the selected character to the Attack Action
    public void SwitchToAttack()
    {
        turnManager.pathfinder.ResetPathFinder();
        if (selectedCharacter != null)
        {
            type = TurnEnums.PathfinderTypes.Attack;
            turnManager.pathfinder.type = type;


            turnManager.pathfinder.FindPaths(selectedCharacter);
        }
    }

    //Switches the selected charatcer to the movement action
    public void SwitchToMovement()
    {
        turnManager.pathfinder.ResetPathFinder();
        if (selectedCharacter != null)
        {
            type = TurnEnums.PathfinderTypes.Movement;
            turnManager.pathfinder.type = type;

            turnManager.pathfinder.FindPaths(selectedCharacter);
        }
    }

    public void SwitchToSkill()
    {
        turnManager.pathfinder.ResetPathFinder();
        if(selectedCharacter != null)
        {
            type = TurnEnums.PathfinderTypes.Skill;
            activeSkill = ActiveSkill.SpawnSelf(selectedCharacter.activeSkill).GetComponent<ActiveSkill>();
        }
    }

    //Changes colors back to normal and clears any pathfinding
    public void ResetBoard()
    {
        turnManager.pathfinder.ResetPathFinder();
        type = TurnEnums.PathfinderTypes.Movement;
        turnManager.pathfinder.type = type;

        if(activeSkill != null)
        {
            activeSkill.DestroySelf();
        }
    }

    //Ends the player turn and swaps to the enemy turn
    public void EndTurn()
    {
        ResetBoard();
        turnManager.SwitchState(TurnEnums.TurnState.EnemyTurn);
    }

    private void AttackTile()
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
            if (currentTile.tileOccupied && currentTile.characterOnTile.characterType == TurnEnums.CharacterType.Enemy)
            {
                //**DEBUG ONLY**
                turnManager.DestroyACharacter(currentTile.characterOnTile);
                ResetBoard();
                selectedCharacter = null;
            }
        }
    }

    private void ActiveSkillUse()
    {
        if (selectedCharacter == null)
        {
            return;
        }

        if (selectedCharacter.moving == true)
        {
            return;
        }

        adjacentTiles = turnManager.pathfinder.FindAdjacentTiles(selectedCharacter.characterTile);

        Tile selectedTile = currentTile;
        float distance = 1000f;
        int loop = 0;
        float rotation = 0f;

        foreach (Tile tile in adjacentTiles)
        {
            loop++;

            float newDistance = Vector3.Distance(currentTile.transform.position, tile.transform.position);

            if (newDistance < distance)
            {
                selectedTile = tile;
                distance = newDistance;
                rotation = selectedCharacter.transform.rotation.y + (60 * loop);
            }
        }

        Vector3 newPos = new Vector3(selectedTile.transform.position.x, 0, selectedTile.transform.position.z);
        activeSkill.transform.position = newPos;

        activeSkill.transform.eulerAngles = new Vector3(0, rotation, 0);

        if (Input.GetMouseButton(0))
        {
            if(currentTile.tileOccupied && currentTile.characterOnTile.characterType == TurnEnums.CharacterType.Enemy)
            {
                //**DEBUG ONLY**
                turnManager.DestroyACharacter(currentTile.characterOnTile);
                ResetBoard();
                selectedCharacter = null;
            }
        }
    }

    #endregion

    #region BreadthFirstMethods

    //Changes the previously selected tile back to its previous material
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
        else if(type == TurnEnums.PathfinderTypes.Attack)
        {
            if (currentTile.tileOccupied)
            {
                InspectCharacter();
                AttackTile();
            }
        }
        else
        {
            if(currentTile.tileOccupied)
            {
                InspectCharacter();
            }
            ActiveSkillUse();
        }
    }

    private void InspectCharacter()
    {
        Character hovererdCharacter = currentTile.characterOnTile;
        TurnEnums.CharacterType selectedCharType = hovererdCharacter.characterType;

        //Highlights the correct characters
        if (selectedCharType == TurnEnums.CharacterType.Player && hovererdCharacter.movementThisTurn < hovererdCharacter.moveDistance)
        {
            currentTile.ChangeTileColor(TileEnums.TileMaterial.highlight);
        }

        if(type != TurnEnums.PathfinderTypes.Movement)
        {
            if(currentTile.inFrontier && selectedCharType == TurnEnums.CharacterType.Enemy)
            {
                currentTile.ChangeTileColor(TileEnums.TileMaterial.highlight);
            }
        }
 
        //Checks the character we are trying to grab isn't an enemy and isn't a character in motion
        if (!hovererdCharacter.moving && selectedCharType != TurnEnums.CharacterType.Enemy)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //If no character is selected
                if (selectedCharacter == null)
                {
                    GrabCharacter();
                }
                else
                {
                    ResetBoard();

                    //If the character we selected is different from the current is switches the selection over to the new one
                    if (selectedCharacter != hovererdCharacter)
                    {
                        GrabCharacter();
                    }
                    //If they are the same we deselect the character
                    else
                    {
                        selectedCharacter = null;
                    }
                }
            }
        }
    }

    //Grabs the information for the selected character and determines where they can travel
    private void GrabCharacter()
    {
        selectedCharacter = currentTile.characterOnTile;
        turnManager.pathfinder.FindPaths(selectedCharacter);
    }

    //Illustrates potential paths and sets the player on their way to a target location when it is clicked
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
