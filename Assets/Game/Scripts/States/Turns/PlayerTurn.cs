using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurn : StateInterface<TurnManager>
{
    #region Variables

    private Tile currentTile;
    private Character selectedCharacter;

    #endregion

    #region StateInterfaceMethods

    public void EnterState(TurnManager manager)
    {
        
    }

    public void ExitState(TurnManager manager)
    {
        foreach(Character character in manager.characterList)
        {
            character.movementThisTurn = 0;
        }
    }

    public void UpdateState(TurnManager manager)
    {
        Clear();
        MouseUpdate(manager);

        if(Input.GetKeyDown(KeyCode.R))
        {
            manager.SwitchState(TurnEnums.TurnState.EnemyTurn);
        }
    }

    #endregion

    #region BreadthFirstMethods

    private void Clear()
    {
        if (currentTile == null)
        {
            return;
        }

        if (currentTile.inFrontier)
        {
            currentTile.ChangeTileColor(TileEnums.TileMaterial.frontier);
            currentTile = null;
            return;
        }

        currentTile.ChangeTileColor(TileEnums.TileMaterial.baseMaterial);
        currentTile = null;
    }

    private void MouseUpdate(TurnManager manager)
    {
        if (Physics.Raycast(manager.mainCam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 200f, manager.tileLayer))
        {
            currentTile = hit.transform.GetComponent<Tile>();
            InspectTile(manager);
        }
    }

    private void InspectTile(TurnManager manager)
    {
        if (currentTile.tileOccupied)
        {
            InspectCharacter(manager);
        }
        else
        {
            NavigateToTile(manager);
        }
    }

    private void InspectCharacter(TurnManager manager)
    {
        currentTile.ChangeTileColor(TileEnums.TileMaterial.highlight);
        if (!currentTile.characterOnTile.moving)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //If no character is selected
                if (selectedCharacter == null)
                {
                    GrabCharacter(manager);
                }
                else
                {
                    manager.pathfinder.ResetPathFinder();

                    //If the character we selected is different from the current is switches the selection over to the new one
                    if (selectedCharacter != currentTile.characterOnTile)
                    {
                        GrabCharacter(manager);
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
    private void GrabCharacter(TurnManager manager)
    {
        selectedCharacter = currentTile.characterOnTile;
        manager.pathfinder.FindPaths(selectedCharacter);
    }

    //Illustrates potential paths and sets the player on their way to a target location when it is clicked
    private void NavigateToTile(TurnManager manager)
    {
        if (currentTile.inFrontier)
        {
            currentTile.ChangeTileColor(TileEnums.TileMaterial.highlight);
        }

        if (selectedCharacter == null)
        {
            return;
        }

        if (selectedCharacter.moving == true || currentTile.Reachable == false)
        {
            return;
        }

        Tile[] path = manager.pathfinder.PathBetween(currentTile, selectedCharacter.characterTile);

        if (Input.GetMouseButtonDown(0))
        {
            selectedCharacter.Move(path);
            manager.pathfinder.ResetPathFinder();
            selectedCharacter = null;
        }
    }

    #endregion
}
