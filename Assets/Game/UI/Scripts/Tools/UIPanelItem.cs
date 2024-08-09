using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

[System.Serializable]
public class UIPanelItem
{
    [Header("Fade Type")]
    public bool fadeIn = true;
    public bool scale = true;
    public bool slide = false;

    [Header("Fade Settings")]
    public float fadeInTime = 0.5f;
    public bool fadeAxisX = false;
    public bool fadeAxisY = true;
    public bool axisInvert = false;

    public RectTransform rectTransform;
    public CanvasGroup canvasGroup;
    private Vector3 originalPosition;
    private Vector3 slidePosition;

    public UIPanelItem(RectTransform rectTransform)
    {
        this.rectTransform = rectTransform;
        originalPosition = rectTransform.position;
    }

    public void Initialize()
    {
        if (fadeIn)
        {
            if (canvasGroup == null)
            {
                canvasGroup = rectTransform.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = rectTransform.gameObject.AddComponent<CanvasGroup>();
                }
            }
            canvasGroup.alpha = 0;
        }

        if (slide)
        {
            slidePosition = originalPosition;
            if (fadeAxisX)
            {
                slidePosition.x = axisInvert ? 
                    (slidePosition.x + rectTransform.sizeDelta.x) : 
                    (slidePosition.x - rectTransform.sizeDelta.x);
            }
            if (fadeAxisY)
            {
                slidePosition.y = axisInvert ? 
                    (slidePosition.y + rectTransform.sizeDelta.y) :
                    (slidePosition.y - rectTransform.sizeDelta.y);
            }
            rectTransform.position = slidePosition;
        }

        if (scale)
        {
            Vector3 scale = Vector3.one;
            if (fadeAxisX)
            {
                scale.x = 0;
            }
            if (fadeAxisY)
            {
                scale.y = 0;
            }
            rectTransform.localScale = scale;
        }
    }

    public void FadeIn()
    {
        if (fadeIn)
        {
            canvasGroup.DOFade(1, fadeInTime);
        }

        if (scale)
        {
            rectTransform.DOScale(Vector3.one, fadeInTime);
        }

        if (slide)
        {
            rectTransform.DOMove(originalPosition, fadeInTime);
        }
    }

    public void FadeOut()
    {
        if (fadeIn)
        {
            canvasGroup.DOFade(0, fadeInTime / 2);
        }

        if (scale)
        {
            Vector3 scale = Vector3.one;
            if (fadeAxisX)
            {
                scale.x = 0;
            }
            if (fadeAxisY)
            {
                scale.y = 0;
            }
            rectTransform.DOScale(scale, fadeInTime / 2);
        }

        if (slide)
        {
            rectTransform.DOMove(slidePosition, fadeInTime / 2);
        }
    }
}
