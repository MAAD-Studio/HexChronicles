using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroHealthBar : HealthBar
{
    [HideInInspector] public Hero hero;
    
    protected override void Start()
    {
        hero = GetComponentInParent<Hero>();
        hero.healthBar = this;
        hero.UpdateHealthBar.AddListener(UpdateHealthBar);
        hero.UpdateStatus.AddListener(UpdateStatus);

        characterName.text = hero.heroSO.attributes.name.ToString();

        float maxHealth = hero.heroSO.attributes.health;
        hpText.text = maxHealth.ToString();

        float width = hero.heroSO.attributes.health * 10f;
        healthRect.sizeDelta = new Vector2(Mathf.Clamp(width, 60, 100), healthRect.sizeDelta.y);
        previewRect.sizeDelta = new Vector2(Mathf.Clamp(width, 60, 100), previewRect.sizeDelta.y);
        
        healthBar.maxValue = maxHealth;
        healthBar.value = maxHealth;
        previewBar.maxValue = maxHealth;
        previewBar.value = maxHealth;
    }

    protected override void UpdateHealthBar()
    {
        float currentHealth = hero.currentHealth;
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

    protected override void UpdateStatus()
    {
        List<Status> newStatus = hero.statusList;

        foreach (Transform child in statusField.transform)
        {
            Destroy(child.gameObject);
        }

        if (newStatus != null)
        {
            foreach (var status in newStatus)
            {
                GameObject statusObject = Instantiate(statusPrefab, statusField.transform, false);
                StatusEffect statusEffect = statusObject.GetComponent<StatusEffect>();
                statusEffect.Initialize(status);
            }
        }
    }

    protected override void OnDestroy()
    {
        hero.UpdateHealthBar.RemoveListener(UpdateHealthBar);
        hero.UpdateStatus.RemoveListener(UpdateStatus);
    }
}
