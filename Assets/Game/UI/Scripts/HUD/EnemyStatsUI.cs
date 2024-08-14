using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStatsUI : StatsUI
{
    [Header("Enemy")]
    public TextMeshProUGUI textRange;
    public TextMeshProUGUI enemyInfo;
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetEnemyStats(Enemy_Base enemy)
    {
        avatar.sprite = enemy.enemySO.attributes.avatar;
        element.sprite = Config.Instance.GetElementSprite(enemy.elementType);
        textName.text = enemy.enemySO.attributes.name;
        enemyInfo.text = enemy.enemySO.attributes.description.DisplayKeywordDescription();
        enemyInfo.ForceMeshUpdate();
        textHP.text = $"{enemy.currentHealth} / {enemy.maxHealth}";
        textMovement.text = $"{enemy.moveDistance}";
        textAttack.text = $"{enemy.attackDamage}";
        textRange.text = $"{enemy.attackDistance}";
        textDef.text = $"{enemy.defensePercentage}%";
        textStatus.text = Config.Instance.GetStatusTypes(enemy).ToString();

        gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }

    public void SetObjectStats(TileObject tileObject)
    {
        avatar.sprite = tileObject.tileObjectData.avatar;
        textName.text = tileObject.tileObjectData.objectName;
        enemyInfo.text = tileObject.tileObjectData.description.DisplayKeywordDescription();
        enemyInfo.ForceMeshUpdate();
        textHP.text = $"{tileObject.currentHealth} / {tileObject.tileObjectData.health}";
        //textAttack.text = $"{tileObject.tileObjectData.attackDamage}";
        //textRange.text = $"{tileObject.tileObjectData.attackDistance}";
        //textDef.text = $"{tileObject.tileObjectData.defense}%";
        //textStatus.text = GetStatusTypes(tileObject).ToString();
        //textStatus.text = "";

        gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
