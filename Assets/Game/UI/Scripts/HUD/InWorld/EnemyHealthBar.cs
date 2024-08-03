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

        float maxHealth = enemy.enemySO.attributes.health;
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
        previewBar.value = enemy.currentHealth;
        healthBar.value = previewBar.value;

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
            previewBar.value = previewBar.minValue;
        }
        else
        {
            killIcon.gameObject.SetActive(false);
            previewBar.value = newHealth;
        }

        hpText.text = newHealth.ToString();

        prediction.gameObject.SetActive(true);
        prediction.ShowHealth(enemy.currentHealth, newHealth);
        parentBar.localScale = scaledUpValue;
    }

    protected override void UpdateHealthBar()
    {
        float currentHealth = enemy.currentHealth;
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
