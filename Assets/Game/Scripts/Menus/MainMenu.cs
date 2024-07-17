using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static GameManager;

public class MainMenu : Menu
{
    public SceneReference Tutorial;
    public SceneReference Map;
    public MenuClassifier tutorialHUDClassifier;
    public MenuClassifier mapClassifier;

    [Header("UI")]
    [SerializeField] private GameObject loadGamePanel;
    [SerializeField] private GameObject savedDataPrefab;
    [SerializeField] private Transform savedDataContainer;

    protected override void Start()
    {
        base.Start();
        loadGamePanel.SetActive(false);
    }

    public void OnStartTutorial()
    {
        SceneLoader.Instance.OnSceneLoadedEvent += OnSceneLoaded;

        SceneLoader.Instance.LoadScene(Tutorial);
    }

    private void OnSceneLoaded(List<string> list)
    {
        SceneLoader.Instance.OnSceneLoadedEvent -= OnSceneLoaded;

        MenuManager.Instance.HideMenu(menuClassifier);
        MenuManager.Instance.ShowMenu(tutorialHUDClassifier);
    }

    public void OnContinueGame()
    {
        SceneLoader.Instance.LoadScene(Map);

        MenuManager.Instance.HideMenu(menuClassifier);
        MenuManager.Instance.ShowMenu(mapClassifier);
    }

    public void OnReturnToMainMenu()
    {
        Time.timeScale = 1.0f;

        MenuManager.Instance.ShowMenu(MenuManager.Instance.LoadingScreenClassifier);
        MenuManager.Instance.HideMenu(MenuManager.Instance.HUDMenuClassifier);
        MenuManager.Instance.HideMenu(MenuManager.Instance.TutorialHUDClassifier);

        SceneLoader.Instance.OnScenesUnLoadedEvent += AllScenesUnloaded;
        SceneLoader.Instance.UnLoadAllLoadedScenes();
    }
    
    private void AllScenesUnloaded()
    {
        SceneLoader.Instance.OnScenesUnLoadedEvent -= AllScenesUnloaded;

        MenuManager.Instance.ShowMenu(MenuManager.Instance.MainMenuClassifier);
        MenuManager.Instance.HideMenu(MenuManager.Instance.LoadingScreenClassifier);
    }

    public void ShowSavedData()
    {
        loadGamePanel.SetActive(true);
        foreach (var data in GameManager.Instance.SaveList)
        {
            AddSavedDataToUI(data);
        }
    }

    private void AddSavedDataToUI(SaveData data)
    {
        var item = Instantiate(savedDataPrefab, savedDataContainer);
        item.GetComponentInChildren<TextMeshProUGUI>().text = FormatSavedData(data);
    }

    private string FormatSavedData(SaveData data)
    {
        return $"Current Level: {data.CurrentLevel}\nSaved Time: {data.SaveTime}";
    }

    public void OnQuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
