using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CreditsScreen : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject[] pages;

    private void Start()
    {
        gameObject.SetActive(false);
        foreach (GameObject page in pages)
        {
            page.SetActive(false);
        }
    }

    public void ShowCredits()
    {
        gameObject.SetActive(true);
        pages[0].SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ShowNextPage();
    }

    private void ShowNextPage()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i].activeSelf)
            {
                pages[i].SetActive(false);
                if (i + 1 < pages.Length)
                {
                    pages[i + 1].SetActive(true);
                }
                else
                {
                    gameObject.SetActive(false);
                }
                break;
            }
        }
    }
}
