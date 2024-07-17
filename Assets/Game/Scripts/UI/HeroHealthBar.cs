using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class HeroHealthBar : HealthBar
{
    [HideInInspector] public Hero hero;
    
    protected override void Start()
    {
        hero = GetComponentInParent<Hero>();
        hero.healthBar = this;
        hero.DamagePreview.AddListener(UpdateHealthBarPreview);
        hero.UpdateHealthBar.AddListener(UpdateHealthBar);
        hero.UpdateStatus.AddListener(UpdateStatus);

        characterName.text = hero.heroSO.attributes.name.ToString();
        hpText.text = hero.heroSO.attributes.health.ToString();

        float width = hero.heroSO.attributes.health * 10f;
        healthBar.sizeDelta = new Vector2(Mathf.Clamp(width, 60, 100), health.rectTransform.sizeDelta.y);

        previewHealth.fillAmount = 1;
        health.fillAmount = 1;
    }

    protected override void UpdateHealthBarPreview()
    {
        float newHealth = hero.currentHealth - damagePreview;
        hpText.text = newHealth.ToString();
        previewHealth.fillAmount = newHealth / hero.maxHealth;
    }

    protected override void UpdateHealthBar()
    {
        hpText.text = hero.currentHealth.ToString();
        StartCoroutine(AnimateHealthBar(hero.currentHealth / hero.maxHealth, false));
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
        hero.DamagePreview.RemoveListener(UpdateHealthBarPreview);
        hero.UpdateHealthBar.RemoveListener(UpdateHealthBar);
        hero.UpdateStatus.RemoveListener(UpdateStatus);
    }
}
