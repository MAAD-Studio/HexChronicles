using UnityEngine;

[CreateAssetMenu(menuName = "Character Attributes/Hero Attributes")]
public class HeroAttributesSO : ScriptableObject
{
    [Header("Basic Attributes")]
    public BasicAttributes attributes = new BasicAttributes();

    [Header("Attack Information")]
    public AttackArea attackArea;
    public Sprite attackShape;
    public KeywordDescription attackInfo;

    public GameObject phantomModel;

    public ActiveSkillSO activeSkillSO;

    public void SetActiveSkill(ActiveSkillSO newSkill)
    {
        activeSkillSO = newSkill;
    }
}
