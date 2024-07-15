using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather_HeatWave : Weather_Base
{
    #region Variables

    [SerializeField] private int healthDebuff = 1;

    #endregion

    #region UnityMethods

    private void Start()
    {
        weatherName = "HeatWave";
    }

    #endregion

    #region CustomMethods

    public override void ApplyEffect(List<Character> characters)
    {
        foreach (Character character in characters)
        {
            if (character.elementType == ElementType.Fire)
            {
                Status newStatus = new Status();
                newStatus.statusType = Status.StatusTypes.AttackBoost;
                newStatus.effectTurns = effectTurns;

                character.AddStatus(newStatus);
            }
            else if (character.elementType == ElementType.Grass)
            {
                Status newStatus = new Status();
                newStatus.statusType = Status.StatusTypes.MovementReduction;
                newStatus.effectTurns = effectTurns;

                character.AddStatus(newStatus);

                character.TakeDamage(healthDebuff, ElementType.Base);
            }

            if (Status.GrabIfStatusActive(character, statusEffect) == null && character.elementType != ElementType.Fire)
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
