using UnityEngine;

public static class Game
{
    static PlayerInputSystem playerInputSystem;
    static PlayerSystem playerSystem;
    static CameraSystem cameraSystem;
    static LampSystem lampSystem;
    static AnomalySystem anomalySystem;
    static DoorSystem doorSystem;
    static InteractSystem interactSystem;

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
        interactSystem = new InteractSystem();

        Debug.Log("init");
    }

    // Once on quit / exiting play mode
    public static void Shutdown()
    {
        Debug.Log("shutdown");
    }

    // On every frame after Update.ScriptRunBehaviourUpdate
    public static void FrameUpdate(float dt)
    {
        if (!PlayerSystem.player || !CameraSystem.camera) return;

        PlayerInput input = playerInputSystem.ReadInputs();

        anomalySystem.Update(input, dt);

        if (anomalySystem.isWaitingForResult)
        {
            input.moveX = 0;
            input.moveZ = 0;
            input.mouseDx = 0;
            input.mouseDy = 0;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = anomalySystem.isReporting ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = anomalySystem.isReporting;
        }

        if (anomalySystem.isReporting)
        {
            input.moveX = 0;
            input.moveZ = 0;
            input.mouseDx = 0;
            input.mouseDy = 0;

            Transform cam_target = AnomalySystem.ui.cameraPcLookat;
            cameraSystem.LerpToTrasnform(cam_target, dt);
        }
        else
        {
            Transform cam_target = PlayerSystem.player.transform;
            cameraSystem.FirstPersonFollowRotate(input, cam_target, dt);

            playerSystem.UpdateFootstepSound(doorSystem.isPlayerOutside);

            doorSystem.Update(input, dt);

            if (doorSystem.isEnterDoor)
            {
                PlayerSystem.player.transform.SetPositionAndRotation(doorSystem.doorExitTransform.position, doorSystem.doorExitTransform.rotation);
            }
        }

        LampSystem.SetLampActive(!AnomalySystem.trigger.isPlayerOnTrigger);

        Vector3 fwd = CameraSystem.camera.transform.forward;
        Vector3 right = CameraSystem.camera.transform.right;
        playerSystem.Move(input, fwd, right, dt);

        interactSystem.Update(doorSystem.isPlayerOnDoorTrigger, AnomalySystem.trigger.isPlayerOnTrigger, anomalySystem.isReporting, anomalySystem.isWaitingForResult);
    }

    // On every physics step after FixedUpdate.ScriptRunBehaviourFixedUpdate
    public static void PhysicsUpdate(float dt)
    {
        //Rigidbody rb;
        //rb.Move(....);
    }
}