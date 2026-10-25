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

    public void LookAt(Vector3 dir)
    {
        if (dir.sqrMagnitude < 1e-6f) return;
        dir.Normalize();

        yaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        pitch = -Mathf.Asin(dir.y) * Mathf.Rad2Deg; 
        pitch = Mathf.Clamp(pitch, camera.minPitch, camera.maxPitch);
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

    public void Zoom(PlayerInput input, float dt)
    {
        float fov_target = input.isZoom ? 30.0f : 60.0f;
        camera.cam.fieldOfView = Mathf.Lerp(camera.cam.fieldOfView, fov_target, 10.0f * dt);
    }

    public void LerpToTrasnform(Transform t, float dt)
    {
        Vector3 p = Vector3.Lerp(camera.transform.position, t.position, camera.transitionSpeedLinear * dt);
        Quaternion r = Quaternion.Lerp(camera.transform.rotation, t.rotation, camera.transitionSpeedAngular * dt);
        camera.transform.SetPositionAndRotation(p, r);
    }

}
