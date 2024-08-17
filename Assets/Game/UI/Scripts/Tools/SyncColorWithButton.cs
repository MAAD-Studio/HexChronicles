using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;
using Unity.VisualScripting;

public class SyncColorWithButton : MonoBehaviour
{
    [SerializeField] private ButtonChangeNotifier notifier;

    private Image icon;
    private TextMeshProUGUI text;

    private void Awake()
    {
        icon = GetComponent<Image>();
        text = GetComponent<TextMeshProUGUI>();

        notifier.onButtonChange.AddListener(OnButtonChange);
    }

    private void Start()
    {
        UpdateColor();
    }

    private void OnButtonChange()
    {
        UpdateColor();
    }

    private void UpdateColor()
    {
        Color color = notifier.GetCurrentButtonColor();

        if (icon != null)
        {
            icon.color = color;
        }

        if (text != null)
        {
            text.color = color;
        }
    }

    private void OnDestroy()
    {
        notifier.onButtonChange.RemoveListener(OnButtonChange);
    }
}
