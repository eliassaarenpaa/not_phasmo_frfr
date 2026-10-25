using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnomalyGracePeriodLights : MonoBehaviour
{
    private void OnEnable() => AnomalySystem.Register(this);
}
