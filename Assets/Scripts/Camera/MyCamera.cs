using UnityEngine;

public class MyCamera : MonoBehaviour
{
    public Vector3 offset;
    public float sensitivity = 2f; // degrees per mouse unit
    public float minPitch = -85f;
    public float maxPitch = 85f;

    void OnEnable() => CameraSystem.Register(this);
    void OnDisable() => CameraSystem.Unregister(this);
}
