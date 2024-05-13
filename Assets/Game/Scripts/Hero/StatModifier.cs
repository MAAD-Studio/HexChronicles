using UnityEngine;

[CreateAssetMenu(menuName = "Character Attributes/StatModifier")]
public class StatModifier : ScriptableObject
{
    public BasicAttributeType attributeType;
    public float value;
    public bool isPercentage; // not actually used

    public StatModifier(BasicAttributeType attributeType, float value, bool isPercentage)
    {
        this.attributeType = attributeType;
        this.value = value;
        this.isPercentage = isPercentage;
    }
}