using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnomalyGracePeriodLights : MonoBehaviour
{
    public AudioSource shutdownSource;

    private void OnEnable() => AnomalySystem.Register(this);
}
