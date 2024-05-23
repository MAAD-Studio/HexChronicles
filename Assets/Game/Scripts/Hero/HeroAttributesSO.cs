using UnityEngine;

[CreateAssetMenu(menuName = "Character Attributes/Hero Attributes")]
public class HeroAttributesSO : ScriptableObject
{
    public BasicAttributes attributes = new BasicAttributes();
    public AttackArea attackArea = new AttackArea();
    public ActiveSkill activeSkill = new ActiveSkill();

}
