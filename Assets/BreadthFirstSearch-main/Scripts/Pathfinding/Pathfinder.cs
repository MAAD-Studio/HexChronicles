using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PathIllustrator))]
public class Pathfinder : MonoBehaviour
{
    #region Variables

    [SerializeField] public LayerMask tileLayer;
    [HideInInspector] public PathIllustrator illustrator;
    [HideInInspector] public List<Tile> frontier = new List<Tile>();

    #endregion

    #region UnityMethods

    void Start()
    {
        illustrator = GetComponent<PathIllustrator>();
        Debug.Assert(illustrator != null, "PathFinder can't find the PathIllustrator component");
    }

    #endregion

    #region BreadthFirstMethods

    public void PathTilesInRange(Tile origin, int originCost, int maxRange, bool includeOccupied)
    {
        FindPaths(origin, originCost, maxRange, includeOccupied);
    }

    public void FindMovementPathsCharacter(Character character)
    {
        FindPaths(character.characterTile, character.movementThisTurn, character.moveDistance, false);
    }

    //BreadthFirst searches for what tiles the character can reach
    private void FindPaths(Tile origin, int originCost, int maxRange, bool includeOccupied)
    {
        ResetPathFinder();

        //Grabs and sets the origin tile
        Queue<Tile> openTiles = new Queue<Tile>();
        openTiles.Enqueue(origin);

        origin.cost = originCost;

        //While we have tiles to investigate
        while (openTiles.Count > 0)
        {
            Tile currentTile = openTiles.Dequeue();

            //Checks every adjacent tile to the current tile we are investigating
            foreach (Tile adjacentTile in FindAdjacentTiles(currentTile, includeOccupied))
            {
                float newCost;

                newCost = currentTile.cost + adjacentTile.tileData.tileCost;

                //If the adjacent tile has already been added to the list of tile to check ignore it
                if (openTiles.Contains(adjacentTile) || frontier.Contains(adjacentTile))
                {
                    continue;
                }

                adjacentTile.cost = newCost;

                //Checks if the character can travel to the adjacent tile, if they can it adds its data into the list to investigate
                if (IsValidTile(adjacentTile, maxRange, includeOccupied))
                {
                    adjacentTile.parentTile = currentTile;
                    openTiles.Enqueue(adjacentTile);
                    AddTileToFrontier(adjacentTile);
                }
            }
        }

        //Once we confirm what tiles can be reached we illustrate them
        illustrator.IllustrateFrontier(frontier);
    }

    //Checks if a tile is valid for reaching
    bool IsValidTile(Tile tile, int maxCost, bool includeOccupied)
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
    public List<Tile> FindAdjacentTiles(Tile origin, bool includeOccupied)
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

                if (includeOccupied || !hitTile.tileOccupied && !hitTile.tileHasObject)
                {
                    adjacentTiles.Add(hitTile);
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

        illustrator.IllustratePath(path);

        return path;
    }

    //Makes the path between two points
    public Tile[] MakePath(Tile destination, Tile origin)
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

    // Used for pushing characters
    public Tile GetTileInDirection(Tile origin, Vector3 direction)
    {
        Vector3 aboveTilePos = origin.transform.position + direction;
        aboveTilePos.y += 1f;

        if (Physics.Raycast(aboveTilePos, Vector3.down, out RaycastHit hit, 50f, tileLayer))
        {
            return hit.transform.GetComponent<Tile>();
        }

        return null;
    }
    #endregion
}