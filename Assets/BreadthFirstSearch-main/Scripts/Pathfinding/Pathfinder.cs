using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent(typeof(PathIllustrator))]
public class Pathfinder : MonoBehaviour
{
    #region Variables

    [SerializeField] private LayerMask tileLayer;
    [HideInInspector] public PathIllustrator illustrator;
    private List<Tile> frontier = new List<Tile>();
    [HideInInspector] public TurnEnums.PathfinderTypes type;

    #endregion

    #region UnityMethods

    void Start()
    {
        illustrator = GetComponent<PathIllustrator>();
        Debug.Assert(illustrator != null, "PathFinder can't find the PathIllustrator component");
    }

    #endregion

    #region CustomMethods

    #endregion

    #region BreadthFirstMethods

    //BreadthFirst searches for what tiles the character can reach
    public void FindPaths(Character character)
    {
        ResetPathFinder();

        //Grabs and sets the origin tile
        Queue<Tile> openTiles = new Queue<Tile>();
        openTiles.Enqueue(character.characterTile);

        if(type == TurnEnums.PathfinderTypes.Movement)
        {
            character.characterTile.cost = character.movementThisTurn;
        }
        else
        {
            character.characterTile.cost = 0;
        }

        //While we have tiles to investigate
        while (openTiles.Count > 0)
        {
            Tile currentTile = openTiles.Dequeue();

            //Checks every adjacent tile to the current tile we are investigating
            foreach (Tile adjacentTile in FindAdjacentTiles(currentTile))
            {
                float newCost;
                if (type == TurnEnums.PathfinderTypes.Movement)
                {
                    newCost = currentTile.cost + adjacentTile.tileData.tileCost;
                }
                else
                {
                    newCost = currentTile.cost + 1;
                }

                //If the adjacent tile has already been added to the list of tile to check ignore it
                if (openTiles.Contains(adjacentTile))
                {
                    if(adjacentTile.cost > newCost)
                    {
                        adjacentTile.cost = newCost;
                        adjacentTile.parentTile = currentTile;
                    }
                    continue;
                }

                adjacentTile.cost = newCost;

                if(type == TurnEnums.PathfinderTypes.Movement)
                {
                    //Checks if the character can travel to the adjacent tile, if they can it adds its data into the list to investigate
                    if (IsValidTile(adjacentTile, character.moveDistance))
                    {
                        adjacentTile.parentTile = currentTile;
                        openTiles.Enqueue(adjacentTile);
                        AddTileToFrontier(adjacentTile);
                    }
                }
                else
                {
                    //Checks if the character can travel to the adjacent tile, if they can it adds its data into the list to investigate
                    if (IsValidTile(adjacentTile, character.attackDistance))
                    {
                        adjacentTile.parentTile = currentTile;
                        openTiles.Enqueue(adjacentTile);
                        AddTileToFrontier(adjacentTile);
                    }
                }
            }
        }

        //Once we confirm what tiles can be reached we illustrate them
        illustrator.IllustrateFrontier(frontier, type);
    }

    //Checks if a tile is valid for reaching
    bool IsValidTile(Tile tile, int maxCost)
    {
        if (!frontier.Contains(tile) && tile.cost <= maxCost && tile.tileData.walkable)
        {
            return true;
        }
        return false;
    }

    //Adds a tile into the frontier
    void AddTileToFrontier(Tile tile)
    {
        tile.inFrontier = true;
        frontier.Add(tile);
    }

    //Finds any tiles adjacent to the current tile
    private List<Tile> FindAdjacentTiles(Tile origin)
    {
        List<Tile> adjacentTiles = new List<Tile>();

        Vector3 direction = Vector3.forward;
        float rayLength = 50f;
        float rayHeightOffset = 1f;

        //Checks in all 6 direction for an adjacent tile
        for (int i = 0; i < 6; i++)
        {
            direction = Quaternion.Euler(0f, 60f, 0f) * direction;

            Vector3 aboveTilePos = origin.transform.position + direction;
            aboveTilePos.y += rayHeightOffset;

            //If we hit a tile we add it to the list of adjacents
            if (Physics.Raycast(aboveTilePos, Vector3.down, out RaycastHit hit, rayLength, tileLayer))
            {
                Tile hitTile = hit.transform.GetComponent<Tile>();
                
                if(type == TurnEnums.PathfinderTypes.Movement)
                {
                    if (!hitTile.tileOccupied)
                    {
                        adjacentTiles.Add(hitTile);
                    }
                }
                else
                {
                    if (!hitTile.tileOccupied)
                    {
                        adjacentTiles.Add(hitTile);
                    }
                    else if(hitTile.characterOnTile.characterType == TurnEnums.CharacterType.Enemy)
                    {
                        adjacentTiles.Add(hitTile);
                    }

                }
            }
        }

        //Checks if a tile was specially connected to the current tile, if so it is added to the list of adjacent tiles
        if (origin.connectedTile != null)
        {
            adjacentTiles.Add(origin.connectedTile);
        }

        return adjacentTiles;
    }

    //Creates and illustrates the path between two points
    public Tile[] PathBetween(Tile dest, Tile source)
    {
        Tile[] path = MakePath(dest, source);
        if(type == TurnEnums.PathfinderTypes.Movement)
        {
            illustrator.IllustratePath(path);
        }
        return path;
    }

    //Makes the path between two points
    private Tile[] MakePath(Tile destination, Tile origin)
    {
        List<Tile> tiles = new List<Tile>();
        Tile current = destination;

        while (current != origin)
        {
            tiles.Add(current);
            if (current.parentTile != null)
            {
                current = current.parentTile;
            }
            else
            {
                break;
            }
        }

        tiles.Add(origin);
        tiles.Reverse();

        Tile[] path = tiles.ToArray();

        return path;
    }

    //Resets any pathing information
    public void ResetPathFinder()
    {
        illustrator.ClearIllustrations();

        foreach (Tile tile in frontier)
        {
            tile.inFrontier = false;
            tile.ChangeTileColor(TileEnums.TileMaterial.baseMaterial);
        }

        frontier.Clear();
    }

    #endregion
}
