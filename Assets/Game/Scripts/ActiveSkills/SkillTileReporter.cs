using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTileReporter : MonoBehaviour
{
    #region Variables

    [HideInInspector] public Tile collisionTile;
    [HideInInspector] public Tile newTile;

    [SerializeField] public ActiveSkill skill;

    #endregion

    #region UnityMethods

    private void OnTriggerStay(Collider other)
    {
        newTile = other.transform.GetComponent<Tile>();

        //Checks that we are colliding with a Tile and it isn't the same Tile we were already colliding with
        if (newTile != null && newTile != collisionTile)
        {
            collisionTile = newTile;

            //If the Tile has a Player Character on it ignore it
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

            skill.affectedTiles.Remove(collisionTile);
        }
    }

    #endregion
}
