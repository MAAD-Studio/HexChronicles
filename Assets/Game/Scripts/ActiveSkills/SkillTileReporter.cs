using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTileReporter : MonoBehaviour
{
    #region Variables

    [HideInInspector] public Tile collisionTile;
    [SerializeField] public ActiveSkill skill;

    #endregion

    #region UnityMethods

    private void OnTriggerStay(Collider other)
    {
        collisionTile = other.transform.GetComponent<Tile>();
        if (collisionTile != null)
        {
            if(collisionTile.tileOccupied && collisionTile.characterOnTile.characterType == TurnEnums.CharacterType.Player)
            {
                return;
            }

            collisionTile.ChangeTileColor(TileEnums.TileMaterial.attackable);
            collisionTile.inFrontier = true;
            skill.affectedTiles.Add(collisionTile);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(collisionTile != null)
        {
            collisionTile.ChangeTileColor(TileEnums.TileMaterial.baseMaterial);
            collisionTile.inFrontier = false;
            //skill.affectedTiles.Remove(collisionTile);
        }
    }

    #endregion
}
