using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTile : Tile
{
    #region Variables

    [SerializeField] int damageOnCross = 1;
    [SerializeField] int damageOnStay = 1;

    #endregion

    #region CustomMethods

    public override void OnTileEnter(Character character)
    {
        if(character.characterType == TurnEnums.CharacterType.Player)
        {
            character.TakeDamage(damageOnCross, ElementType.Base);
        }
    }

    public override void OnTileStay(Character character)
    {
        if (character.characterType == TurnEnums.CharacterType.Player)
        {
            character.TakeDamage(damageOnStay, ElementType.Base);
        }
    }

    #endregion
}
