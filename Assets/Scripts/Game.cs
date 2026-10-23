using UnityEngine;

public static class Game
{
    static PlayerInputSystem playerInputSystem;
    static PlayerSystem playerSystem;
    static CameraSystem cameraSystem;

    // Once before the first scene loads (before any Awake)
    public static void Init()
    {
        playerInputSystem = new PlayerInputSystem();
        playerSystem = new PlayerSystem();
        cameraSystem = new CameraSystem();
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
        playerInputSystem.ReadInputs();

        if (PlayerSystem.player)
        {
            cameraSystem.FirstPersonFollowRotate(playerInputSystem.input, PlayerSystem.player.transform, dt);
        }
    }

    // On every physics step after FixedUpdate.ScriptRunBehaviourFixedUpdate
    public static void PhysicsUpdate(float dt)
    {
        //Rigidbody rb;
        //rb.Move(....);
    }
}