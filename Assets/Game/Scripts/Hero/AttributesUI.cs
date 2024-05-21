using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.TextCore.Text;

public class AttributesUI : MonoBehaviour
{
    public Character character;
    public TextMeshProUGUI text;
    private bool isHero;
    private Hero hero;
    private Enemy_Base enemy;

    private void Start()
    {
        if (character is Hero)
        {
            isHero = true;
            hero = (Hero)character;
        }
        else if (character is Enemy_Base)
        {
            isHero = false;
            enemy = (Enemy_Base)character;
        }
        else
        {
            Debug.LogError("Character is not a Hero or Enemy_Basic");
        }
    }

    private void Update()
    {
        if (character != null && isHero)
        {
            text.text = "Hero: " + hero.heroSO.attributes.name + "\n" +
                            "Health: " + hero.currentHealth + " / " + hero.maxHealth + "\n" +
                            "Movement Range: " + hero.moveDistance + "\n" +
                            "Attack Damage: " + hero.attackDamage + "\n" +
                            //"Attack Range: " + hero.heroSO.attributes.attackRange + "\n" +
                            "Defense: " + hero.defensePercentage + "\n" +
                            "Element: " + hero.heroSO.attributes.elementType + "\n" +
                            "Status: " + GetStatusTypes();
        }
        else if (character != null && !isHero)
        {
            text.text = "Enemy: " + enemy.enemySO.attributes.name + "\n" +
                            "Health: " + enemy.currentHealth + " / " + enemy.maxHealth + "\n" +
                            "Movement Range: " + enemy.moveDistance + "\n" +
                            "Attack Damage: " + enemy.attackDamage + "\n" +
                            //"Attack Range: " + enemy.enemySO.attributes.attackRange + "\n" +
                            "Defense: " + enemy.defensePercentage + "\n" +
                            "Element: " + enemy.enemySO.attributes.elementType + "\n" +
                            "Status: " + GetStatusTypes();
        }
    }

    private string GetStatusTypes()
    {
        string statusList = "";
        foreach (var status in character.statusList)
        {
            statusList += status.statusType.ToString() + ", ";
        }
        return statusList;
    }
}
