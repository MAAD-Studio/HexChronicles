using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    [SerializeField] private Button closeBtn;
    [SerializeField] private List<TabGroupButton> tabButtons;
    [SerializeField] private List<TabPages> tabPages;
    private TabGroupButton selectedTab;
    private TabGroupButton defaultTab;
    private UIPanel uiPanel;

    [SerializeField] private Button previousBtn;
    [SerializeField] private Button nextBtn;
    private TabPages currentPages;

    private void Start()
    {
        uiPanel = GetComponent<UIPanel>();
        closeBtn.onClick.AddListener(() => uiPanel.FadeOut());
        defaultTab = tabButtons[0];
        OnTabSelected(defaultTab);
    }

    public void OnTabEnter(TabGroupButton tab)
    {
        ResetTabs();
        if (selectedTab == null || tab != selectedTab) 
        {
            tab.HoverState();
        }
    }

    public void OnTabExit(TabGroupButton tab)
    {
        ResetTabs();
    }

    public void OnTabSelected(TabGroupButton tab)
    {
        selectedTab = tab;
        ResetTabs();
        tab.SelectedState();

        int index = tabButtons.IndexOf(tab);
        for (int i = 0; i < tabButtons.Count; i++)
        {
            if (i == index)
            {
                tabPages[i].gameObject.SetActive(true);
            }
            else
            {
                tabPages[i].gameObject.SetActive(false);
            }
        }

        currentPages = tabPages[index];
        previousBtn.onClick.RemoveAllListeners();
        nextBtn.onClick.RemoveAllListeners();
        previousBtn.onClick.AddListener(() => currentPages.PreviousPage());
        nextBtn.onClick.AddListener(() => currentPages.NextPage());
    }

    public void ResetTabs()
    {
        foreach (TabGroupButton tab in tabButtons)
        {
            if (selectedTab != null && tab == selectedTab) { continue; }
            tab.IdleState();
        }
    }
}
