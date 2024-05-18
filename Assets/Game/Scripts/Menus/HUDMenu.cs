using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.TextCore.Text;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class HUDMenu : Menu
{
    public MenuClassifier pauseMenuClassifier;

    private Menu pauseMenu;


    protected override void Start()
    {
        base.Start();
        pauseMenu = MenuManager.Instance.GetMenu<Menu>(pauseMenuClassifier);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            OnPauseGame();

            // TODO: Disable tile selection

        }
    }

    public void OnPauseGame()
    {
        if (pauseMenu.gameObject.activeInHierarchy == false)
        {
            Time.timeScale = 0.0f;
            MenuManager.Instance.ShowMenu(pauseMenuClassifier);
        }
    }
}
