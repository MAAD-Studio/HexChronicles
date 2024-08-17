using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CharacterInfoUITip : UITip
{
    [SerializeField] private GameObject tipsParent;

    protected override void Start()
    {
        base.Start();
        tipsParent.SetActive(false);
    }

    public override void ShowTooltip()
    {
        tipsParent.SetActive(true); 
        base.ShowTooltip();
    }

    protected override void HideTooltip()
    {
        base.HideTooltip();
        tipsParent.SetActive(false);
    }
}
