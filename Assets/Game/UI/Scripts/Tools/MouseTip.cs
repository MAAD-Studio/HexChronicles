using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class MouseTip : Singleton<MouseTip>
{
    [SerializeField] private Vector3 offset = new Vector3(0, 0, 0);
    [SerializeField] private GameObject tipObject;
    [SerializeField] private TextMeshProUGUI tipText;
    private RectTransform rectTransform;

    private bool isShown = false;
    private Vector3 initialCameraPosition;

    private void Start()
    {
        tipObject.SetActive(false);
        rectTransform = tipObject.GetComponent<RectTransform>();
    }

    private void Update()
    {
        // Hide tip if camera moves
        if (isShown && Vector3.Distance(Camera.main.transform.position, initialCameraPosition) > 1.0f)
        {
            HideTip(); 
        }
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

        // Prevent from going off screen
        float pivotX = tipObject.transform.position.x / Screen.width;
        float pivotY = tipObject.transform.position.y / Screen.height;
        rectTransform.pivot = new Vector2(pivotX, pivotY);

        tipObject.SetActive(true);
        tipText.text = text.ToString();

        initialCameraPosition = Camera.main.transform.position;
        isShown = true;

        Invoke("HideTip", 2);
    }

    public void HideTip()
    {
        tipObject.SetActive(false);
        isShown = false;
    }
}
