using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using static AnomalyObject;
using Debug = UnityEngine.Debug;

enum ThreatLevel { NONE, LOW, HIGH, EXTREME }
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
    public AnomalyEffect effect;
    public AnomalyType anomaly;
    public RoomType room;
    public bool Equals(Anomaly o) => anomaly == o.anomaly && room == o.room;
    public override bool Equals(object o) => o is Anomaly a && Equals(a);
    public override int GetHashCode() => ((int)anomaly * 397) ^ (int)room;
}

public class AnomalySystem
{
    ThreatLevel threat;
    public const int maxUnreportedAnomalies = 10;
    public const float gracePeriodDuration = 1.0f;

    bool gotResult = false;
    public bool isReporting = false;
    public bool isWaitingForResult = false;
    bool isGracePeriodEnded = false;
    float reportTime;
    float timer;
    float timeAtNextAnomaly;
    float intensity;
    public List<Anomaly> active = new List<Anomaly>();

    static List<AnomalyObject> objects = new List<AnomalyObject>();

    public static AnomalyUI ui = null;
    public static AnomalyGracePeriodLights lights = null;
    public static AnomalyReportTrigger trigger = null;
    RoomType selectedRoom = RoomType.None;
    AnomalyType selectedType = AnomalyType.None;

    public static List<AnomalyTypeWeight> typeWeights = new List<AnomalyTypeWeight>
{
    new AnomalyTypeWeight { type = AnomalyType.ObjectChanged,         weight = 30f },
    new AnomalyTypeWeight { type = AnomalyType.ObjectMoved,           weight = 30f },
    new AnomalyTypeWeight { type = AnomalyType.ObjectRemoved,         weight = 25f },
    new AnomalyTypeWeight { type = AnomalyType.UnknownEntity,         weight = 10f },
    new AnomalyTypeWeight { type = AnomalyType.ElectricalDisturbance, weight = 5f  },
};




    public static float GetTypeWeight(AnomalyType t)
    {
        for (int i = 0; i < typeWeights.Count; i++)
            if (typeWeights[i].type == t) return typeWeights[i].weight;
        return 0f;
    }

    public static Gadget gadget;
    public static void Register(Gadget g)
    {
        gadget = g;
    }


    public static void Register(AnomalyUI _ui)
    {
        ui = _ui;
    }

    public static void Register(AnomalyGracePeriodLights _lights)
    {
        lights = _lights;
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
        return Time.time + Mathf.Lerp(UnityEngine.Random.Range(5.0f, 5.0f), UnityEngine.Random.Range(5.0f, 5.0f), intensity);

        //return Time.time + Mathf.Lerp(UnityEngine.Random.Range(60.0f, 120.0f), UnityEngine.Random.Range(15.0f, 30.0f), intensity);
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
        AnomalyObject o = GetRandomAnomalyObject();
        if (o == null) { Debug.LogWarning("No anomaly object available."); return; }

        AnomalyEffect e = o.GetRandomEffect();

        if (!e.Apply(o))
        {
            Debug.LogWarning($"Effect {e.type}/{e.effect} could not be applied.");
            return;
        }

        var a = new Anomaly
        {
            anomaly_obj = o,
            room = o.room,
            anomaly = e.type,
            effect = e
        };

        active.Add(a);
        Debug.Log($"Anomaly started: {a.anomaly} in {a.room}");

        gadget.beepSource.Play();
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
            Anomaly a = active[idx];
            AnomalyObject o = a.anomaly_obj;

            a.effect.Revert(o);

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

    public void UpdateReportUI(PlayerInput input)
    {
        if (!ui) return;

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


    public void UpdateAnomalyEventLoop(PlayerInput input, float clockTimer, float totalGameDuration, float dt)
    {
        timer += dt;
        if (timer >= gracePeriodDuration)
        {
            if (!isGracePeriodEnded)
            {
                Debug.Log("GRACE PERIOD HAS ENDED");

                lights.gameObject.SetActive(false);

                timeAtNextAnomaly = GetTimeAtNextAnomaly();
                isGracePeriodEnded = true;
            }

            if (Time.time >= timeAtNextAnomaly)
            {
                DoAnomaly();
                timeAtNextAnomaly = GetTimeAtNextAnomaly();
            }

            intensity = Mathf.Clamp01(intensity + dt / totalGameDuration);
            //Debug.Log(intensity);


            if (active.Count > 0)
            {
                float ratio = (float)maxUnreportedAnomalies / active.Count;
                Debug.Log(active.Count);

                ThreatLevel newLevel =
       ratio >= 3f / 4f ? ThreatLevel.EXTREME :
       ratio >= 1f / 2f ? ThreatLevel.HIGH :
       ratio > 0f ? ThreatLevel.LOW :
                          ThreatLevel.NONE;


                if (newLevel != threat)
                {
                    threat = newLevel;
                    Debug.Log($"THREAT LEVEL {threat.ToString().ToUpper()}");
                }
            }
        }

        gadget.treathValue.text = threat.ToString();
    }

    public void Reset()
    {
        // Undo any anomalies that are still applied to the world
        foreach (Anomaly a in active)
        {
            if (a.anomaly_obj != null)
                a.effect.Revert(a.anomaly_obj);
        }
        active.Clear();

        // Event loop state
        timer = 0f;
        timeAtNextAnomaly = 0f;
        intensity = 0f;
        isGracePeriodEnded = false;
        threat = ThreatLevel.NONE;
        if(lights) lights.gameObject.SetActive(true);
        if (gadget) gadget.treathValue.text = ThreatLevel.NONE.ToString();

        // Report flow state
        isReporting = false;
        isWaitingForResult = false;
        gotResult = false;
        reportTime = 0f;
        selectedRoom = RoomType.None;
        selectedType = AnomalyType.None;

        // Put the UI back on its initial screen
        if (ui)
        {
            SetActiveScreen(AnomalyScreenType.SelectRoomScreen);
            ui.reportButton.tmpro.text = "< REPORT >";
            ui.reportButton.clicked = false;

            foreach (var b in ui.roomButtons) b.clicked = false;
            foreach (var b in ui.typeButtons) b.clicked = false;
            foreach (var b in ui.nextButtons) b.clicked = false;
        }
    }



}
