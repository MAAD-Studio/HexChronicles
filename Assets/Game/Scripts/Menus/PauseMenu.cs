using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class PauseMenu : Menu
{
    public MenuClassifier hudMenuClassifier;
    [SerializeField] private GameObject normalPanel;
    [SerializeField] private GameObject tutorialPanel;

    public static UnityEvent EndLevel = new UnityEvent();

    private void OnEnable()
    {
        if (GameManager.Instance.IsTutorial)
        {
            tutorialPanel.SetActive(true);
            normalPanel.SetActive(false);
        }
        else
        {
            tutorialPanel.SetActive(false);
            normalPanel.SetActive(true);
        }
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            OnResumeGame();
        }
    }

    public void OnResumeGame()
    {
        Time.timeScale = 1.0f;
        MenuManager.Instance.HideMenu(menuClassifier);
    }

    public void OnRestartLevel()
    {
        Time.timeScale = 1.0f;

        MenuManager.Instance.HideMenu(menuClassifier);
        MenuManager.Instance.HideMenu(hudMenuClassifier);

        EndBattle();
        GameManager.Instance.LoadCurrentLevel();
    }

    public void OnReturnToMainMenu()
    {
        Time.timeScale = 1.0f;
        AudioManager.Instance.PlayMusicFadeIn("MainTheme", 2);
        MenuManager.Instance.GetMenu<MainMenu>(MenuManager.Instance.MainMenuClassifier)?.OnReturnToMainMenu();
        MenuManager.Instance.HideMenu(menuClassifier);

        EndBattle();
    }

    public void OnReturnToMap()
    {
        Time.timeScale = 1.0f;
        AudioManager.Instance.PlayMusicFadeIn("MainTheme", 2);
        MenuManager.Instance.GetMenu<WorldMap>(MenuManager.Instance.WorldMapClassifier)?.OnReturnToMap();
        MenuManager.Instance.HideMenu(menuClassifier);

        EndBattle();
    }

    private void EndBattle()
    {
        EndLevel.Invoke();
        GameManager.Instance.CleanActiveScene();
    }

    public void OnEndTutorial()
    {
        AudioManager.Instance.PlayMusicFadeIn("MainTheme", 2);
        MainMenu mainMenu = MenuManager.Instance.GetMenu<MainMenu>(MenuManager.Instance.MainMenuClassifier);
        mainMenu.OnReturnToMainMenu();
        MenuManager.Instance.HideMenu(menuClassifier);
        GameManager.Instance.CleanActiveScene();

        mainMenu.OnSelectTutorial();
    }
}
