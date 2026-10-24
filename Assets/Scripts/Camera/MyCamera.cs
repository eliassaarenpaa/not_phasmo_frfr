using UnityEngine;

public class MyCamera : MonoBehaviour
{
    public Vector3 offset;
    public float sensitivity = 2f; // degrees per mouse unit
    public float minPitch = -85f;
    public float maxPitch = 85f;

    public float transitionSpeedLinear = 2.0f;
    public float transitionSpeedAngular = 10.0f;

    void OnEnable() => CameraSystem.Register(this);
    void OnDisable() => CameraSystem.Unregister(this);
}
