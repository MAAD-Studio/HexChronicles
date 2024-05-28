using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDMenu : Menu
{
    public MenuClassifier pauseMenuClassifier;

    private Menu pauseMenu;


    protected override void Start()
    {
        base.Start();
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
    public void ShowVictory()
    {
        MenuManager.Instance.ShowMenu(MenuManager.Instance.VictoryScreenClassifier);
        MenuManager.Instance.HideMenu(menuClassifier);
    }

    // Only For Testing
    public void ShowDefeat()
    {
        MenuManager.Instance.ShowMenu(MenuManager.Instance.DefeatedScreenClassifier);
        MenuManager.Instance.HideMenu(menuClassifier);
    }
}
