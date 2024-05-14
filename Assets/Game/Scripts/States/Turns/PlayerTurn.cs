using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurn : StateInterface<TurnManager>
{
    #region Variables

    private Tile currentTile;
    private Character selectedCharacter;
    private TurnEnums.PathfinderTypes type;
    private TurnManager turnManager;

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
        if (Input.GetKeyDown(KeyCode.R))
        {
            EndTurn();
        }

        //Switchs between movement and attacl **DEBUG USE ONLY**
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (type == TurnEnums.PathfinderTypes.Movement)
            {
                SwitchToAttack();
            }
            else
            {
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

    //Changes colors back to normal and clears any pathfinding
    public void ResetBoard()
    {
        turnManager.pathfinder.ResetPathFinder();
        type = TurnEnums.PathfinderTypes.Movement;
        turnManager.pathfinder.type = type;
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
        else
        {
            if (currentTile.tileOccupied)
            {
                InspectCharacter();
                AttackTile();
            }
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

        if(type == TurnEnums.PathfinderTypes.Attack)
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
