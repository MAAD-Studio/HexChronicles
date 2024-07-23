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
        hpText.text = tileObject.tileObjectData.health.ToString();

        float width = tileObject.tileObjectData.health * 10f;
        healthBar.sizeDelta = new Vector2(Mathf.Clamp(width, 60, 100), health.rectTransform.sizeDelta.y);
        
        previewHealth.fillAmount = 1;
        health.fillAmount = 1;

        prediction.Hide();
    }

    private void DonePreview()
    {
        hpText.text = tileObject.currentHealth.ToString();
        previewHealth.fillAmount = tileObject.currentHealth / tileObject.tileObjectData.health;
        health.fillAmount = previewHealth.fillAmount;

        prediction.Hide();
        killIcon.gameObject.SetActive(false);
        parentBar.localScale = new Vector3(1f, 1f, 1f);
    }

    protected override void UpdateHealthBarPreview(int arg0)
    {
        float newHealth = tileObject.currentHealth - arg0;

        if (newHealth <= 0)
        {
            killIcon.gameObject.SetActive(true);
            newHealth = 0;
        }
        else
        {
            killIcon.gameObject.SetActive(false);
        }

        hpText.text = newHealth.ToString();
        previewHealth.fillAmount = newHealth / tileObject.tileObjectData.health;
        //StartCoroutine(AnimateHealthBar(newHealth / enemy.maxHealth, true));
        
        prediction.gameObject.SetActive(true);
        prediction.ShowHealth(tileObject.currentHealth, newHealth);
        killIcon.gameObject.SetActive(false);
        parentBar.localScale = scaledUpValue;
    }

    protected override void UpdateHealthBar()
    {
        hpText.text = tileObject.currentHealth.ToString();
        previewHealth.fillAmount = tileObject.currentHealth / tileObject.tileObjectData.health;
        StartCoroutine(AnimateHealthBar(tileObject.currentHealth / tileObject.tileObjectData.health, false));
    }

    protected override void OnDestroy()
    {
        tileObject.DamagePreview.RemoveListener(UpdateHealthBarPreview);
        tileObject.DonePreview.RemoveListener(DonePreview);
        tileObject.UpdateHealthBar.RemoveListener(UpdateHealthBar);
    }
}
