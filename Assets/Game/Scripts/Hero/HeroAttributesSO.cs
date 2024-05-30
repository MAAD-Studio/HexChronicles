using UnityEngine;

[CreateAssetMenu(menuName = "Character Attributes/Hero Attributes")]
public class HeroAttributesSO : ScriptableObject
{
    public BasicAttributes attributes = new BasicAttributes();
    public AttackArea attackArea;
    public ActiveSkill activeSkill = new ActiveSkill();
    public GameObject phantomModel;
}
