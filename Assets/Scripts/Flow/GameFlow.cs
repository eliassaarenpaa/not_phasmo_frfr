using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlow : MonoBehaviour
{
    public GameObject menu;
    public GameObject world;

    public GameObject interior;
    public GameObject exterior;

    private void OnEnable() => GameFlowSystem.Register(this);
}
