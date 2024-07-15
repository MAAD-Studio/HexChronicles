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
                Status newStatus = new Status();
                newStatus.statusType = Status.StatusTypes.MovementReduction;
                newStatus.effectTurns = effectTurns;

                character.AddStatus(newStatus);

                character.TakeDamage(healthDebuff, ElementType.Base);
            }
            else if (character.elementType == ElementType.Grass)
            {
               
            }

            if (Status.GrabIfStatusActive(character, statusEffect) == null && character.elementType != ElementType.Grass)
            {
                Status newStatus = new Status();
                newStatus.statusType = statusEffect;
                newStatus.effectTurns = effectTurns;

                character.AddStatus(newStatus);
            }

            character.effectedByWeather = true;
        }
    }

    public override void ApplyTileEffect(Tile tile, TurnManager turnManager, WeatherPatch patch)
    {
       
    }

    #endregion
}
