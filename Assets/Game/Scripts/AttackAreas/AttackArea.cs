using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    #region Variables

    List<TileReporter> tileReporters = new List<TileReporter>();
    List<Tile> reporterTiles = new List<Tile>();

    #endregion

    #region UnityMethods

    // Start is called before the first frame update
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
            tile.ChangeTileColor(TileEnums.TileMaterial.baseMaterial);
        }
    }

    //Colors all tiles in its area of effect
    private void ColourArea()
    {
        foreach (Tile tile in reporterTiles)
        {
            if(tile.tileOccupied)
            {
                tile.ChangeTileColor(TileEnums.TileMaterial.highlight);
            }
            else
            {
                tile.ChangeTileColor(TileEnums.TileMaterial.attackable);
            }
        }
    }

    //Resets the area than checks what tiles it is interacting with
    public void DetectArea(bool illustrate)
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
            ColourArea();
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
