using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryScreen : Menu
{
    [SerializeField] private GameObject tutorialVictory;
    [SerializeField] private GameObject normalVictory;
    
    private VictoryReward reward;

    protected override void Start()
    {
        base.Start();
        WorldTurnBase.Victory.AddListener(OnVictory);
        TurnManager.LevelVictory.AddListener(OnVictory);
        reward = GetComponent<VictoryReward>();
    }

    private void OnVictory()
    {
        if (GameManager.Instance.IsTutorial)
        {
            tutorialVictory.SetActive(true);
            normalVictory.SetActive(false);
        }
        else 
        {
            tutorialVictory.SetActive(false);
            normalVictory.SetActive(true);
            reward.SpawnCards();
        }
    }

    public void OnReturnToMap()
    {
        // Save level progress
        GameManager.Instance.CurrentLevelIndex++;
        GameManager.Instance.SaveLoadManager.SaveGame();

        AudioManager.Instance.PlayMusicFadeIn("MainTheme", 2);
        MenuManager.Instance.GetMenu<WorldMap>(MenuManager.Instance.WorldMapClassifier)?.OnReturnToMap();
        MenuManager.Instance.HideMenu(menuClassifier);

        GameManager.Instance.CleanActiveScene();
    }

    public void OnReturnToSelectTutorial()
    {
        AudioManager.Instance.PlayMusicFadeIn("MainTheme", 2);
        MainMenu mainMenu = MenuManager.Instance.GetMenu<MainMenu>(MenuManager.Instance.MainMenuClassifier);
        mainMenu.OnReturnToMainMenu();
        MenuManager.Instance.HideMenu(menuClassifier);
        GameManager.Instance.CleanActiveScene();

        mainMenu.OnSelectTutorial();
    }
}
