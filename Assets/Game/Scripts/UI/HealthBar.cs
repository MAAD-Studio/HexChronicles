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
    [SerializeField] protected RectTransform parentBar;
    [SerializeField] protected RectTransform healthRect;
    [SerializeField] protected Slider healthBar;
    [SerializeField] protected RectTransform previewRect;
    [SerializeField] protected Slider previewBar;
    
    [SerializeField] protected GameObject statusField;
    [SerializeField] protected GameObject statusPrefab;

    protected abstract void Start();
    protected abstract void OnDestroy();
    protected abstract void UpdateHealthBar();
    protected abstract void UpdateStatus();

    protected IEnumerator AnimateHealthBar(float targetValue, bool isPreview)
    {
        float elapsedTime = 0f;
        float animationDuration = 1.5f;
        while (elapsedTime < animationDuration)
        {
            if (isPreview)
            {
                previewBar.value = Mathf.Lerp(previewBar.value, targetValue, elapsedTime / animationDuration);
            }
            else
            {
                healthBar.value = Mathf.Lerp(healthBar.value, targetValue, elapsedTime / animationDuration);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
