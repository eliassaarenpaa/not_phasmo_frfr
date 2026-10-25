using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DoorSystem
{
    public bool isPlayerOutside = true;

    static List<DoorTeleporter> doorTPs = new List<DoorTeleporter>();

    public int numTimesEnteredDoor = 0;

    public bool isEnterDoor;
    public bool isPlayerOnDoorTrigger;
    public Transform doorExitTransform;


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
        isPlayerOnDoorTrigger = door != null;

        if (door != null)
        {
            if (input.isInteract)
            {
                door.sceneDisable.transform.position = new Vector3(9999, 9999, 9999);
                door.sceneEnable.transform.position = new Vector3(0, 0, 0);

                door.sceneDisable.SetActive(true);
                door.sceneEnable.SetActive(true);

                isPlayerOutside = door.isInside; // interacting with inside door == going outside
                isEnterDoor = true;
                doorExitTransform = door.exit.transform;
                numTimesEnteredDoor++;
            }
        }

    }


    public void Reset()
    {
        isPlayerOutside = true;
        numTimesEnteredDoor = 0;

        isEnterDoor = false;
        isPlayerOnDoorTrigger = false;
        doorExitTransform = null;
    }


}
