using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TutorialHUDMenu : Menu
{
    public MenuClassifier pauseMenuClassifier;

    private Menu pauseMenu;

    protected override void Start()
    {
        base.Start();
        EventBus.Instance.Subscribe<OnNewLevelStart>(OnNewLevel);
        EventBus.Instance.Subscribe<PauseGame>(OnPauseGame);
        
        pauseMenu = MenuManager.Instance.GetMenu<Menu>(pauseMenuClassifier);
    }

    private void OnNewLevel(object obj)
    {
        TurnManager.LevelDefeat.AddListener(LevelDefeat);
        TurnManager.LevelVictory.AddListener(LevelVictory);
        WorldTurnBase.Victory.AddListener(LevelVictory);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EventBus.Instance.Publish(new PauseGame());

            // TODO: Disable tile selection

        }
    }

    public void OnPauseGame(object obj)
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

        UnsubscribeEvents();
    }

    private void UnsubscribeEvents()
    {
        TurnManager.LevelDefeat.RemoveListener(LevelDefeat);
        TurnManager.LevelVictory.RemoveListener(LevelVictory);
        WorldTurnBase.Victory.RemoveListener(LevelVictory);
    }

    // Only For Testing
    private void LevelDefeat()
    {
        MenuManager.Instance.ShowMenu(MenuManager.Instance.DefeatedScreenClassifier);
        MenuManager.Instance.HideMenu(menuClassifier);

        UnsubscribeEvents();
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
