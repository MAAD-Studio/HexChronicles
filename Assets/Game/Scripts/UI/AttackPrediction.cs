using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttackPrediction : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private GameObject healthItem;
    [SerializeField] private TextMeshProUGUI currentHealth;
    [SerializeField] private TextMeshProUGUI newHealth;

    [Header("Remove Status")]
    [SerializeField] private GameObject removeStatusItem;

    [SerializeField] private GameObject currentTurnsItem;
    [SerializeField] private Image currentStatusIcon;
    [SerializeField] private TextMeshProUGUI currentTurns;

    [SerializeField] private Image removeStatusIcon;
    [SerializeField] private TextMeshProUGUI removeStatusText;

    [Header("Add Status")]
    [SerializeField] private GameObject addStatusItem;

    [SerializeField] private Image newStatusIcon;
    [SerializeField] private TextMeshProUGUI newTurns;
    [SerializeField] private TextMeshProUGUI addStatusText;

    [SerializeField] private GameObject changeTurnsItem;
    [SerializeField] private Image oldStatusIcon;
    [SerializeField] private TextMeshProUGUI oldTurns;

    [Header("Status Sprites")]
    [SerializeField] private Sprite burning;
    [SerializeField] private Sprite wet;
    [SerializeField] private Sprite bound;

    public void ShowHealth(float currentHealth, float newHealth)
    {
        healthItem.SetActive(true);
        this.currentHealth.text = currentHealth.ToString();
        if (currentHealth >= 10)
        {
            this.currentHealth.fontSize = 10;
        }
        else
        {
            this.currentHealth.fontSize = 14;
        }
        this.newHealth.text = newHealth.ToString();
        if (newHealth >= 10)
        {
            this.newHealth.fontSize = 10;
        }
        else
        {
            this.newHealth.fontSize = 14;
        }
    }

    public void RemoveStatus(Status status)
    {
        removeStatusItem.SetActive(true);
        if (status.statusType == Status.StatusTypes.Burning)
        {
            currentStatusIcon.sprite = burning;
            removeStatusIcon.sprite = burning;
            removeStatusText.text = "Burn";
        }
        else if (status.statusType == Status.StatusTypes.Wet)
        {
            currentStatusIcon.sprite = wet;
            removeStatusIcon.sprite = wet;
            removeStatusText.text = "Wet";
        }
        else if (status.statusType == Status.StatusTypes.Bound)
        {
            currentStatusIcon.sprite = bound;
            removeStatusIcon.sprite = bound;
            removeStatusText.text = "Bound";
        }

        if (status.effectTurns == 0)
        {
            currentTurnsItem.SetActive(false);
        }
        else
        {
            currentTurnsItem.SetActive(true);
            currentTurns.text = status.effectTurns.ToString();
        }
    }

    public void AddStatus(Status status)
    {
        addStatusItem.SetActive(true);
        changeTurnsItem.SetActive(false);

        if (status.statusType == Status.StatusTypes.Burning)
        {
            newStatusIcon.sprite = burning;
            addStatusText.text = "Burn";
        }
        else if (status.statusType == Status.StatusTypes.Wet)
        {
            newStatusIcon.sprite = wet;
            addStatusText.text = "Wet";
        }
        else if (status.statusType == Status.StatusTypes.Bound)
        {
            newStatusIcon.sprite = bound;
            addStatusText.text = "Bound";
        }
        newTurns.text = status.effectTurns.ToString();
    }

    public void ChangeStatus(Status status, int newTurns)
    {
        addStatusItem.SetActive(true);
        changeTurnsItem.SetActive(true);

        if (status.statusType == Status.StatusTypes.Burning)
        {
            newStatusIcon.sprite = burning;
            addStatusText.text = "Burn";
        }
        else if (status.statusType == Status.StatusTypes.Wet)
        {
            newStatusIcon.sprite = wet;
            addStatusText.text = "Wet";
        }
        else if (status.statusType == Status.StatusTypes.Bound)
        {
            newStatusIcon.sprite = bound;
            addStatusText.text = "Bound";
        }

        oldStatusIcon.sprite = newStatusIcon.sprite;
        oldTurns.text = status.effectTurns.ToString();
        this.newTurns.text = newTurns.ToString();
    }

    public void Hide()
    {
        healthItem.SetActive(false);
        removeStatusItem.SetActive(false);
        addStatusItem.SetActive(false);
        gameObject.SetActive(false);
    }
}
