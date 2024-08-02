using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectTutorialCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private SceneReference tutorialScene;
    private SelectTutorial selectTutorial;
    private bool selected = false;

    public void SetTutorial(SelectTutorial selectTutorial, SceneReference tutorialScene)
    {
        this.selectTutorial = selectTutorial;
        this.tutorialScene = tutorialScene;
    }

    #region PointerEventHandler
    public void OnPointerEnter(PointerEventData eventData)
    {
        HoverState();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!selected)
        {
            IdleState();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        selectTutorial.ResetTutorialCards();
        selectTutorial.OnTutorialSelected(tutorialScene);
        SelectedState();
        selected = true;
    }
    #endregion

    #region States
    public void IdleState()
    {
        selected = false;
        gameObject.GetComponent<Image>().color = new Color(0.7f, 0.7f, 0.7f);
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    public void HoverState()
    {
        gameObject.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
    }

    public void SelectedState()
    {
        gameObject.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
        transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
    }
    #endregion
}
