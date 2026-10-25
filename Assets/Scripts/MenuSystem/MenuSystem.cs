using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

}
