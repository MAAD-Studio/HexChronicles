using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HUDMenu : Menu
{
    public MenuClassifier pauseMenuClassifier;

    private Menu pauseMenu;

    protected override void Start()
    {
        base.Start();

        TurnManager.OnLevelDefeat.AddListener(ShowDefeat);
        WorldTurnBase.Victory.AddListener(ShowVictory);
        pauseMenu = MenuManager.Instance.GetMenu<Menu>(pauseMenuClassifier);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnPauseGame();

            // TODO: Disable tile selection

        }
    }

    public void OnPauseGame()
    {
        if (pauseMenu.gameObject.activeInHierarchy == false)
        {
            Time.timeScale = 0.0f;
            MenuManager.Instance.ShowMenu(pauseMenuClassifier);
        }
    }

    // Only For Testing
    private void ShowVictory()
    {
        MenuManager.Instance.ShowMenu(MenuManager.Instance.VictoryScreenClassifier);
        MenuManager.Instance.HideMenu(menuClassifier);

        WorldTurnBase.Victory.RemoveListener(ShowVictory);
    }

    // Only For Testing
    private void ShowDefeat(TurnManager arg0)
    {
        MenuManager.Instance.ShowMenu(MenuManager.Instance.DefeatedScreenClassifier);
        MenuManager.Instance.HideMenu(menuClassifier);

        TurnManager.OnLevelDefeat.RemoveListener(ShowDefeat);
    }
}
