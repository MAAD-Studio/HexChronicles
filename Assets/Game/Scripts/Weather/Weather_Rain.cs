using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather_Rain : Weather_Base
{
    #region UnityMethods

    #endregion

    #region CustomMethods

    public override void ApplyEffect(List<Character> characters)
    {
        foreach(Character character in characters)
        {
            Status newStatus = new Status();
            newStatus.statusType = statusEffect;
            newStatus.effectTurns = effectTurns;
            character.AddStatus(newStatus);
            character.effectedByWeather = true;
        }
    }

    #endregion
}
