using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.ScrollRect;

public class AttackArea : MonoBehaviour
{
    #region Variables

    private List<TileReporter> tileReporters = new List<TileReporter>();
    private List<Tile> reporterTiles = new List<Tile>();

    [SerializeField] public bool freeRange = false;
    [SerializeField] public ElementType effectedTileType;

    [SerializeField] public float maxHittableRange = 1f;

    #endregion

    #region UnityMethods

    void Start()
    {
        foreach (TileReporter reporter in transform.GetComponentsInChildren<TileReporter>())
        {
            tileReporters.Add(reporter);
        }
    }

    #endregion

    #region CustomMethods

    private void ResetArea()
    {
        foreach (Tile tile in reporterTiles)
        {
            // Reset preview on enemy healthbar
            if (tile.tileOccupied && tile.characterOnTile != null)
            {
                tile.characterOnTile.PreviewDamage(0);
            }

            if (tile.tileHasObject && tile.objectOnTile != null)
            {
                tile.objectOnTile.PreviewDamage(0);
            }

            tile.ChangeTileEffect(TileEnums.TileEffects.attackable, false);
        }
    }

    //Colors all tiles in its area of effect
    private void ColourArea(bool highlightOccupied)
    {
        foreach (Tile tile in reporterTiles)
        {
            if(tile.tileOccupied && highlightOccupied || tile.tileData.tileType == effectedTileType && freeRange || tile.tileHasObject && highlightOccupied)
            {
                tile.ChangeTileColor(TileEnums.TileMaterial.highlight);
            }
            tile.ChangeTileEffect(TileEnums.TileEffects.attackable, true);
        }
    }

    //Resets the area than checks what tiles it is interacting with
    public void DetectArea(bool illustrate, bool highlightOccupied)
    {
        ResetArea();

        reporterTiles.Clear();
        foreach (TileReporter reporter in tileReporters)
        {
            if(reporter.currentTile != null)
            {
                reporterTiles.Add(reporter.currentTile);
            }
        }

        if(illustrate)
        {
            ColourArea(highlightOccupied);
        }
    }

    //Returns a list of all Characters in its area based on the provided type
    public List<Character> CharactersHit(TurnEnums.CharacterType type)
    {
        List<Character> characters = new List<Character>();
        foreach (Tile tile in reporterTiles)
        {
            if (tile.tileOccupied && tile.characterOnTile.characterType == type)
            {
                characters.Add(tile.characterOnTile);
            }
        }
        return characters;
    }

    //Returns a list of all objects in its area
    public List<TileObject> ObjectsHit()
    {
        List<TileObject> objects = new List<TileObject>();
        foreach(Tile tile in reporterTiles)
        {
            if(tile.tileHasObject)
            {
                objects.Add(tile.objectOnTile);
            }
        }
        return objects;
    }

    //Returns a list of tiles being effected by the AttackArea
    public List<Tile> TilesHit()
    {
        List<Tile> tileList = new List<Tile>();
        foreach(Tile tile in reporterTiles)
        {
            tileList.Add(tile);
        }
        return tileList;
    }

    //Checks if its area contains a tile
    public bool ContainsTile(Tile tileToCheck)
    {
        foreach(Tile tile in reporterTiles)
        {
            if(tileToCheck == tile)
            {
                return true;
            }
        }

        return false;
    }

    public void PositionAndRotateAroundCharacter(Pathfinder pathfinder, Tile originTile, Tile targetTile)
    {
        Tile selectedTile = null;
        float distance = 1000f;

        foreach (Tile tile in pathfinder.FindAdjacentTiles(originTile, true))
        {
            float newDistance = Vector3.Distance(targetTile.transform.position, tile.transform.position);
            if (newDistance < distance)
            {
                selectedTile = tile;
                distance = newDistance;
            }
        }

        Vector3 newPos = selectedTile.transform.position;
        newPos.y = 0;

        transform.position = newPos;
        Rotate(selectedTile, originTile);
    }

    public void Rotate(Tile targetTile, Tile originTile)
    {
        Transform originTransform = originTile.transform;
        Transform tileTransform = targetTile.transform;

        float rotation = originTransform.eulerAngles.y;

        float angle = Vector3.Angle(originTransform.forward, (tileTransform.position - originTransform.position));

        if (Vector3.Distance(tileTransform.position, originTransform.position + (originTransform.right * 6)) <
            Vector3.Distance(tileTransform.position, originTransform.position + (-originTransform.right) * 6))
        {
            rotation += angle;
        }
        else
        {
            rotation -= angle;
        }

        transform.eulerAngles = new Vector3(0, rotation, 0);
    }

    public void DestroySelf()
    {
        ResetArea();
        Destroy(this.gameObject);
    }

    public static AttackArea SpawnAttackArea(AttackArea attackArea)
    {
        return Instantiate(attackArea);
    }

    #endregion
}
