using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Attributes/Basic Upgrade")]
public class BasicUpgradeSO : ScriptableObject
{
    public List<BasicUpgrade> upgrades;
    public string upgradeName;
    public Sprite image;
    public KeywordDescription description;
}