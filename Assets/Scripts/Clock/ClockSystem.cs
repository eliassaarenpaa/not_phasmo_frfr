using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockSystem
{
    public static ClockUI ui;
    public const float totalGameDuration = 900.0f; // 15 min
    public float timer;

    public void StartTimer()
    {
        timer = totalGameDuration;
    }
    public void UpdateTimer(float dt)
    {
        timer -= dt;
    }

    int lastDisplayedSeconds = -1;
    public void UpdateUI()
    {
        int totalSeconds = Mathf.CeilToInt(Mathf.Max(timer, 0f));
        if (totalSeconds == lastDisplayedSeconds) return;
        lastDisplayedSeconds = totalSeconds;

        ui.tmpro.text = string.Format("{0:00}:{1:00}", totalSeconds / 60, totalSeconds % 60);
    }


    public static void Register(ClockUI _ui)
    {
        ui = _ui;
    }

}
