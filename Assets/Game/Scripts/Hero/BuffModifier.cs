using UnityEngine;

[CreateAssetMenu(menuName = "Character Attributes/StatModifier")]
public class BuffModifier : ScriptableObject
{
    public BasicAttributeType attributeType;
    public float value;
    public bool isPercentage; // not used yet

    public BuffModifier(BasicAttributeType attributeType, float value, bool isPercentage)
    {
        this.attributeType = attributeType;
        this.value = value;
        this.isPercentage = isPercentage;
    }
}