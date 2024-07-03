using UnityEngine;

[CreateAssetMenu(menuName = "Character Attributes/Hero Attributes")]
public class HeroAttributesSO : ScriptableObject
{
    public BasicAttributes attributes = new BasicAttributes();

    public AttackArea attackArea;
    public Sprite attackShape;
    public KeywordDescription attackInfo;

    public GameObject phantomModel;

    public ActiveSkillSO activeSkillSO;

    public void SetActiveSkill(ActiveSkillSO newSkill)
    {
        activeSkillSO = newSkill;
        Debug.Log($"{attributes.name} has selected {newSkill.skillName}");
    }
}
