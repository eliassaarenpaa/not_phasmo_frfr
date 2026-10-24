using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTeleporter : MonoBehaviour
{
    public bool isInside;
    public DoorTriggerArea trigger;
    public Transform exit;

    public GameObject sceneEnable;

    public GameObject sceneDisable;

    [HideInInspector]
    public Vector3 originalSceneEnablePos;

    [HideInInspector]
    public Vector3 originalSceneDisablePos;

    private void Awake()
    {
        originalSceneEnablePos = sceneEnable.transform.position;
        originalSceneDisablePos = sceneDisable.transform.position;
    }

    private void OnEnable() => DoorSystem.Register(this);
    private void OnDisable() => DoorSystem.Unregister(this);
}
