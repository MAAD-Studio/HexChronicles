using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryScreen : Menu
{
    [SerializeField] private Button continueButton;
    
    private VictoryReward reward;

    protected override void Start()
    {
        base.Start();
        continueButton.onClick.AddListener(OnReturnToMap);
        WorldTurnBase.Victory.AddListener(OnVictory);
        reward = GetComponent<VictoryReward>();
    }

    private void OnVictory()
    {
        reward.SpawnCards();
    }

    public void OnReturnToMap()
    {
        GameManager.Instance.CurrentLevelIndex++;
        GameManager.Instance.SaveLoadManager.SaveGame();

        MenuManager.Instance.GetMenu<WorldMap>(MenuManager.Instance.WorldMapClassifier)?.OnReturnToMap();
        MenuManager.Instance.HideMenu(menuClassifier);

        GameManager.Instance.CleanActiveScene();
    }
}
