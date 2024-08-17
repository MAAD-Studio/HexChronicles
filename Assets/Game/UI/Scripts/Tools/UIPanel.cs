using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup), (typeof(RectTransform)))]
public class UIPanel : MonoBehaviour
{
    [SerializeField] private float fadeInTime = 0.5f;
    [SerializeField] private float fadeOutTime = 0.5f;
    [SerializeField] private Ease easeType;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    [Header("Settings")]
    [SerializeField] private Transform startPositon;
    [SerializeField] private bool fadeAxisX = false;
    [SerializeField] private bool fadeAxisY = true;

    [Header("Scale Items If Any")]
    [SerializeField] private UIPanelItem[] childItems;

    public void Initialize()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }

    public void FadeIn()
    {
        if (canvasGroup == null || rectTransform == null)
        {
            Initialize();
        }

        // Start Values:
        gameObject.SetActive(true);
        canvasGroup.alpha = 0;
        rectTransform.position = startPositon.position;

        if (fadeAxisX && fadeAxisY)
        {
            rectTransform.localScale = Vector3.zero;
        }
        else if (fadeAxisX)
        {
            rectTransform.localScale = new Vector3(0, 1, 1);
        }
        else if (fadeAxisY)
        {
            rectTransform.localScale = new Vector3(1, 0, 1);
        }

        if (childItems != null && childItems.Length > 0)
        {
            foreach (UIPanelItem item in childItems)
            {
                item.Initialize();
            }
        }

        // Tweening:
        rectTransform.DOAnchorPos(Vector3.zero, fadeInTime / 2, false).SetEase(easeType).OnComplete(() =>
        {
            StartCoroutine(FadeInItems());
        });
        canvasGroup.DOFade(1, fadeInTime);
        rectTransform.DOScale(Vector3.one, fadeInTime);
    }

    public void FadeOut()
    {
        rectTransform.DOAnchorPos(startPositon.position, fadeOutTime / 2).SetEase(easeType);
        canvasGroup.DOFade(0, fadeOutTime).OnComplete(() => gameObject.SetActive(false));
        rectTransform.DOScale(Vector3.zero, fadeOutTime);
    }

    IEnumerator FadeInItems()
    {
        if (childItems == null || childItems.Length == 0)
        {
            yield break;
        }
        foreach (UIPanelItem item in childItems)
        {
            item.FadeIn();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
