using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    [SerializeField] private Button closeBtn;
    [SerializeField] private List<Tab> tabButtons;
    [SerializeField] private List<GameObject> tabPages;
    private Tab selectedTab;
    private Tab defaultTab;

    private void Start()
    {
        closeBtn.onClick.AddListener(() => gameObject.SetActive(false));
        defaultTab = tabButtons[0];
        OnTabSelected(defaultTab);
        gameObject.SetActive(false);
    }

    public void OnTabEnter(Tab tab)
    {
        ResetTabs();
        if (selectedTab == null || tab != selectedTab) 
        {
            tab.HoverState();
        }
    }

    public void OnTabExit(Tab tab)
    {
        ResetTabs();
    }

    public void OnTabSelected(Tab tab)
    {
        selectedTab = tab;
        ResetTabs();
        tab.SelectedState();

        int index = tabButtons.IndexOf(tab);
        for (int i = 0; i < tabButtons.Count; i++)
        {
            if (i == index)
            {
                tabPages[i].SetActive(true);
            }
            else
            {
                tabPages[i].SetActive(false);
            }
        }
    }

    public void ResetTabs()
    {
        foreach (Tab tab in tabButtons)
        {
            if (selectedTab != null && tab == selectedTab) { continue; }
            tab.IdleState();
        }
    }
}
