using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DoorSystem
{
    public bool isPlayerOutside = true;

    static DoorUI ui;

    static List<DoorTeleporter> doorTPs = new List<DoorTeleporter>();

    public bool isEnterDoor;
    public Vector3 doorExitPoint;


    public static void Register(DoorUI _ui)
    {
        ui = _ui;
    }

    public static void Register(DoorTeleporter tp)
    {
        doorTPs.Add(tp);
    }

    public static void Unregister(DoorTeleporter tp)
    {
        doorTPs.Remove(tp);
    }

    public void Update(PlayerInput input, float dt)
    {
        isEnterDoor = false;

        DoorTeleporter door = doorTPs.FirstOrDefault(d => d.trigger.isPlayerOnTrigger);

        if (door != null)
        {
            if (input.isInteract)
            {
                isPlayerOutside = door.isInside; // interacting with inside door == going outside
                isEnterDoor = true;
                doorExitPoint = door.exit.transform.position;
            }
        }

        ui.gameObject.SetActive(door != null);

    }
}
