using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTeleporter : MonoBehaviour
{
    public bool isInside;
    public DoorTriggerArea trigger;
    public Transform exit;

    private void OnEnable() => DoorSystem.Register(this);
    private void OnDisable() => DoorSystem.Unregister(this);
}
