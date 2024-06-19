using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObjectHealthBar : EnemyHealthBar
{
    [HideInInspector] public TileObject tileObject;

    protected override void Start()
    {
        tileObject = GetComponentInParent<TileObject>();
        tileObject.healthBar = this;
        tileObject.DamagePreview.AddListener(UpdateHealthBarPreview);
        tileObject.UpdateHealthBar.AddListener(UpdateHealthBar);

        characterName.text = tileObject.tileObjectData.objectName.ToString();
        atkPercentage.text = (100 - tileObject.tileObjectData.defense).ToString() + "%";
        hpText.text = tileObject.tileObjectData.health + " HP";
        previewHealth.fillAmount = 1;
        health.fillAmount = 1;
    }

    protected override void UpdateHealthBarPreview()
    {
        previewHealth.fillAmount = (tileObject.currentHealth - damagePreview) / tileObject.tileObjectData.health;

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

        if ((tileObject.currentHealth - damagePreview) <= 0)
        {
            killIcon.gameObject.SetActive(true);
        }
        else
        {
            killIcon.gameObject.SetActive(false);
        }
    }

    protected override void UpdateHealthBar()
    {
        health.fillAmount = tileObject.currentHealth / tileObject.tileObjectData.health;
        hpText.text = tileObject.currentHealth + " HP";

        damagePreview = 0;
        UpdateHealthBarPreview();
    }

    protected override void OnDestroy()
    {
        tileObject.DamagePreview.RemoveListener(UpdateHealthBarPreview);
        tileObject.UpdateHealthBar.RemoveListener(UpdateHealthBar);
    }
}
