using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather_SporeStorm : Weather_Base
{
    #region Variables

    [SerializeField] private int healthDebuff = 1;

    #endregion

    #region UnityMethods

    private void Start()
    {
        weatherName = "SporeStorm";
    }

    #endregion

    #region CustomMethods

    public override void ApplyEffect(List<Character> characters)
    {
        foreach (Character character in characters)
        {
            if (character.elementType == ElementType.Water)
            {
                ApplyStatusToCharacter(character, Status.StatusTypes.MovementReduction);
                character.TakeDamage(healthDebuff, ElementType.Base);
            }
            else if (character.elementType == ElementType.Grass)
            {
               
            }

            if (Status.GrabIfStatusActive(character, primaryEffect) == null && character.elementType != ElementType.Grass)
            {
                ApplyStatusToCharacter(character, primaryEffect);
            }
        }
    }

    public override void ApplyTileEffect(Tile tile, TurnManager turnManager)
    {
       
    }

    #endregion
}
