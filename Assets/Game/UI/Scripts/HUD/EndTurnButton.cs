using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class EndTurnButton : MonoBehaviour
{
    public Button endTurnBtn;
    [SerializeField] private GameObject endTurnVFX;
    [SerializeField] private Vector3 scaleUpValue = new Vector3(1.2f, 1.5f, 1.2f);
    [SerializeField] private float scaleUpDuration = 0.6f;
    [SerializeField] private float scaleBackDuration = 0.3f;

    private Image image;
    private bool shouldScale = false;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    public void AddListener(Action action)
    {
        endTurnBtn.onClick.AddListener(() => action());
    }

    public void EnableButton()
    {
        endTurnBtn.interactable = true;
    }

    public void DisableButton()
    {
        endTurnBtn.interactable = false;
    }

    public void Reset()
    {
        if (image == null)
        {
            image = GetComponent<Image>();
        }
        endTurnBtn.onClick.RemoveAllListeners();
        EndTurnInactive();
    }

    
    #region Effects

    public void EndTurnActive()
    {
        image.color = new Color(1, 0.88f, 0, 1);
        endTurnVFX.SetActive(true);
        shouldScale = true;
        ScaleUp(endTurnVFX);
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
