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
        MenuManager.Instance.GetMenu<WorldMap>(MenuManager.Instance.WorldMapClassifier)?.OnReturnToMap();
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
