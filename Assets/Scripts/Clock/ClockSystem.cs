using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockSystem
{
    public static ClockUI ui;
    public const float totalGameDuration = 60.0f; // 15 min == 900
    public const int startHour = 0;                // 12 AM
    public const int endHour = 6;                  // 6 AM
    public float timer;

    int lastDisplayedMinutes = -1;

    public void StartTimer()
    {
        timer = 0;
        lastDisplayedMinutes = -1;
    }

    public void UpdateTimer(float dt)
    {
        timer += dt;
    }

    public void UpdateUI()
    {
        float progress = Mathf.Clamp01(timer / totalGameDuration);
        int totalMinutes = Mathf.FloorToInt(progress * (endHour - startHour) * 60f);
        totalMinutes += startHour * 60;

        if (totalMinutes == lastDisplayedMinutes) return;
        lastDisplayedMinutes = totalMinutes;

        int hour = totalMinutes / 60;
        int minute = totalMinutes % 60;
        int displayHour = hour == 0 ? 12 : hour; // 0 shows as 12 AM

        ui.tmpro.text = string.Format("{0}:{1:00} AM", displayHour, minute);
    }

    public static void Register(ClockUI _ui)
    {
        ui = _ui;
    }
}