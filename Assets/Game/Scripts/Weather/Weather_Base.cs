using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather_Base : MonoBehaviour
{
    #region Variables

    [HideInInspector] public string weatherName;

    [Header("Weather Details: ")]
    [SerializeField] private WeatherType weatherType;
    public WeatherType WeatherType
    {
        get { return weatherType; }
    }

    [SerializeField] private ElementType element;
    public ElementType Element
    {
        get { return element; }
    }

    [Header("Materials: ")]
    [SerializeField] public Material weatherMaterial;

    [Header("Effects Info: ")]
    [Range(0f, 10f)]
    [SerializeField] protected int effectTurns = 1;

    [SerializeField] protected Status.StatusTypes primaryEffect = Status.StatusTypes.None;
    [SerializeField] protected Status.StatusTypes secondaryEffect = Status.StatusTypes.None;

    [SerializeField] protected int healAffect = 1;
    [SerializeField] protected int damageAffect = 1;

    #endregion

    #region CustomMethods

    public virtual void ApplyEffect(List<Character> characters)
    {
    }

    public virtual void ApplyTileEffect(Tile tile, TurnManager turnManager, WeatherPatch patch)
    {
    }

    protected void ApplyStatusToCharacter(Character character, Status.StatusTypes type)
    {
        Status newStatus = new Status();
        newStatus.statusType = type;
        newStatus.effectTurns = effectTurns;

        character.AddStatus(newStatus);
    }

    #endregion
}
