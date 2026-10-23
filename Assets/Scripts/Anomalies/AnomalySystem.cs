using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public enum AnomalyType
{
    None,
    ObjectMoved,
    ObjectChanged,
    Electrical,
}

public enum RoomType
{
    None,
    Room1,
    Room2,
    Room3,
}

public struct Anomaly : IEquatable<Anomaly>
{
    public AnomalyType anomaly;
    public RoomType room;
    public bool Equals(Anomaly o) => anomaly == o.anomaly && room == o.room;
    public override bool Equals(object o) => o is Anomaly a && Equals(a);
    public override int GetHashCode() => ((int)anomaly * 397) ^ (int)room;
}

public class AnomalySystem
{
    bool is_active = false;
    float timer;
    float time_at_next_anomaly;
    float intensity;
    List<Anomaly> active = new List<Anomaly>();

    static AnomalyUI ui = null;
    static AnomalyReportTrigger trigger = null;
    RoomType selectedRoom = RoomType.None;
    AnomalyType selectedType = AnomalyType.None;

    public static bool IsReporting()
    {
        return ui && ui.gameObject.activeSelf && trigger && trigger.isPlayerOnTrigger;
    }

    public static void Register(AnomalyUI _ui)
    {
        ui = _ui;
    }

    public static void Register(AnomalyReportTrigger t)
    {
        trigger = t;
    }

    float GetTimeAtNextAnomaly()
    {
        return Time.time + Mathf.Max(UnityEngine.Random.Range(3.0f, 10.0f) - intensity, 3.0f);
    }

    public void BeginAnomalies()
    {
        timer = Time.time;
        is_active = true;
        time_at_next_anomaly = GetTimeAtNextAnomaly();
    }

    public void EndAnomalies()
    {
        is_active = false;
        active.Clear();
    }

    private static T GetRandomEnum<T>() where T : Enum
    {
        Array values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(UnityEngine.Random.Range(1, values.Length));
    }

    public Anomaly GetRandomAnomaly()
    {
        Anomaly a;
        a.anomaly = GetRandomEnum<AnomalyType>();
        a.room = GetRandomEnum<RoomType>();
        return a;
    }

    public void DoAnomaly(Anomaly a)
    {
        active.Add(a);
        Debug.Log($"Anomaly started: {a.anomaly} in {a.room}");
    }

    public bool Report(Anomaly a)
    {
        Debug.Log("ANOMALY REPORTED");

        bool correct;
        if (active.Contains(a))
        {
            Debug.Log("CORRECT");
            correct = true;
        }
        else
        {
            Debug.Log("INCORRECT");
            correct = false;
        }

        selectedRoom = RoomType.None;
        selectedType = AnomalyType.None;

        return correct;
    }

    static void SetButtonColor(Button btn, Color c)
    {
        ColorBlock cb = btn.colors;
        cb.normalColor = c;
        cb.highlightedColor = c;
        cb.selectedColor = c;
        btn.colors = cb;
    }

    public void Update(float dt)
    {
        if (!is_active)
            return;

        timer += dt;
        intensity += 0.1f * dt;

        if (Time.time >= time_at_next_anomaly)
        {
            DoAnomaly(GetRandomAnomaly());
            time_at_next_anomaly = GetTimeAtNextAnomaly();
        }


        if (trigger.isPlayerOnTrigger)
        {
            if (ui)
            {
                foreach (AnomalyRoomButton b in ui.roomButtons)
                {
                    if (b.clicked)
                    {
                        selectedRoom = b.room;
                        b.clicked = false;
                    }

                    SetButtonColor(b.button, selectedRoom == b.room ? Color.red : Color.white);
                }

                foreach (AnomalyTypeButton b in ui.typeButtons)
                {
                    if (b.clicked)
                    {
                        selectedType = b.type;
                        b.clicked = false;
                    }

                    SetButtonColor(b.button, selectedType == b.type ? Color.red : Color.white);
                }

                if (ui.reportButton.clicked)
                {
                    if (selectedRoom != RoomType.None && selectedType != AnomalyType.None)
                    {
                        var a = new Anomaly
                        {
                            anomaly = selectedType,
                            room = selectedRoom,
                        };

                        if (Report(a))
                        {
                            // TODO: Remove the anomaly
                            active.Remove(a);
                        }
                    }


                    ui.reportButton.clicked = false;
                }
            }
        }
        else
        {
            selectedRoom = RoomType.None;
            selectedType = AnomalyType.None;
        }

        ui.gameObject.SetActive(trigger.isPlayerOnTrigger);

    }



}
