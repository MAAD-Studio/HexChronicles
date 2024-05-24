using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class PauseMenu : Menu
{
    public MenuClassifier hudMenuClassifier;

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

    public void OnReturnToMainMenu()
    {
        MenuManager.Instance.GetMenu<MainMenu>(MenuManager.Instance.MainMenuClassifier)?.OnReturnToMainMenu();
        MenuManager.Instance.HideMenu(menuClassifier);

        CleanActiveScene();
    }

    public void OnReturnToMap()
    {
        MenuManager.Instance.GetMenu<WorldMap>(MenuManager.Instance.MapClassifier)?.OnReturnToMap();
        MenuManager.Instance.HideMenu(menuClassifier);

        CleanActiveScene();
    }

    private void CleanActiveScene()
    {
        foreach (GameObject go in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            Destroy(go);
        }
    }
}
