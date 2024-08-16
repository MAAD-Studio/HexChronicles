using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Button))]
public class ButtonChangeNotifier : MonoBehaviour
{
    private Button button;

    [HideInInspector] public UnityEvent onButtonChange = new UnityEvent();

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public Color GetCurrentButtonColor()
    {
        ColorBlock colors = button.colors;
        switch (button.transition)
        {
            case Selectable.Transition.ColorTint:
                if (!button.interactable)
                    return colors.disabledColor;
                else
                    return colors.normalColor;
            default:
                return colors.normalColor;
        }
    }
}
