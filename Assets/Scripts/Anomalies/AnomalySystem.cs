using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public enum AnomalyEffectType
{
    None,
    PrefabToggle,
}

public enum AnomalyScreenType
{
    None,
    SelectRoomScreen,
    SelectTypeScreen,
    ReportScreen,
}

public enum AnomalyType
{
    None,
    ObjectChanged,
    ObjectMoved,
    ObjectRemoved,
    UnknownEntity,
    ElectricalDisturbance,
}

public enum RoomType
{
    None,
    LivingRoom,
    Cellar,
    Kitchen,
    MainHall,
    Bathroom,
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
    bool gotResult = false;
    public bool isReporting = false;
    public bool isWaitingForResult = false;
    float reportTime;
    bool isActive = false;
    float timer;
    float timeAtNextAnomaly;
    float intensity;
    List<Anomaly> active = new List<Anomaly>();


    static List<AnomalyObject> objects = new List<AnomalyObject>();


    public static AnomalyUI ui = null;
    public static AnomalyReportTrigger trigger = null;
    RoomType selectedRoom = RoomType.None;
    AnomalyType selectedType = AnomalyType.None;

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
        return Time.time + Mathf.Max(UnityEngine.Random.Range(3.0f, 5.0f) - intensity, 3.0f);
    }

    public void BeginAnomalies()
    {
        timer = Time.time;
        isActive = true;
        timeAtNextAnomaly = GetTimeAtNextAnomaly();
    }

    public void EndAnomalies()
    {
        isActive = false;
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

                    switch (o.effect)
                    {
                        case AnomalyEffectType.PrefabToggle:
                            {
                                o.original_ver.gameObject.SetActive(false);
                                o.anomaly_ver.gameObject.SetActive(true);
                                break;
                            }
                    }

                    //o.transform.position = GetRandomPosAround(o.transform.position);
                    break;
                }
        }

        active.Add(a);
        Debug.Log($"Anomaly started: {a.anomaly} in {a.room}");
    }

    public bool Report(AnomalyType type, RoomType room)
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

            switch (o.effect)
            {
                case AnomalyEffectType.PrefabToggle:
                    {
                        o.original_ver.gameObject.SetActive(true);
                        o.anomaly_ver.gameObject.SetActive(false);
                        break;
                    }
            }

            active.RemoveAt(idx);
        }

        Debug.Log(correct ? "CORRECT" : "INCORRECT");
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

    static void SetActiveScreen(AnomalyScreenType type)
    {
        ui.roomSelectScreen.SetActive(type == AnomalyScreenType.SelectRoomScreen);
        ui.typeSelectScreen.SetActive(type == AnomalyScreenType.SelectTypeScreen);
        ui.reportScreen.SetActive(type == AnomalyScreenType.ReportScreen);
    }

    public void Update(PlayerInput input, float dt)
    {
        if (!isActive || !ui)
            return;

        timer += dt;
        intensity += 0.1f * dt;

        if (Time.time >= timeAtNextAnomaly)
        {
            DoAnomaly();
            timeAtNextAnomaly = GetTimeAtNextAnomaly();
        }

        if (trigger.isPlayerOnTrigger)
        {
            if (!isReporting)
            {
                if (input.isInteract)
                {
                    isReporting = true;
                }
            }
        }

        if (isReporting)
        {
            if (!isWaitingForResult)
            {
                if (input.isBack)
                {
                    isReporting = false;
                }
            }

            foreach (AnomalyRoomButton b in ui.roomButtons)
            {
                b.selectedTickbox.SetActive(b.room == selectedRoom);

                if (b.clicked)
                {
                    selectedRoom = b.room;
                    b.clicked = false;
                }
            }

            foreach (AnomalyTypeButton b in ui.typeButtons)
            {
                b.selectedTickbox.SetActive(b.type == selectedType);

                if (b.clicked)
                {
                    selectedType = b.type;
                    b.clicked = false;
                }
            }

            foreach (AnomalyNextButton b in ui.nextButtons)
            {
                if (b.clicked)
                {
                    if (ui.roomSelectScreen.activeSelf && selectedRoom != RoomType.None
                        || ui.typeSelectScreen.activeSelf && selectedType != AnomalyType.None)
                    {
                        SetActiveScreen(b.nextScreen);
                    }

                    b.clicked = false;
                }
            }

            if (ui.reportButton.clicked)
            {
                if (selectedRoom != RoomType.None && selectedType != AnomalyType.None && !isWaitingForResult)
                {
                    isWaitingForResult = true;
                    reportTime = Time.time;
                }

                ui.reportButton.clicked = false;
            }
        }

        if (isWaitingForResult)
        {
            float reportDuration = 5.0f;
            float resetDuration = 3.0f;
            float elapsed = Time.time - reportTime;
            float remaining = Mathf.Max(0f, reportDuration - elapsed);

            if (!gotResult)
            {
                if (Time.time >= reportTime + reportDuration)
                {
                    if (Report(selectedType, selectedRoom))
                    {
                        ui.reportButton.tmpro.text = "[ANOMALY REMOVED]";
                    }
                    else
                    {
                        ui.reportButton.tmpro.text = "[ANOMALY NOT FOUND]";
                    }

                    gotResult = true;
                }
                else
                {
                    ui.reportButton.tmpro.text = $"REPORTING... {Mathf.CeilToInt(remaining)}";
                }
            }

            if (Time.time >= reportTime + reportDuration + resetDuration)
            {
                ui.reportButton.tmpro.text = "< REPORT >";
                SetActiveScreen(AnomalyScreenType.SelectRoomScreen);
                isWaitingForResult = false;
                gotResult = false;
            }
        }
    }

}
