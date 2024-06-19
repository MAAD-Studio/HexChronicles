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
        EventBus.Instance.Subscribe<OnNewLevelStart>(OnNewLevel);
        
        pauseMenu = MenuManager.Instance.GetMenu<Menu>(pauseMenuClassifier);
    }

    private void OnNewLevel(object obj)
    {
        TurnManager.LevelDefeat.AddListener(LevelDefeat);
        WorldTurnBase.Victory.AddListener(LevelVictory);
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
    private void LevelVictory()
    {
        MenuManager.Instance.ShowMenu(MenuManager.Instance.VictoryScreenClassifier);
        MenuManager.Instance.HideMenu(menuClassifier);

        WorldTurnBase.Victory.RemoveListener(LevelVictory);
    }

    // Only For Testing
    private void LevelDefeat()
    {
        MenuManager.Instance.ShowMenu(MenuManager.Instance.DefeatedScreenClassifier);
        MenuManager.Instance.HideMenu(menuClassifier);

        TurnManager.LevelDefeat.RemoveListener(LevelDefeat);
    }

    // Only For Testing
    public void InvokeVictory()
    {
        WorldTurnBase.Victory.Invoke();
    }
    public void InvokeDefeat()
    {
        TurnManager.LevelDefeat.Invoke();
    }
}
