using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreGameScreen : Menu
{
    [SerializeField] private Button startButton;
    public GameObject worldMap; // testing

    protected override void Start()
    {
        base.Start();
        startButton.onClick.AddListener(OnStartGame);
    }

    // Only For Testing
    private void OnStartGame()
    {
        WorldMap map = worldMap.GetComponent<WorldMap>();
        map?.OnLoadLevel(map.levels[0]);
    }
}
