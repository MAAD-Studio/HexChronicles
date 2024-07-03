using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// The skill to be selected in Skill Selection
public class ActiveSkillDisplay : MonoBehaviour
{
    public Button button;
    [SerializeField] private Image skillshape;
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI description;

    public void SetSkillDisplay(ActiveSkillSO skill)
    {
        skillshape.sprite = skill.skillshape;
        skillName.text = skill.skillName;
        description.text = skill.description.DisplayKeywordDescription();
    }

    public void OnSelected()
    {
        button.interactable = false;

        // Reset the other skill buttons
        foreach (Transform child in transform.parent)
        {
            if (child != transform)
            {
                child.GetComponent<ActiveSkillDisplay>().button.interactable = true;
            }
        }
    }
}
