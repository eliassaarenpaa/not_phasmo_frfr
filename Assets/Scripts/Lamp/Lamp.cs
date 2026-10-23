using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : MonoBehaviour
{
    public Light light;
    void OnEnable() => LampSystem.Register(this);
    void OnDisable() => LampSystem.Unregister(this);
}
