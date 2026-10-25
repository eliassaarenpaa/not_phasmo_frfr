using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public static class Game
{
    static bool isMenu = false;
    static bool gameWon = false;
    static bool gameLost = false;

    static GameFlowSystem gameFlowSystem;
    static PlayerInputSystem playerInputSystem;
    static PlayerSystem playerSystem;
    static CameraSystem cameraSystem;
    static LampSystem lampSystem;
    static AnomalySystem anomalySystem;
    static DoorSystem doorSystem;
    static InteractSystem interactSystem;
    static ClockSystem clockSystem;
    static MenuSystem menuSystem;


    // Once before the first scene loads (before any Awake)
    public static void Init()
    {
        gameFlowSystem = new GameFlowSystem();
        playerInputSystem = new PlayerInputSystem();
        playerSystem = new PlayerSystem();
        cameraSystem = new CameraSystem();
        lampSystem = new LampSystem();
        anomalySystem = new AnomalySystem();
        doorSystem = new DoorSystem();
        interactSystem = new InteractSystem();
        clockSystem = new ClockSystem();
        menuSystem = new MenuSystem();

        Debug.Log("init");
    }

    // Once on quit / exiting play mode
    public static void Shutdown()
    {
        Debug.Log("shutdown");
    }

    // On every frame after Update.ScriptRunBehaviourUpdate

    public static void RestartGame()
    {
        if (!PlayerSystem.player || !CameraSystem.camera) return;

        playerSystem.Reset();
        clockSystem.Reset();
        anomalySystem.Reset();
        doorSystem.Reset();
        gameWon = false;
        gameLost = false;
        GameFlowSystem.flow.exterior.gameObject.SetActive(true);
        GameFlowSystem.flow.interior.gameObject.SetActive(false);
        GameFlowSystem.flow.exterior.transform.position = new Vector3(0, 0, 0);
    }

    public static void FrameUpdate(float dt)
    {
        if (!GameFlowSystem.flow) return;

        if (gameFlowSystem.UpdateState(menuSystem.IsPlay(), gameWon, gameLost))
        {
            GameState newState = gameFlowSystem.state;
            switch (newState)
            {
                case GameState.Menu:
                    GameFlowSystem.flow.world.gameObject.SetActive(false);
                    GameFlowSystem.flow.menu.gameObject.SetActive(true);
                    menuSystem.SetScreen(MenuScreen.MainMenu);
                    break;
                case GameState.Playing:
                    GameFlowSystem.flow.world.gameObject.SetActive(true);
                    GameFlowSystem.flow.menu.gameObject.SetActive(false);
                    RestartGame();
                    break;
                case GameState.Win:
                    GameFlowSystem.flow.world.gameObject.SetActive(false);
                    GameFlowSystem.flow.menu.gameObject.SetActive(true);
                    menuSystem.SetScreen(MenuScreen.WinMenu);
                    break;
                case GameState.Lose:
                    GameFlowSystem.flow.world.gameObject.SetActive(false);
                    GameFlowSystem.flow.menu.gameObject.SetActive(true);
                    menuSystem.SetScreen(MenuScreen.LoseMenu);
                    break;
            }
        }

        switch (gameFlowSystem.state)
        {
            case GameState.Menu:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            case GameState.Playing:
                {
                    if (!PlayerSystem.player || !CameraSystem.camera) return;


                    if (clockSystem.timer >= ClockSystem.totalGameDuration)
                    {
                        if (!gameWon)
                        {
                            // WIN GAME????
                            gameWon = true;
                        }

                        return;
                    }

                    if (anomalySystem.active.Count >= AnomalySystem.maxUnreportedAnomalies)
                    {
                        // Lose game??
                        if (!gameLost)
                        {
                            gameLost = true;
                        }
                    }

                    PlayerInput input = playerInputSystem.ReadInputs();

                    anomalySystem.UpdateReportUI(input);

                    // Only begin timer when player has first entered the house
                    if (doorSystem.isEnterDoor && doorSystem.numTimesEnteredDoor == 1)
                    {

                    }

                    // Only update anomalies and timer after first time entered house
                    if (doorSystem.numTimesEnteredDoor > 0)
                    {
                        clockSystem.UpdateTimer(dt);
                        anomalySystem.UpdateAnomalyEventLoop(input, clockSystem.timer, ClockSystem.totalGameDuration, dt);
                    }
                    clockSystem.UpdateUI();


                    LampSystem.SetLampActive(!AnomalySystem.trigger.isPlayerOnTrigger);

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
                            cameraSystem.LookAt(doorSystem.doorExitTransform.forward);
                        }
                    }


                    Vector3 fwd = CameraSystem.camera.transform.forward;
                    Vector3 right = CameraSystem.camera.transform.right;

                    playerSystem.Move(input, fwd, right, dt);

                    interactSystem.Update(doorSystem.isPlayerOnDoorTrigger, AnomalySystem.trigger.isPlayerOnTrigger, anomalySystem.isReporting, anomalySystem.isWaitingForResult);
                }
                break;
            case GameState.Win:
            case GameState.Lose:
                break;
        }

    }

    // On every physics step after FixedUpdate.ScriptRunBehaviourFixedUpdate
    public static void PhysicsUpdate(float dt)
    {
        //Rigidbody rb;
        //rb.Move(....);
    }
}