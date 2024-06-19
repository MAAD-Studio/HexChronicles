using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PreGameScreen : Menu
{
    [SerializeField] private Button startButton;
    public GameObject worldMap;

    protected override void Start()
    {
        base.Start();
        startButton.onClick.AddListener(StartLevel);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ClosePreGame();
        }
    }

    private void StartLevel()
    {
        WorldMap map = worldMap.GetComponent<WorldMap>();
        map?.LoadLevel();

        MenuManager.Instance.HideMenu(menuClassifier);
    }

    public void ClosePreGame()
    {
        MenuManager.Instance.ShowMenu(MenuManager.Instance.WorldMapClassifier);
        MenuManager.Instance.HideMenu(menuClassifier);
    }
}
