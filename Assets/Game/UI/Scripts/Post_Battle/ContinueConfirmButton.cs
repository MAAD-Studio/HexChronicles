using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContinueConfirmButton : MonoBehaviour
{
    [SerializeField] private VictoryScreen victoryScreen;
    [SerializeField] private VictoryReward victoryReward;
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private Button confirmBtn;
    [SerializeField] private Button hidePanelBtn;

    private Button button;

    public bool IsConfirmPanelActive => confirmPanel.activeSelf;

    private void Start()
    {
        confirmPanel.SetActive(false);
        button = GetComponent<Button>();
        button.onClick.AddListener(OnContinue);
        confirmBtn.onClick.AddListener(() => victoryScreen.OnReturnToMap());
        hidePanelBtn.onClick.AddListener(() => HideConfirmPanel());
    }

    private void OnContinue()
    {
        // Add seleced skill to the player
        if (victoryReward.SelectedSkill == null)
        {
            confirmPanel.SetActive(true);
            confirmPanel.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack).From(0);
        }
        else
        {
            ActiveSkillCollection.Instance.PlayerAddSkill(victoryReward.SelectedSkill);
            victoryScreen.OnReturnToMap();
        }
    }

    public void HideConfirmPanel()
    {
        confirmPanel.transform.DOScale(0, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            confirmPanel.SetActive(false);
        });
    }

    public void Reset()
    {
        confirmPanel.SetActive(false);
    }
}
