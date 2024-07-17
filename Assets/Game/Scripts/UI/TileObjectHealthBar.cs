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
        tileObject.DonePreview.AddListener(DonePreview);
        tileObject.UpdateHealthBar.AddListener(UpdateHealthBar);

        characterName.text = tileObject.tileObjectData.objectName.ToString();
        atkPercentage.text = (100 - tileObject.tileObjectData.defense).ToString() + "%";
        hpText.text = tileObject.tileObjectData.health + " HP";

        float width = tileObject.tileObjectData.health * 10f;
        bar.sizeDelta = new Vector2(Mathf.Clamp(width, 60, 100), health.rectTransform.sizeDelta.y);
        
        previewHealth.fillAmount = 1;
        health.fillAmount = 1;

        prediction.gameObject.SetActive(false);
    }

    private void DonePreview()
    {
        previewHealth.fillAmount = tileObject.currentHealth / tileObject.tileObjectData.health;
        hpText.text = tileObject.currentHealth + " HP";
        prediction.gameObject.SetActive(false);
        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    protected override void UpdateHealthBarPreview()
    {
        previewHealth.fillAmount = (tileObject.currentHealth - damagePreview) / tileObject.tileObjectData.health;
        hpText.text = (tileObject.currentHealth - damagePreview) + " HP";
        prediction.gameObject.SetActive(true);
        gameObject.transform.localScale = scaledUpValue;

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
        hpText.text = tileObject.currentHealth + " HP";
        StartCoroutine(AnimateHealthBar(tileObject.currentHealth / tileObject.tileObjectData.health));

        damagePreview = 0;
        UpdateHealthBarPreview();
    }

    protected override void OnDestroy()
    {
        tileObject.DamagePreview.RemoveListener(UpdateHealthBarPreview);
        tileObject.UpdateHealthBar.RemoveListener(UpdateHealthBar);
    }
}
