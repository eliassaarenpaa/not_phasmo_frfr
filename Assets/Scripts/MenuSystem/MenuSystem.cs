using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MenuScreen
{
    MainMenu,
    WinMenu,
    LoseMenu,
}

public class MenuSystem 
{
    public static MenuUI ui;

    public static void Register(MenuUI _ui)
    {
        ui = _ui;
    }

    public bool IsPlay()
    {
        if (ui.playButton.clicked)
        {
            ui.playButton.clicked = false;
            return true;
        }
        return false;
    }

    public void SetScreen(MenuScreen screen)
    {
        ui.menuScreen.SetActive(screen == MenuScreen.MainMenu);
        ui.winScreen.SetActive(screen == MenuScreen.WinMenu);
        ui.loseScreen.SetActive(screen == MenuScreen.LoseMenu);
    }

}
