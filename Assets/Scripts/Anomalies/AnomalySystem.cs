using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

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
    public AnomalyObject anomaly_obj;
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


    static List<AnomalyObject> objects = new List<AnomalyObject>();


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

    public static void Register(AnomalyObject o)
    {
        objects.Add(o);
    }

    public static void Unregister(AnomalyObject o)
    {
        objects.Remove(o);
    }


    float GetTimeAtNextAnomaly()
    {
        return Time.time + Mathf.Max(UnityEngine.Random.Range(15.0f, 30.0f) - intensity, 3.0f);
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

    public AnomalyObject GetRandomAnomalyObject()
    {
        if (objects == null || objects.Count == 0)
            return null;

        var candidates = objects
            .Where(o => !active.Any(a => a.anomaly_obj == o))
            .ToList();

        if (candidates.Count == 0)
            return null;

        return candidates[UnityEngine.Random.Range(0, candidates.Count)];
    }

    public Vector3 GetRandomPosAround(Vector3 pos)
    {
        float max_offset = 1.0f;
        return pos + new Vector3(
            UnityEngine.Random.Range(-max_offset, max_offset),
            0,
            UnityEngine.Random.Range(-max_offset, max_offset)
            );
    }



    public void DoAnomaly()
    {
        Anomaly a = new Anomaly();
        AnomalyType type = AnomalyType.ObjectMoved;
        //AnomalyType type = GetRandomEnum<AnomalyType>();
        a.anomaly = type;

        switch (type)
        {
            case AnomalyType.ObjectChanged:
                {
                    // TODO:
                    break;
                }
            case AnomalyType.ObjectMoved:
                {
                    AnomalyObject o = GetRandomAnomalyObject();
                    if (o == null) return;

                    a.room = o.room;
                    a.anomaly_obj = o;

                    o.transform.position = GetRandomPosAround(o.transform.position);
                    break;
                }
        }

        active.Add(a);
        Debug.Log($"Anomaly started: {a.anomaly} in {a.room}");
    }

    public void Report(AnomalyType type, RoomType room)
    {
        Debug.Log("ANOMALY REPORTED");
        selectedRoom = RoomType.None;
        selectedType = AnomalyType.None;

        var guess = new Anomaly { anomaly = type, room = room };

        int idx = active.FindIndex(a => a.Equals(guess));
        bool correct = idx >= 0;

        if (correct)
        {
            AnomalyObject o = active[idx].anomaly_obj;
            // fix anomaly
            o.transform.position = o.original_pos;
            o.transform.localScale = o.original_scale;
            active.RemoveAt(idx);
        }

        Debug.Log(correct ? "CORRECT" : "INCORRECT");
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
            DoAnomaly();
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
                        Report(selectedType, selectedRoom);
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
