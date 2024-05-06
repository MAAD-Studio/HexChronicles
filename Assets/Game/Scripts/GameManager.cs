using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    
    [SerializeField] private GameObject pauseUI;
    public bool isRunning;

    void Start()
    {
        isRunning = true;
        pauseUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isRunning)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }

    private void PauseGame()
    {
        //Time.timeScale = 0;
        isRunning = false;
        pauseUI.SetActive(true);
    }

    public void ResumeGame()
    {
        //Time.timeScale = 1;
        isRunning = true;
        pauseUI.SetActive(false);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
