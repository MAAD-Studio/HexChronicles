using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PreGameScreen : Menu
{
    [SerializeField] private Button startButton;
    public GameObject worldMap;

    [SerializeField] private GameObject heroList;
    private List<HeroSkillInfo> heroSkillList;
    [SerializeField] private LevelDetailUI levelDetail;

    protected override void Start()
    {
        base.Start();
        startButton.onClick.AddListener(StartLevel);

        // Get every HeroSkillInfo from heroList:
        heroSkillList = new List<HeroSkillInfo>(heroList.GetComponentsInChildren<HeroSkillInfo>());
        
        SkillSelectionUI skillSelectionUI = GetComponent<SkillSelectionUI>();
        skillSelectionUI.Initialize(heroSkillList);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ClosePreGame();
        }
    }

    public void UpdatePreGameScreen(int levelIndex)
    {
        // Invoke the first hero in the list as the default display
        heroSkillList[0].OnSelected();

        // Show the level information:
        levelDetail.InitializeInfo(GameManager.Instance.levelDetails[levelIndex]);
    }

    private void StartLevel()
    {
        WorldMap map = worldMap.GetComponent<WorldMap>();
        map?.LoadLevel();

        MenuManager.Instance.HideMenu(menuClassifier);
    }

    public void ClosePreGame()
    {
        MenuManager.Instance.ShowMenu(MenuManager.Instance.WorldMapClassifier);
        MenuManager.Instance.HideMenu(menuClassifier);
    }
}
