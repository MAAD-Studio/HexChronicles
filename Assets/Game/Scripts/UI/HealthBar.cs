using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public bool isCharacter;
    [HideInInspector] public Character character;
    [HideInInspector] public TileObject tileObject;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] protected Image health;
    [SerializeField] protected Image previewHealth;
    [SerializeField] protected float damagePreview;

    protected virtual void Start()
    {
        if (isCharacter)
        {
            if (character is Enemy_Base)
            {
                Enemy_Base enemy = (Enemy_Base)character;
                characterName.text = "" + enemy.enemySO.name;
            }
            else if (character is Hero)
            {
                Hero hero = (Hero)character;
                characterName.text = "" + hero.heroSO.attributes.name;
            }
        }
        else
        {
            characterName.text = "Spawner Tower";
        }
    }

    protected virtual void Update()
    {
        if (isCharacter)
        {
            health.fillAmount = character.currentHealth / character.maxHealth;
            previewHealth.fillAmount = character.currentHealth / character.maxHealth;
            hpText.text = character.currentHealth + " HP";
        }
        else
        {
            health.fillAmount = tileObject.currentHealth / tileObject.tileObjectData.health;
            previewHealth.fillAmount = tileObject.currentHealth / tileObject.tileObjectData.health;
            hpText.text = tileObject.currentHealth + " HP";
        }
    }
}
