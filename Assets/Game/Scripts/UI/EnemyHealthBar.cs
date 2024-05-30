using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : HealthBar
{
    [SerializeField] private Image killIcon;
    [SerializeField] private GameObject atkInfoPanel;
    [SerializeField] private TextMeshProUGUI atkPercentage;

    protected override void Start()
    {
        base.Start();
        killIcon.enabled = false;
        atkPercentage.enabled = false;
    }

    protected override void Update()
    {
        base.Update();
        if (isCharacter)
        {
            if ((character.currentHealth - damagePreview) <= 0)
            {
                killIcon.enabled = true;
            }
            else
            {
                killIcon.enabled = false;
            }
        }
        else
        {
            if ((tileObject.currentHealth - damagePreview) <= 0)
            {
                killIcon.enabled = true;
            }
            else
            {
                killIcon.enabled = false;
            }
        }
    }
}
