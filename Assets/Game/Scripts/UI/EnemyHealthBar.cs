using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : HealthBar
{
    [HideInInspector] public Enemy_Base enemy;
    [SerializeField] protected Image killIcon;
    [SerializeField] protected GameObject atkInfoPanel;
    [SerializeField] protected TextMeshProUGUI atkPercentage;
    [SerializeField] protected Vector3 scaledUpValue = new Vector3(1.3f, 1.3f, 1.3f);

    protected override void Start()
    {
        enemy = GetComponentInParent<Enemy_Base>();
        enemy.healthBar = this;
        enemy.OnDamagePreview += UpdateHealthBarPreview;
        enemy.OnUpdateHealthBar += UpdateHealthBar;

        characterName.text = enemy.enemySO.name.ToString();
        atkPercentage.text = (100 - enemy.enemySO.attributes.defensePercentage).ToString() + "%";
        hpText.text = enemy.enemySO.attributes.health + " HP";
        previewHealth.fillAmount = 1;
        health.fillAmount = 1;
    }

    protected override void UpdateHealthBarPreview(object sender, EventArgs e)
    {
        previewHealth.fillAmount = (enemy.currentHealth - damagePreview) / enemy.maxHealth;

        if (damagePreview != 0)
        {
            gameObject.transform.localScale = scaledUpValue;
            atkInfoPanel.SetActive(true);
        }
        else
        {
            gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
            atkInfoPanel.SetActive(false);
        }

        if ((enemy.currentHealth - damagePreview) <= 0)
        {
            killIcon.gameObject.SetActive(true);
        }
        else
        {
            killIcon.gameObject.SetActive(false);
        }
    }

    protected override void UpdateHealthBar(object sender, EventArgs e)
    {
        health.fillAmount = enemy.currentHealth / enemy.maxHealth;
        hpText.text = enemy.currentHealth + " HP";

        damagePreview = 0;
        UpdateHealthBarPreview(this, EventArgs.Empty);
    }

    protected override void OnDestroy()
    {
        enemy.OnDamagePreview -= UpdateHealthBarPreview;
        enemy.OnUpdateHealthBar -= UpdateHealthBar;
    }
}
