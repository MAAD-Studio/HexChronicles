using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MouseTip : Singleton<MouseTip>
{
    [SerializeField] private Vector3 offset = new Vector3(0, 0, 0);
    [SerializeField] private GameObject tipObject;
    [SerializeField] private TextMeshProUGUI tipText;

    private void Start()
    {
        tipObject.SetActive(false);
    }

    public void ShowTip(Vector3 position, string text, bool screenSpace)
    {
        if (screenSpace)
        {
            tipObject.transform.position = position + offset;
        }
        else
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(position);
            tipObject.transform.position = screenPos + offset;
        }

        tipObject.SetActive(true);
        tipText.text = text.ToString();

        Invoke("HideTip", 2);
    }

    public void HideTip()
    {
        tipObject.SetActive(false);
    }
}
