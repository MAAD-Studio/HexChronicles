using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : Singleton<MenuManager>
{
    public MenuClassifier MainMenuClassifier;
    public MenuClassifier LoadingScreenClassifier;
    public MenuClassifier HUDMenuClassifier;
    public MenuClassifier PauseMenuClassifier;
    public MenuClassifier WorldMapClassifier;
    public MenuClassifier PreGameScreenClassifier;
    public MenuClassifier VictoryScreenClassifier;
    public MenuClassifier DefeatedScreenClassifier;

    private Dictionary<Guid, Menu> menuList = new Dictionary<Guid, Menu>();

    public T GetMenu<T>(MenuClassifier menuClassifier) where T : Menu
    {
        Menu menu;
        if (menuList.TryGetValue(menuClassifier.Id, out menu))
        {
            return (T)menu;
        }
        return null;
    }

    public void AddMenu(Menu menu)
    {
        if (menuList.ContainsKey(menu.menuClassifier.Id))
        {
            Debug.Assert(false, $"{menu.name} menu is already registered using {menu.menuClassifier.name}");
            return;
        }
        menuList.Add(menu.menuClassifier.Id, menu);
    }

    public void RemoveMenu(Menu menu)
    {
        menuList.Remove(menu.menuClassifier.Id);
    }

    public void ShowMenu(MenuClassifier classifier, string options = "")
    {
        Menu menu;
        if (menuList.TryGetValue(classifier.Id, out menu))
        {
            menu.OnShowMenu(options);
        }
    }

    public void HideMenu(MenuClassifier classifier, string options = "")
    {
        Menu menu;
        if (menuList.TryGetValue(classifier.Id, out menu))
        {
            menu.OnHideMenu(options);
        }
    }
}
