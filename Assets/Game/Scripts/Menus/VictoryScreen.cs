using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryScreen : Menu
{
    [SerializeField] private Button continueButton;

    protected override void Start()
    {
        base.Start();
        continueButton.onClick.AddListener(OnReturnToMap);
    }

    public void OnReturnToMap()
    {
        MenuManager.Instance.GetMenu<WorldMap>(MenuManager.Instance.WorldMapClassifier)?.OnReturnToMap();
        MenuManager.Instance.HideMenu(menuClassifier);
    }
}
