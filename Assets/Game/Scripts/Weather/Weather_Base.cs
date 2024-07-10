using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather_Base : MonoBehaviour
{
    #region Variables

    // !!! TEMP USE - Consider add enum for weather types
    [HideInInspector] public string weatherName;

    [SerializeField] protected Status.StatusTypes statusEffect = Status.StatusTypes.CannotMove;
    [Range(0f, 10f)]
    [SerializeField] protected int effectTurns = 1;

    #endregion

    #region CustomMethods

    public virtual void ApplyEffect(List<Character> characters)
    {
        
    }

    public virtual void ApplyTileEffect(Tile tile, TurnManager turnManager)
    {

    }

    #endregion
}
