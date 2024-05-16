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

    private void ColourArea()
    {
        foreach (Tile tile in reporterTiles)
        {
            tile.ChangeTileColor(TileEnums.TileMaterial.attackable);
        }
    }

    public void DetectArea()
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

        ColourArea();
    }

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

    public void DestroySelf()
    {
        ResetArea();
        Destroy(this);
    }

    public static AttackArea SpawnAttackArea(AttackArea attackArea)
    {
        return Instantiate(attackArea);
    }

    #endregion
}
