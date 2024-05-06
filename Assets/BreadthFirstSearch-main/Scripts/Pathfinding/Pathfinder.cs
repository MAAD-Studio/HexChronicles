using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PathIllustrator))]
public class Pathfinder : MonoBehaviour
{
    #region member fields
    PathIllustrator illustrator;
    [SerializeField]
    LayerMask tileMask;

    Frontier currentFrontier = new Frontier();
    #endregion

    private void Start()
    {
        if (illustrator == null)
            illustrator = GetComponent<PathIllustrator>();
    }

    /// <summary>
    /// Main pathfinding function, marks tiles as being in frontier, while keeping a copy of the frontier
    /// in "currentFrontier" for later clearing
    /// </summary>
    /// <param name="character"></param>
    public void FindPaths(Character character)
    {
        ResetPathfinder();

        // When the character is selected, put that character's tile in a queue with a cost of 0
        // openSet contains all tiles that are in the frontier
        Queue<Tile> openSet = new Queue<Tile>();
        openSet.Enqueue(character.characterTile);
        character.characterTile.cost = 0;

        // While there are tiles to explore,
        // take the next tile from the queue and add it to the frontier
        while (openSet.Count > 0) 
        {
            Tile currentTile = openSet.Dequeue();

            // For each neighboring tile, their cost is the current cost + 1
            // Then add them to the openSet queue to be explored next
            foreach (Tile adjacentTile in FindAdjacentTiles(currentTile))
            {
                if (openSet.Contains(adjacentTile))
                    continue;

                adjacentTile.cost = currentTile.cost + 1;

                if (!IsValidTile(adjacentTile, character.movedata.MaxMove))
                    continue;

                adjacentTile.parent = currentTile;

                openSet.Enqueue(adjacentTile);
                AddTileToFrontier(adjacentTile);
            }
        }
        // Now the openSet has been fully explored or reach the maximum length,
        // highlight all frontier tiles in green and store it for future use
        illustrator.IllustrateFrontier(currentFrontier);
    }

    bool IsValidTile(Tile tile, int maxcost)
    {
        bool valid = false;

        if (!currentFrontier.tiles.Contains(tile) && tile.cost <= maxcost)
            valid = true;

        return valid;
    }

    void AddTileToFrontier(Tile tile)
    {
        tile.InFrontier = true;
        currentFrontier.tiles.Add(tile);
    }

    /// <summary>
    /// Returns a list of all neighboring hexagonal tiles and ladders
    /// </summary>
    /// <param name="origin"></param>
    /// <returns></returns>
    private List<Tile> FindAdjacentTiles(Tile origin)
    {
        List<Tile> tiles = new List<Tile>();
        Vector3 direction = Vector3.forward;
        float rayLength = 50f;
        float rayHeightOffset = 1f;

        //Rotate a raycast in 60 degree steps and find all adjacent tiles
        for (int i = 0; i < 6; i++)
        {
            direction = Quaternion.Euler(0f, 60f, 0f) * direction;

            Vector3 aboveTilePos = (origin.transform.position + direction).With(y: origin.transform.position.y + rayHeightOffset);

            if (Physics.Raycast(aboveTilePos, Vector3.down, out RaycastHit hit, rayLength, tileMask))
            {
                Tile hitTile = hit.transform.GetComponent<Tile>();
                if (hitTile.Occupied == false)
                    tiles.Add(hitTile);
            }
        }

        if (origin.connectedTile != null)
            tiles.Add(origin.connectedTile);

        return tiles;
    }

    /// <summary>
    /// Called by Interact.cs to create a path between two tiles on the grid 
    /// </summary>
    /// <param name="dest"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public Path PathBetween(Tile dest, Tile source)
    {
        Path path = MakePath(dest, source);
        illustrator.IllustratePath(path);
        return path;
    }

    /// <summary>
    /// Creates a path between two tiles
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    // <Function>
    // This is the path that are anticipated while hovering cursor over a tile
    private Path MakePath(Tile destination, Tile origin) 
    {
        List<Tile> tiles = new List<Tile>();
        Tile current = destination;

        // Starting from the destination point, trace back to the origin, add each tile to a list
        // As long as it's not reaching the origin, keep tracing the current tile's parent
        while (current != origin)
        {
            tiles.Add(current);
            if (current.parent != null)
                current = current.parent;
            else
                break;
        }

        tiles.Add(origin);
        tiles.Reverse(); // Reverse the list of tiles so that the origin is the first element
        // Generate a Path from this list
        Path path = new Path();
        path.tilesInPath = tiles.ToArray();

        return path;
    }

    public void ResetPathfinder()
    {
        illustrator.Clear();

        foreach (Tile item in currentFrontier.tiles)
        {
            item.InFrontier = false;
            item.ClearColor();
        }

        currentFrontier.tiles.Clear();
    }
}
