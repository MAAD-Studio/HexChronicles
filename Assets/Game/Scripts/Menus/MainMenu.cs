using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : Menu
{
    public SceneReference Map;
    public MenuClassifier tutorialHUDClassifier;
    public MenuClassifier mapClassifier;

    [Header("UI")]
    [SerializeField] private GameObject loadGamePanel;
    [SerializeField] private GameObject savedDataPrefab;
    [SerializeField] private Transform savedDataContainer;

    private SelectTutorial selectTutorial;

    protected override void Start()
    {
        base.Start();
        AudioManager.Instance.PlayMusicFadeIn("MainTheme", 2);

        selectTutorial = GetComponent<SelectTutorial>();
        selectTutorial.HidePanel();

        HideSavedData();
    }

    public void OnSelectTutorial()
    {
        selectTutorial.ShowPanel();
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


    #region Show Save and Load
    public void ShowSavedData()
    {
        loadGamePanel.SetActive(true);

        if (GameManager.Instance.SaveLoadManager.SaveList.Count == 0)
        {
            GameObject gameObject = new GameObject("NoSavedData");
            gameObject.transform.SetParent(savedDataContainer);
            TextMeshProUGUI text = gameObject.AddComponent<TextMeshProUGUI>();
            text.text = "No saved data found!";
            text.fontSize = 22;
            return;
        }

        // Add each saved data to the UI panel
        foreach (var data in GameManager.Instance.SaveLoadManager.SaveList)
        {
            GameObject item = Instantiate(savedDataPrefab, savedDataContainer);
            item.GetComponentInChildren<TextMeshProUGUI>().text = 
                $"<b>Current Level: {data.CurrentLevel + 1}\nHas Skills: {data.PlayerSkills.Count}</b>\nSaved Time: {data.SaveTime}";

            Button button = item.GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                GameManager.Instance.SaveLoadManager.ApplySavedData(data);
                MenuManager.Instance.GetMenu<WorldMap>(MenuManager.Instance.WorldMapClassifier).RefreshLevelButtons();
                HideSavedData();
                OnContinueGame();
            });
        }
    }

    public void HideSavedData()
    {
        loadGamePanel.SetActive(false);
        foreach (Transform child in savedDataContainer)
        {
            Destroy(child.gameObject);
        }
    }
    #endregion

    public void OnQuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
