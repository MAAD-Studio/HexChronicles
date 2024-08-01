using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabPages : MonoBehaviour
{
    [SerializeField] private List<GameObject> tabPages;
    [SerializeField] private TextMeshProUGUI pageText;
    private int currentPageID = 0;

    private void Start()
    {
        foreach (GameObject page in tabPages)
        {
            page.SetActive(false);
        }
        currentPageID = 0;
        tabPages[currentPageID].SetActive(true);
        pageText.text = $"Page {currentPageID + 1} / {tabPages.Count}";
    }

    public void PreviousPage()
    {
        if (currentPageID == 0)
        {
            return;
        }
        else
        {
            tabPages[currentPageID].SetActive(false);
            currentPageID--;
            tabPages[currentPageID].SetActive(true);
        }
        pageText.text = $"Page {currentPageID + 1} / {tabPages.Count}";
    }

    public void NextPage()
    {
        if (currentPageID == tabPages.Count - 1)
        {
            return;
        }
        else
        {
            tabPages[currentPageID].SetActive(false);
            currentPageID++;
            tabPages[currentPageID].SetActive(true);
        }
        pageText.text = $"Page {currentPageID + 1} / {tabPages.Count}";
    }
}
