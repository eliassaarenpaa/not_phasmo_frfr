using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnomalyObject : MonoBehaviour
{
    public RoomType room;

    public Vector3 original_pos;
    public Vector3 original_scale;


    void OnEnable()
    {
        AnomalySystem.Register(this);
        original_pos = transform.position;
        original_scale = transform.localScale;
    }

    void OnDisable() => AnomalySystem.Unregister(this);
}
