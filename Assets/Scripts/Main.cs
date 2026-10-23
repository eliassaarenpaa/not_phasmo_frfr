using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
using Debug = UnityEngine.Debug;
public struct FrameUpdateSystem { }
public struct PhysicsUpdateSystem { }

public static class Main
{
    static bool initialized;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Boot()
    {
        // If domain reload is disabled, a previous run may not have shut down cleanly
        if (initialized) Shut();
        initialized = true;

        PlayerLoopSystem loop = PlayerLoop.GetCurrentPlayerLoop();
        RemoveSystem<FrameUpdateSystem>(ref loop);
        RemoveSystem<PhysicsUpdateSystem>(ref loop);

        InsertAfter<Update.ScriptRunBehaviourUpdate>(ref loop,
            new PlayerLoopSystem { type = typeof(FrameUpdateSystem), updateDelegate = RunFrameUpdate });
        InsertAfter<FixedUpdate.ScriptRunBehaviourFixedUpdate>(ref loop,
            new PlayerLoopSystem { type = typeof(PhysicsUpdateSystem), updateDelegate = RunPhysicsUpdate });

        PlayerLoop.SetPlayerLoop(loop);

        Application.quitting -= Shut;
        Application.quitting += Shut;

        try { Game.Init(); }
        catch (Exception e) { Debug.LogError($"CustomLoop.Init threw: {e}"); }
    }

    static void Shut()
    {
        if (!initialized) return;
        initialized = false;
        Application.quitting -= Shut;

        try { Game.Shutdown(); }
        catch (Exception e) { Debug.LogError($"CustomLoop.Shutdown threw: {e}"); }

        PlayerLoopSystem loop = PlayerLoop.GetCurrentPlayerLoop();
        RemoveSystem<FrameUpdateSystem>(ref loop);
        RemoveSystem<PhysicsUpdateSystem>(ref loop);
        PlayerLoop.SetPlayerLoop(loop);
    }

    static void RunFrameUpdate()
    {
        if (!initialized) return;
        try { Game.FrameUpdate(Time.deltaTime); }
        catch (Exception e) { Debug.LogError($"Game.FrameUpdate threw: {e}"); }
    }

    static void RunPhysicsUpdate()
    {
        if (!initialized) return;
        try { Game.PhysicsUpdate(Time.fixedDeltaTime); }
        catch (Exception e) { Debug.LogError($"Game.PhysicsUpdate threw: {e}"); }
    }

    static bool InsertAfter<T>(ref PlayerLoopSystem parent, PlayerLoopSystem newSystem)
    {
        if (parent.subSystemList == null) return false;

        var list = new List<PlayerLoopSystem>(parent.subSystemList);
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].type == typeof(T))
            {
                list.Insert(i + 1, newSystem);
                parent.subSystemList = list.ToArray();
                return true;
            }
            var child = list[i];
            if (InsertAfter<T>(ref child, newSystem))
            {
                list[i] = child;
                parent.subSystemList = list.ToArray();
                return true;
            }
        }
        return false;
    }

    static void RemoveSystem<T>(ref PlayerLoopSystem parent)
    {
        if (parent.subSystemList == null) return;
        var list = new List<PlayerLoopSystem>(parent.subSystemList);
        list.RemoveAll(s => s.type == typeof(T));
        for (int i = 0; i < list.Count; i++)
        {
            var child = list[i];
            RemoveSystem<T>(ref child);
            list[i] = child;
        }
        parent.subSystemList = list.ToArray();
    }
}
