using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera), typeof(Pathfinder))]
public class TileInteractor : MonoBehaviour
{
    #region Variables

    [SerializeField] private LayerMask tileLayer;

    private Camera mainCam;
    private Tile currentTile;
    private Character selectedCharacter;
    private Pathfinder pathfinder;

    #endregion

    #region UnityMethods

    void Start()
    {
        mainCam = gameObject.GetComponent<Camera>();
        Debug.Assert(mainCam != null, "TileInteractor couldn't find the MainCamera component");

        pathfinder = gameObject.GetComponent<Pathfinder>();
        Debug.Assert(pathfinder != null, "TileInteractor couldn't find the PathFinder component");
    }

    void Update()
    {
        Clear();
        MouseUpdate();
    }

    #endregion

    #region BreadthFirstMethods

    private void Clear()
    {
        if(currentTile == null)
        {
            return;
        }

        if(currentTile.inFrontier)
        {
            currentTile.ChangeTileColor(TileEnums.TileMaterial.frontier);
            currentTile = null;
            return;
        }

        currentTile.ChangeTileColor(TileEnums.TileMaterial.baseMaterial);
        currentTile = null;
    }

    private void MouseUpdate()
    {
        if (Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 200f, tileLayer))
        {
            currentTile = hit.transform.GetComponent<Tile>();
            InspectTile();
        }
    }

    private void InspectTile()
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

    private void InspectCharacter()
    {
        currentTile.ChangeTileColor(TileEnums.TileMaterial.highlight);
        if (!currentTile.characterOnTile.moving)
        {
            if(Input.GetMouseButtonDown(0))
            {
                //If no character is selected
                if(selectedCharacter == null) 
                {
                    GrabCharacter();
                }
                else
                {
                    pathfinder.ResetPathFinder();

                    //If the character we selected is different from the current is switches the selection over to the new one
                    if(selectedCharacter != currentTile.characterOnTile)
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
        pathfinder.FindPaths(selectedCharacter);
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
            return;
        }

        if(selectedCharacter.moving == true || currentTile.Reachable == false)
        {
            return;
        }

        Tile[] path = pathfinder.PathBetween(currentTile, selectedCharacter.characterTile);

        if(Input.GetMouseButtonDown(0))
        {
            pathfinder.ResetPathFinder();
            selectedCharacter = null;
        }
    }

    #endregion
}
