using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UITip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject tipObject;

    void OnEnable()
    {
        HideTooltip();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowTooltip();
        UpdateTooltipPosition(Input.mousePosition);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideTooltip();
    }

    public void ShowTooltip()
    {
        tipObject.SetActive(true);
    }

    public void HideTooltip()
    {
        tipObject.SetActive(false);
    }

    public void UpdateTooltipPosition(Vector2 position)
    {
        tipObject.transform.position = position;
    }
}
