using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UITip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform rectTransform; // TipObject
    [SerializeField] protected bool followMouse = true;

    [Header("Advanced Settings")]
    [SerializeField] protected bool advancedPanel = false;
    [SerializeField] protected bool fadeIn = true;
    [SerializeField] protected bool scale = true;
    [SerializeField] protected bool slide = false;
    [SerializeField] protected float fadeInTime = 0.5f;
    [SerializeField] protected bool fadeAxisX = false;
    [SerializeField] protected bool fadeAxisY = true;
    [SerializeField] protected bool axisInvert = false;

    protected UIPanelItem uiPanelItem;
    protected CanvasGroup canvasGroup;
    protected float fadeTime = 0.2f;
    protected RectTransform thisRect;

    protected bool isLoaded = false;

    public virtual void Initialize()
    {
        Debug.Assert(rectTransform != null, $"{name} couldn't find RectTransform");
        canvasGroup = rectTransform.GetComponent<CanvasGroup>();
        Debug.Assert(canvasGroup != null, $"{name} couldn't find CanvasGroup");

        rectTransform.gameObject.SetActive(false);
        thisRect = GetComponent<RectTransform>();

        if (advancedPanel)
        {
            uiPanelItem = new UIPanelItem(rectTransform);
            uiPanelItem.canvasGroup = canvasGroup;
            uiPanelItem.fadeIn = fadeIn;
            uiPanelItem.scale = scale;
            uiPanelItem.slide = slide;
            uiPanelItem.fadeInTime = fadeInTime;
            uiPanelItem.fadeAxisX = fadeAxisX;
            uiPanelItem.fadeAxisY = fadeAxisY;
            uiPanelItem.axisInvert = axisInvert;
            uiPanelItem.Initialize();
        }

        isLoaded = true;
    }

    protected virtual void Start()
    {
        if (!isLoaded)
        {
            Initialize();
        }
    }

    protected void Update()
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

    public virtual void ShowTooltip()
    {
        if (followMouse)
        {
            rectTransform.position = Input.mousePosition;
        }
        rectTransform.gameObject.SetActive(true);

        if (advancedPanel)
        {
            uiPanelItem.FadeIn();
        }
        else
        {
            canvasGroup.alpha = 1;
            rectTransform.localScale = Vector3.zero;
            rectTransform.DOScale(Vector3.one, fadeTime);
        }
    }

    protected void UpdateTooltipPosition(Vector2 position)
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

    protected virtual void HideTooltip()
    {
        if (advancedPanel)
        {
            uiPanelItem.FadeOut();
        }
        else
        {
            canvasGroup.DOFade(0, fadeTime / 2).OnComplete(() => 
            { 
                rectTransform.gameObject.SetActive(false);
            });
        }
    }

    protected bool IsMouseOverThis()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(thisRect, Input.mousePosition);
    }
}
