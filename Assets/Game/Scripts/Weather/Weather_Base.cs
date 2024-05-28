using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather_Base : MonoBehaviour
{
    #region Variables

    [SerializeField] protected Status.StatusTypes statusEffect = Status.StatusTypes.CannotMove;
    [Range(0f, 10f)]
    [SerializeField] protected int effectTurns = 1;

    #endregion

    #region CustomMethods

    public virtual void ApplyEffect(List<Character> characters)
    {
        
    }

    #endregion
}