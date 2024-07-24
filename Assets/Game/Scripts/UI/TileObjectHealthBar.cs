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

        float maxHealth = tileObject.tileObjectData.health;
        hpText.text = maxHealth.ToString();

        float width = maxHealth * 10f;
        healthRect.sizeDelta = new Vector2(Mathf.Clamp(width, 60, 100), healthRect.sizeDelta.y);
        previewRect.sizeDelta = new Vector2(Mathf.Clamp(width, 60, 100), previewRect.sizeDelta.y);

        healthBar.maxValue = maxHealth;
        healthBar.value = maxHealth;
        previewBar.maxValue = maxHealth;
        previewBar.value = maxHealth;

        prediction.Hide();
    }

    private void DonePreview()
    {
        hpText.text = tileObject.currentHealth.ToString();
        previewBar.value = tileObject.currentHealth;
        healthBar.value = previewBar.value;

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
            previewBar.value = previewBar.minValue;
        }
        else
        {
            killIcon.gameObject.SetActive(false);
            previewBar.value = newHealth;
        }

        hpText.text = newHealth.ToString();
        //StartCoroutine(AnimateHealthBar(newHealth / enemy.maxHealth, true));
        
        prediction.gameObject.SetActive(true);
        prediction.ShowHealth(tileObject.currentHealth, newHealth);
        killIcon.gameObject.SetActive(false);
        parentBar.localScale = scaledUpValue;
    }

    protected override void UpdateHealthBar()
    {
        float currentHealth = tileObject.currentHealth;
        hpText.text = currentHealth.ToString();
        if (currentHealth <= 0)
        {
            previewBar.value = previewBar.minValue;
            StartCoroutine(AnimateHealthBar(healthBar.minValue, false));
        }
        else
        {
            previewBar.value = currentHealth;
            StartCoroutine(AnimateHealthBar(currentHealth, false));
        }
    }

    protected override void OnDestroy()
    {
        tileObject.DamagePreview.RemoveListener(UpdateHealthBarPreview);
        tileObject.DonePreview.RemoveListener(DonePreview);
        tileObject.UpdateHealthBar.RemoveListener(UpdateHealthBar);
    }
}
