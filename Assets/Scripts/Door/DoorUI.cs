using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorUI : MonoBehaviour
{
    private void OnEnable() => DoorSystem.Register(this);

}
