using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(RectTransform))]
public class Menu : MonoBehaviour
{
    public MenuClassifier menuClassifier;

    public enum StartingMenuState
    {
        Ignore,
        Active,
        Disable
    }
    public StartingMenuState startingMenuState = StartingMenuState.Active;
    public bool resetPosition = true;
    public UnityEvent OnRefreshMenu = new UnityEvent();

    private Canvas canvas;
    private RectTransform rectTransform;
    private Animator animator;

    public bool IsOpen 
    {
        get
        {
            return isOpen;
        }
        set
        {
            isOpen = value;

            canvas.gameObject.SetActive(isOpen);

            if (isOpen == true)
            {
                OnRefreshMenu.Invoke();
            }    
        }
    }
    private bool isOpen = false;

    public virtual void OnShowMenu(string options = "")
    {
        IsOpen = true;
    }

    public virtual void OnHideMenu(string options = "")
    {
        IsOpen = false;
    }

    protected virtual void Awake()
    {
        if (resetPosition == true)
        {
            rectTransform = GetComponent<RectTransform>();
            rectTransform.localPosition = Vector3.zero;
        }

        MenuManager.Instance.AddMenu(this);
    }

    protected virtual void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.CrossFade(Animator.StringToHash("Idle"), 0.0f);
        }

        switch(startingMenuState)
        {
            case StartingMenuState.Active:
                isOpen = true;
                canvas.gameObject.SetActive(true);
                break;

            case StartingMenuState.Disable:
                isOpen = false;
                canvas.gameObject.SetActive(false);
                break;
        }
    }
}
