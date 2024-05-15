using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : Menu
{
    public SceneReference Level;
    public MenuClassifier hudClassifier;

    public void OnLoadLevel()
    {
        SceneLoader.Instance.LoadScene(Level);
        MenuManager.Instance.HideMenu(menuClassifier);

        MenuManager.Instance.ShowMenu(hudClassifier);
    }

    public void OnReturnToMainMenu()
    {
        Time.timeScale = 1.0f;

        MenuManager.Instance.ShowMenu(MenuManager.Instance.LoadingScreenClassifier);
        MenuManager.Instance.HideMenu(MenuManager.Instance.HUDMenuClassifier);

        SceneLoader.Instance.OnScenesUnLoadedEvent += AllScenesUnloaded;
        SceneLoader.Instance.UnLoadAllLoadedScenes();
    }
    
    private void AllScenesUnloaded()
    {
        SceneLoader.Instance.OnScenesUnLoadedEvent -= AllScenesUnloaded;

        MenuManager.Instance.ShowMenu(MenuManager.Instance.MainMenuClassifier);
        MenuManager.Instance.HideMenu(MenuManager.Instance.LoadingScreenClassifier);
    }
}
