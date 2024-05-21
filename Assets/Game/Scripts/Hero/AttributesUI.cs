using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttributesUI : MonoBehaviour
{
    public Hero hero;
    public TextMeshProUGUI heroText;

    private void Update()
    {
        if (hero != null)
        {
            heroText.text = "Hero: " + hero.heroSO.attributes.name + "\n" +
                            "Description: " + hero.heroSO.attributes.description + "\n" +
                            "Health: " + hero.currentHealth + " / " + hero.maxHealth + "\n" +
                            "Movement Range: " + hero.moveDistance + "\n" +
                            "Attack Damage: " + hero.attackDamage + "\n" +
                            //"Attack Range: " + hero.heroSO.attributes.attackRange + "\n" +
                            "Defense: " + hero.defensePercentage + "\n" +
                            "Element: " + hero.heroSO.attributes.elementType + "\n" +
                            "Status: " + GetStatusTypes();
        }
    }

    private string GetStatusTypes()
    {
        string statusList = "";
        foreach (var status in hero.statusList)
        {
            statusList += status.statusType.ToString() + ", ";
        }
        return statusList;
    }
}
