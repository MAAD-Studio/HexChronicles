using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [HideInInspector] public Character character;
    [SerializeField] private Image health;
    [SerializeField] private Image damagePreview;
    [SerializeField] private TextMeshProUGUI hpText;

    void Start()
    {
        
    }

    void Update()
    {
        health.fillAmount = character.currentHealth / character.maxHealth;
        damagePreview.fillAmount = character.currentHealth / character.maxHealth;
        hpText.text = character.currentHealth + " HP";
    }
}
