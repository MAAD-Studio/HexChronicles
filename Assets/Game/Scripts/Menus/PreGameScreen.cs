using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PreGameScreen : Menu
{
    [SerializeField] private Button startButton;
    public GameObject worldMap; // testing

    protected override void Start()
    {
        base.Start();
        startButton.onClick.AddListener(OnStartGame);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ClosePreGame();
        }
    }

    // Only For Testing
    private void OnStartGame()
    {
        WorldMap map = worldMap.GetComponent<WorldMap>();
        map?.OnLoadLevel(map.levels[0]);

        MenuManager.Instance.HideMenu(menuClassifier);
    }

    public void ClosePreGame()
    {
        MenuManager.Instance.ShowMenu(MenuManager.Instance.WorldMapClassifier);
        MenuManager.Instance.HideMenu(menuClassifier);
    }
}
