using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class EndTurnButton : MonoBehaviour
{
    public Button endTurnBtn;

    [Header("Effects")]
    [SerializeField] private GameObject endTurnVFX;
    [SerializeField] private Color highlightColor;
    [SerializeField] private Vector3 scaleUpValue = new Vector3(1.2f, 1.5f, 1.2f);
    [SerializeField] private float scaleUpDuration = 0.6f;
    [SerializeField] private float scaleBackDuration = 0.3f;

    [Header("Confirm Panel")]
    [SerializeField] private GameObject askPanel;
    [SerializeField] private Button actualEndTurnBtn;
    [SerializeField] private Button hidePanelBtn;

    private Image image;
    private bool shouldScale = false;
    private ButtonChangeNotifier notifier;
    private bool isHide = false;
    private bool keepActive = false;

    private void Awake()
    {
        image = GetComponent<Image>();
        notifier = GetComponent<ButtonChangeNotifier>();
    }

    private void Start()
    {
        EventBus.Instance.Subscribe<UpdateCharacterDecision>(OnUpdateCharacterDecision);

        hidePanelBtn.onClick.AddListener(() => HideAskPanel());

        askPanel.SetActive(false);
        actualEndTurnBtn.gameObject.SetActive(false);
    }

    public void AddEndTurnBtnListener(Action action)
    {
        endTurnBtn.onClick.AddListener(() => action());
    }

    public void AddConfirmBtnListener(Action action)
    {
        actualEndTurnBtn.onClick.AddListener(() =>
        {
            action();
            HideAskPanel();
        });
    }

    public void EnableButton()
    {
        endTurnBtn.interactable = true;
        notifier.onButtonChange?.Invoke();
    }

    public void DisableButton()
    {
        endTurnBtn.interactable = false;
        notifier.onButtonChange?.Invoke();
    }

    // Used for Tutorials
    public void HideEndTurn()
    {
        endTurnBtn.gameObject.SetActive(false);
        endTurnVFX.gameObject.SetActive(false);
        isHide = true;
        keepActive = false;
    }

    // Used for Tutorials
    public void ShowEndTurn()
    {
        endTurnBtn.gameObject.SetActive(true);
        endTurnVFX.gameObject.SetActive(true);
        isHide = false;
        keepActive = false;
    }

    public void KeepActive()
    {
        EndTurnActive();
        keepActive = true;
    }

    public void ShowAskPanel()
    {
        askPanel.SetActive(true);
        actualEndTurnBtn.gameObject.SetActive(true);
        askPanel.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack).From(0);
    }

    private void HideAskPanel()
    {
        actualEndTurnBtn.gameObject.SetActive(false);
        askPanel.transform.DOScale(0, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            askPanel.SetActive(false);
        });
    }

    private void OnUpdateCharacterDecision(object obj)
    {
        HideAskPanel();
    }

    public void ResetEndTurn()
    {
        if (image == null)
        {
            image = GetComponent<Image>();
        }
        endTurnBtn.onClick.RemoveAllListeners();
        actualEndTurnBtn.onClick.RemoveAllListeners();
        hidePanelBtn.onClick.RemoveAllListeners();
        EndTurnInactive();
        EventBus.Instance.Unsubscribe<UpdateCharacterDecision>(OnUpdateCharacterDecision);
    }

    #region Effects

    public void EndTurnActive()
    {
        if (isHide)
        {
            return;
        }
        image.color = highlightColor;
        endTurnVFX.SetActive(true);

        if (!keepActive) // Prevent keep scaling in tutorials
        {
            shouldScale = true;
            ScaleUp(endTurnVFX);
        }
    }

    private void ScaleUp(GameObject gameObject)
    {
        if (shouldScale)
        {
            gameObject.transform.DOScale(scaleUpValue, scaleUpDuration).SetEase(Ease.OutBack).OnComplete(() =>
            {
                ScaleDown(gameObject);
            });
        }
    }

    private void ScaleDown(GameObject gameObject)
    {
        if (shouldScale)
        {
            gameObject.transform.DOScale(new Vector3(1, 1, 1), scaleBackDuration).SetEase(Ease.OutBack).OnComplete(() =>
            {
                ScaleUp(gameObject);
            });
        }
    }

    public void EndTurnInactive()
    {
        image.color = Color.white;
        endTurnVFX.SetActive(false);
        shouldScale = false;
    }
    #endregion
}
