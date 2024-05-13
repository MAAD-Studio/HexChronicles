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
            heroText.text = "Hero: " + hero.heroAttributes.heroName + "\n" +
                            "Description: " + hero.heroAttributes.description + "\n" +
                            "Health: " + hero.heroAttributes.health + "/" + hero.originalHealth + "\n" +
                            "Movement Range: " + hero.heroAttributes.movementRange + "\n" +
                            "Attack Damage: " + hero.heroAttributes.attackDamage + "\n" +
                            "Attack Range: " + hero.heroAttributes.attackRange + "\n" +
                            "Defense: " + hero.heroAttributes.defensePercentage + "\n" +
                            "Element: " + hero.heroAttributes.elementType;
        }
    }
}
