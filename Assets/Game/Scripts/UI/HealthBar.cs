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
    [SerializeField] public float damagePreview;

    protected abstract void Start();
    protected abstract void OnDestroy();
    protected abstract void UpdateHealthBarPreview();
    protected abstract void UpdateHealthBar();

}
