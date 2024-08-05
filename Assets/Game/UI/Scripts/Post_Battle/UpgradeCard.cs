using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeCard : MonoBehaviour
{
    public BasicUpgradeSO basicUpgrade;

    [Header("UI")]
    public Image image;
    public TextMeshProUGUI nameText;
    public GameObject attributeParent;
    public GameObject attributePrefab;
    public TextMeshProUGUI description;
    [SerializeField] private List<Sprite> sprites;

    private void Start()
    {
        // TODO: Pick a random upgrade from upgrade collection - which doesn't exist now

        InitializeUI();
    }

    public void InitializeUI()
    {
        nameText.text = basicUpgrade.upgradeName;
        image.sprite = basicUpgrade.image;
        description.text = basicUpgrade.description.DisplayKeywordDescription();
        
        foreach (BasicUpgrade upgrade in basicUpgrade.upgrades)
        {
            GameObject newAttribute = Instantiate(attributePrefab, attributeParent.transform);
            UpgradeAttribute upgradeAttribute = newAttribute.GetComponent<UpgradeAttribute>();

            upgradeAttribute.valueText.text = "+" + upgrade.value.ToString();

            if (upgrade.attributeType == BasicAttributeType.Health)
            {
                upgradeAttribute.attributeIcon.sprite = sprites[0];
                upgradeAttribute.lableText.text = "Health";
            }
            else if (upgrade.attributeType == BasicAttributeType.MovementRange)
            {
                upgradeAttribute.attributeIcon.sprite = sprites[1];
                upgradeAttribute.lableText.text = "Move";
            }
            else if (upgrade.attributeType == BasicAttributeType.AttackDamage)
            {
                upgradeAttribute.attributeIcon.sprite = sprites[2];
                upgradeAttribute.lableText.text = "Attack";
            }
            else if (upgrade.attributeType == BasicAttributeType.DefensePercentage)
            {
                upgradeAttribute.valueText.text = "+" + upgrade.value.ToString() + "%";
                upgradeAttribute.attributeIcon.sprite = sprites[3];
                upgradeAttribute.lableText.text = "Defense";
            }
        }

        // Recalculate the layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    public void ApplyUpgrades(Hero hero)
    {
        foreach (BasicUpgrade upgrade in basicUpgrade.upgrades)
        {
            hero.AddUpgrade(upgrade);
        }
    }
}
