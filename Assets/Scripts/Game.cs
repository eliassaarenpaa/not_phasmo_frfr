using UnityEngine;

public static class Game
{
    static PlayerInputSystem playerInputSystem;
    static PlayerSystem playerSystem;
    static CameraSystem cameraSystem;
    static LampSystem lampSystem;
    static AnomalySystem anomalySystem;
    static DoorSystem doorSystem;

    // Once before the first scene loads (before any Awake)
    public static void Init()
    {
        playerInputSystem = new PlayerInputSystem();
        playerSystem = new PlayerSystem();
        cameraSystem = new CameraSystem();
        lampSystem = new LampSystem();
        anomalySystem = new AnomalySystem();
        anomalySystem.BeginAnomalies();
        doorSystem = new DoorSystem();

        Debug.Log("init");

        Cursor.lockState = CursorLockMode.Locked; // freeze cursor on screen centre
        Cursor.visible = false; // invisible cursor
    }

    // Once on quit / exiting play mode
    public static void Shutdown()
    {
        Debug.Log("shutdown");
    }

    // On every frame after Update.ScriptRunBehaviourUpdate
    public static void FrameUpdate(float dt)
    {
        PlayerInput input = playerInputSystem.ReadInputs();

        anomalySystem.Update(dt);

        if (AnomalySystem.IsReporting())
        {
            input.mouseDx = 0;
            input.mouseDy = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        doorSystem.Update(input, dt);

        if (doorSystem.isEnterDoor)
        {
            PlayerSystem.player.transform.position = doorSystem.doorExitPoint;
        }

        if (PlayerSystem.player && CameraSystem.camera)
        {
            Transform cam_target = PlayerSystem.player.transform;
            cameraSystem.FirstPersonFollowRotate(input, cam_target, dt);

            Vector3 fwd = CameraSystem.camera.transform.forward;
            Vector3 right = CameraSystem.camera.transform.right;
            playerSystem.Move(input, fwd, right, dt);

            playerSystem.UpdateFootstepSound(doorSystem.isPlayerOutside);
        }

    }

    // On every physics step after FixedUpdate.ScriptRunBehaviourFixedUpdate
    public static void PhysicsUpdate(float dt)
    {
        //Rigidbody rb;
        //rb.Move(....);
    }
}