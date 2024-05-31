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

    TurnManager turnManager;

    protected override void Start()
    {
        base.Start();

        // Temporary events
        turnManager = FindObjectOfType<TurnManager>();
        turnManager.OnLevelDefeat += ShowDefeat;
        TileObject.objectDestroyed.AddListener(LevelVictory);
        pauseMenu = MenuManager.Instance.GetMenu<Menu>(pauseMenuClassifier);
    }

    private void LevelVictory(TileObject arg0)
    {
        ShowVictory();
        TileObject.objectDestroyed.RemoveListener(LevelVictory);
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
    public void ShowVictory()
    {
        MenuManager.Instance.ShowMenu(MenuManager.Instance.VictoryScreenClassifier);
        MenuManager.Instance.HideMenu(menuClassifier);
    }

    // Only For Testing
    public void ShowDefeat(object sender, EventArgs e)
    {
        MenuManager.Instance.ShowMenu(MenuManager.Instance.DefeatedScreenClassifier);
        MenuManager.Instance.HideMenu(menuClassifier);

        turnManager.OnLevelDefeat -= ShowDefeat;
    }
}
