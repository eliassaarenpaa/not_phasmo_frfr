using System;
using UnityEngine;
using UnityEngine.Assertions;

public class CameraSystem
{
    static MyCamera camera;

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
        // Mouse axes are already per-frame deltas, so don't multiply by dt.
        yaw += input.mouseDx * camera.sensitivity;
        pitch -= input.mouseDy * camera.sensitivity;   // minus: mouse up = look up
        pitch = Mathf.Clamp(pitch, camera.minPitch, camera.maxPitch);

        Quaternion look = Quaternion.Euler(pitch, yaw, 0f);

        camera.transform.SetPositionAndRotation(
            target.position + camera.offset,
            look);
    }
    public void Follow(Transform t, float move_speed, float look_speed_deg, float dt)
    {
        Assert.IsNotNull(camera);
        Assert.IsNotNull(t);
        if (!camera || !t) { return; }

        Vector3 pos = camera.transform.position;
        float k = 1f - Mathf.Exp(-move_speed * dt);
        pos = Vector3.Lerp(pos, t.position + camera.offset, k);
        camera.transform.position = pos;

        Quaternion look = Quaternion.LookRotation((t.position - pos).normalized);
        camera.transform.rotation = Quaternion.RotateTowards(camera.transform.rotation, look, look_speed_deg * dt);
    }
}
