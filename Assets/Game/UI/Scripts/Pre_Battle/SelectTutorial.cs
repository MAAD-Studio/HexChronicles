using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectTutorial : MonoBehaviour
{
    [SerializeField] private GameObject selectPanel;
    [SerializeField] private Button backBtn;
    [SerializeField] private Button startBtn;

    [SerializeField] private SelectTutorialCard[] tutorialCardsInOrder;
    [SerializeField] private SceneReference[] tutorialsInOrder;

    private SceneReference selectedTutorial;

    void Start()
    {
        backBtn.onClick.AddListener(() => HidePanel());
        startBtn.onClick.AddListener(() => StartTutorial());

        for (int i = 0; i < tutorialCardsInOrder.Length; i++)
        {
            int index = i; // Prevent referencing the last value of i
            tutorialCardsInOrder[i].SetTutorial(this, tutorialsInOrder[index]);
        }
        tutorialCardsInOrder[0].OnPointerClick(null);
    }

    public void OnTutorialSelected(SceneReference tutorial)
    {
        selectedTutorial = tutorial;
    }

    public void ResetTutorialCards()
    {
        foreach (SelectTutorialCard card in tutorialCardsInOrder)
        {
            card.IdleState();
        }
    }

    private void StartTutorial()
    {
        SceneLoader.Instance.OnSceneLoadedEvent += OnSceneLoaded;

        GameManager.Instance.IsTutorial = true;
        SceneLoader.Instance.LoadScene(selectedTutorial);
        MenuManager.Instance.HideMenu(MenuManager.Instance.MainMenuClassifier);
    }

    private void OnSceneLoaded(List<string> list)
    {
        SceneLoader.Instance.OnSceneLoadedEvent -= OnSceneLoaded;

        selectPanel.SetActive(false);
        MenuManager.Instance.ShowMenu(MenuManager.Instance.TutorialHUDClassifier);
        EventBus.Instance.Publish(new OnTutorialStart());
    }

    public void ShowPanel()
    {
        selectPanel.SetActive(true);
    }

    public void HidePanel()
    {
        selectPanel.SetActive(false);
    }
}
