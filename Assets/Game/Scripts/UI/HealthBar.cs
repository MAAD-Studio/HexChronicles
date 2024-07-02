using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class HealthBar : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI characterName;
    [SerializeField] protected TextMeshProUGUI hpText;
    [SerializeField] protected Image health;
    [SerializeField] protected Image previewHealth;
    [SerializeField] protected RectTransform bar;
    [SerializeField] public float damagePreview;

    [SerializeField] protected GameObject statusField;
    [SerializeField] protected GameObject statusPrefab;
    protected List<Status> status;

    protected abstract void Start();
    protected abstract void OnDestroy();
    protected abstract void UpdateHealthBarPreview();
    protected abstract void UpdateHealthBar();
    protected abstract void UpdateStatus();

    protected IEnumerator AnimateHealthBar(float targetFillAmount)
    {
        float elapsedTime = 0f;
        float animationDuration = 1.5f;
        while (elapsedTime < animationDuration)
        {
            health.fillAmount = Mathf.Lerp(health.fillAmount, targetFillAmount, elapsedTime / animationDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
