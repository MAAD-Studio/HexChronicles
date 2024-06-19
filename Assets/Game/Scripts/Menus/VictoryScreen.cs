using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryScreen : Menu
{
    [SerializeField] private Button continueButton;
    [SerializeField] private GameObject upgradeCard;

    protected override void Start()
    {
        base.Start();
        continueButton.onClick.AddListener(OnReturnToMap);
        WorldTurnBase.Victory.AddListener(OnVictory);
    }

    private void OnVictory()
    {
        // Spawn 3 upgrade cards
        /*for (int i = 0; i < 3; i++)
        {
            GameObject card = Instantiate(upgradeCard, transform);
            card.GetComponent<UpgradeCard>().InitializeUI();
        }*/
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
