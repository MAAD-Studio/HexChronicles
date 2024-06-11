using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHoverUI : MonoBehaviour
{
    public Image element;
    public TextMeshProUGUI textHP;
    public TextMeshProUGUI textAttack;
    public TextMeshProUGUI textMovement;
    public TextMeshProUGUI textRange;
    public TextMeshProUGUI textDef;
    public TextMeshProUGUI textStatus;

    public CharacterUIConfig characterUIConfig;

    public void SetStats(Enemy_Base enemy)
    {
        element.sprite = characterUIConfig.GetElementSprite(enemy.elementType);
        textHP.text = $"{enemy.currentHealth} / {enemy.maxHealth}";
        textAttack.text = $"{enemy.attackDamage}";
        textMovement.text = $"{enemy.moveDistance}";
        textRange.text = $"{enemy.attackDistance}";
        textDef.text = $"{enemy.defensePercentage}%";
        textStatus.text = characterUIConfig.GetStatusTypes(enemy).ToString();

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
