using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TabGroupButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private TabGroup tabGroup;
    [SerializeField] private TextMeshProUGUI tabText;

    [Header("Customize Text")]
    [SerializeField] private Color idleColor;
    [SerializeField] private int idleSize;
    [SerializeField] private Color hoverColor;
    [SerializeField] private int hoverSize;
    [SerializeField] private Color selectedColor;
    [SerializeField] private int selectedSize;

    private void Start()
    {
        if (tabText == null)
        {
            tabText = GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    #region PointerEventHandler
    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }
    #endregion


    #region TabStates
    public void IdleState()
    {
        tabText.color = idleColor;
        tabText.fontSize = idleSize;
    }

    public void HoverState()
    {
        tabText.color = hoverColor;
        tabText.fontSize = hoverSize;
    }

    public void SelectedState()
    {
        tabText.color = selectedColor;
        tabText.fontSize = selectedSize;
    }
    #endregion
}
