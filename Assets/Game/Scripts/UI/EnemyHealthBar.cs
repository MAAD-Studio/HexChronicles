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

    [SerializeField] protected AttackPrediction prediction;

    protected override void Start()
    {
        enemy = GetComponentInParent<Enemy_Base>();
        enemy.healthBar = this;
        enemy.DamagePreview.AddListener(UpdateHealthBarPreview);
        enemy.AddStatusPreview.AddListener(AddStatusPreview);
        enemy.ChangeStatusPreview.AddListener(ChangeStatusPreview);
        enemy.RemoveStatusPreview.AddListener(RemoveStatusPreview);
        enemy.DonePreview.AddListener(DonePreview);
        enemy.UpdateHealthBar.AddListener(UpdateHealthBar);
        enemy.UpdateStatus.AddListener(UpdateStatus);

        characterName.text = enemy.enemySO.name.ToString();
        atkPercentage.text = (100 - enemy.enemySO.attributes.defensePercentage).ToString() + "%";
        hpText.text = enemy.enemySO.attributes.health.ToString();

        float width = enemy.enemySO.attributes.health * 10f;
        healthBar.sizeDelta = new Vector2(Mathf.Clamp(width, 60, 100), health.rectTransform.sizeDelta.y);
        previewHealth.fillAmount = 1;
        health.fillAmount = 1;

        prediction.Hide();
    }

    private void RemoveStatusPreview(Status arg0)
    {
        prediction.RemoveStatus(arg0);
    }

    private void ChangeStatusPreview(Status arg0, int newTurns)
    {
        prediction.ChangeStatus(arg0, newTurns);
    }

    private void AddStatusPreview(Status arg0)
    {
        prediction.AddStatus(arg0);
    }

    private void DonePreview()
    {
        //TODO: Done Preview is now happening every time right after Preview
        hpText.text = enemy.currentHealth.ToString();
        previewHealth.fillAmount = enemy.currentHealth / enemy.maxHealth;
        health.fillAmount = previewHealth.fillAmount;

        prediction.Hide();
        killIcon.gameObject.SetActive(false);
        parentBar.localScale = new Vector3(1f, 1f, 1f);
    }

    protected virtual void UpdateHealthBarPreview(int arg0)
    {
        float newHealth = enemy.currentHealth - arg0;

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
        previewHealth.fillAmount = newHealth / enemy.maxHealth;
        //StartCoroutine(AnimateHealthBar(newHealth / enemy.maxHealth, true));

        prediction.gameObject.SetActive(true);
        prediction.ShowHealth(enemy.currentHealth, newHealth);
        parentBar.localScale = scaledUpValue;
    }

    protected override void UpdateHealthBar()
    {
        hpText.text = enemy.currentHealth.ToString();
        previewHealth.fillAmount = enemy.currentHealth / enemy.maxHealth;
        StartCoroutine(AnimateHealthBar(enemy.currentHealth / enemy.maxHealth, false));
    }

    protected override void UpdateStatus()
    {
        List<Status> newStatus = enemy.statusList;

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
        enemy.DamagePreview.RemoveListener(UpdateHealthBarPreview);
        enemy.AddStatusPreview.RemoveListener(AddStatusPreview);
        enemy.ChangeStatusPreview.RemoveListener(ChangeStatusPreview);
        enemy.RemoveStatusPreview.RemoveListener(RemoveStatusPreview);
        enemy.DonePreview.RemoveListener(DonePreview);
        enemy.UpdateHealthBar.RemoveListener(UpdateHealthBar);
        enemy.UpdateStatus.RemoveListener(UpdateStatus);
    }
}
