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
        hero.DamagePreview.AddListener(UpdateHealthBarPreview);
        hero.UpdateHealthBar.AddListener(UpdateHealthBar);

        characterName.text = hero.heroSO.attributes.name.ToString();
        hpText.text = hero.heroSO.attributes.health + " HP";

        float width = hero.heroSO.attributes.health * 10f;
        bar.sizeDelta = new Vector2(Mathf.Clamp(width, 60, 100), health.rectTransform.sizeDelta.y);

        previewHealth.fillAmount = 1;
        health.fillAmount = 1;
    }

    protected override void UpdateHealthBarPreview()
    {
        previewHealth.fillAmount = (hero.currentHealth - damagePreview) / hero.maxHealth;
    }

    protected override void UpdateHealthBar()
    {
        hpText.text = hero.currentHealth + " HP";
        StartCoroutine(AnimateHealthBar(hero.currentHealth / hero.maxHealth));

        damagePreview = 0;
        UpdateHealthBarPreview();
    }

    protected override void OnDestroy()
    {
        hero.DamagePreview.RemoveListener(UpdateHealthBarPreview);
        hero.UpdateHealthBar.RemoveListener(UpdateHealthBar);
    }
}
