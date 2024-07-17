using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level Data")]
public class LevelSO : ScriptableObject
{
    [Header("Level Basic")]
    public string levelName;
    public Sprite levelImage;
    [TextArea(3, 10)]
    public string levelDescription;
    public SceneReference levelScene;
    public List<EnemyAttributesSO> enemies;

    [Header("Level Objective")]
    public int limitTurns;
    public string primaryObjective;
    [TextArea(3, 10)]
    public string primaryObjectiveDescription;

    [Header("Level Rewards")]
    public List<ActiveSkillSO> activeSkills;
    public List<BasicUpgradeSO> upgrades;
}

