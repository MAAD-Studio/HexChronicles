using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSkill : MonoBehaviour
{
    #region Variables

    [HideInInspector] public List<Tile> affectedTiles = new List<Tile>();

    #endregion

    //Resets the Tiles affected by the SkillTileReporters. Important for when the ActiveSkill gets Destroyed
    public void ResetTiles()
    {
        foreach(Tile tile in affectedTiles)
        {
            tile.ChangeTileColor(TileEnums.TileMaterial.baseMaterial);
            tile.inFrontier = false;
        }
    }

    //PlayerTurn needs this method to Instantiate ActiveSkills for it since it isn't a MonoBehavior
    public static GameObject SpawnSelf(ActiveSkill activeSkill)
    {
        return Instantiate(activeSkill.gameObject, new Vector3(100, 100, 100), Quaternion.identity);
    }

    //PlayerTurn needs this method to Destroy ActiveSkills for it since it isn't a MonoBehavior
    public void DestroySelf()
    {
        ResetTiles();
        Destroy(this.gameObject);
    }
}
