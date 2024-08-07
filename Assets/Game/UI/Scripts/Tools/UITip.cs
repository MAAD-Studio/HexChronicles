using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UITip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform rectTransform;
    [SerializeField] private bool followMouse = true;
    [SerializeField] private bool scaleUp = true;
    [SerializeField] private bool fadeIn = true;

    private CanvasGroup canvasGroup;
    private float fadeTime = 0.2f;
    private RectTransform thisRect;

    void OnEnable()
    {
        Debug.Assert(rectTransform != null, $"{name} couldn't find RectTransform");
        canvasGroup = rectTransform.GetComponent<CanvasGroup>();
        Debug.Assert(canvasGroup != null, $"{name} couldn't find CanvasGroup");

        rectTransform.gameObject.SetActive(false);
        thisRect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (followMouse && rectTransform.gameObject.activeSelf)
        {
            UpdateTooltipPosition(Input.mousePosition);
        }
    }

    // Showing:
    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowTooltip();
    }

    public void ShowTooltip()
    {
        if (followMouse)
        {
            rectTransform.position = Input.mousePosition;
        }
        rectTransform.gameObject.SetActive(true);

        if (scaleUp)
        {
            if (!fadeIn)
            {
                canvasGroup.alpha = 1;
            }
            rectTransform.localScale = Vector3.zero;
            rectTransform.DOScale(Vector3.one, fadeTime);
        }
        
        if (fadeIn)
        {
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1, fadeTime);
        }
    }

    private void UpdateTooltipPosition(Vector2 position)
    {
        rectTransform.DOMove(position, 0.1f);
    }

    // Hiding:
    public void OnPointerExit(PointerEventData eventData)
    {
        if (IsMouseOverThis())
        {
            return; // Block the tooltip from hiding when the mouse is over the tooltip
        }

        HideTooltip();
    }

    private void HideTooltip()
    {
        canvasGroup.DOFade(0, fadeTime).OnComplete(() => rectTransform.gameObject.SetActive(false));
    }

    private bool IsMouseOverThis()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(thisRect, Input.mousePosition);
    }
}
