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
            heroText.text = "Hero: " + hero.heroSO.attributes.heroName + "\n" +
                            "Description: " + hero.heroSO.attributes.description + "\n" +
                            "Health: " + hero.heroSO.attributes.health + "/" + hero.originalHealth + "\n" +
                            "Movement Range: " + hero.heroSO.attributes.movementRange + "\n" +
                            "Attack Damage: " + hero.heroSO.attributes.attackDamage + "\n" +
                            "Attack Range: " + hero.heroSO.attributes.attackRange + "\n" +
                            "Defense: " + hero.heroSO.attributes.defensePercentage + "\n" +
                            "Element: " + hero.heroSO.attributes.elementType;
        }
    }
}
