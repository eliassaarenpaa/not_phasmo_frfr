using System;
using UnityEngine;
using UnityEngine.Assertions;

public class CameraSystem
{
    public static MyCamera camera;

    float yaw;
    float pitch;

    public static void Register(MyCamera c)
    {
        camera = c;
    }

    public static void Unregister(MyCamera c)
    {
        camera = null;
    }

    public void FirstPersonFollowRotate(PlayerInput input, Transform target, float dt)
    {
        yaw += input.mouseDx * camera.sensitivity;
        pitch -= input.mouseDy * camera.sensitivity;
        pitch = Mathf.Clamp(pitch, camera.minPitch, camera.maxPitch);

        Quaternion look = Quaternion.Euler(pitch, yaw, 0f);

        camera.transform.SetPositionAndRotation(
            target.position + camera.offset,
            look);
    }
   
}
