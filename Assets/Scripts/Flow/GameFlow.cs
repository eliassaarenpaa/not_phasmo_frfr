using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlow : MonoBehaviour
{
    public GameObject menu;
    public GameObject world;

    private void OnEnable() => GameFlowSystem.Register(this);
}
