using UnityEngine;

public enum GameState
{
    Menu,
    Playing,
    Win,
    Lose
}

public class GameFlowSystem
{
    public GameState state = GameState.Menu;
    public static GameFlow flow;


    public static void Register(GameFlow f)
    {
        flow = f;
    }
    float timeOnEndEnter;


    public bool UpdateState(bool isPlay, bool isWin, bool isLose)
    {
        if (flow == null) return false;

        switch (state)
        {
            case GameState.Menu:
                if (isPlay) return ChangeState(GameState.Playing);
                break;

            case GameState.Playing:
                if (isWin) return ChangeState(GameState.Win);
                else if (isLose) return ChangeState(GameState.Lose);
                break;

            case GameState.Win:
            case GameState.Lose:
                const float delayToMenu = 3.0f;
                if (Time.time >= timeOnEndEnter + delayToMenu)
                {
                    return ChangeState(GameState.Menu);
                }
                break;
        }
        return false;
    }

    bool ChangeState(GameState next)
    {
        if (state == next) return false;
        state = next;

        Debug.Log($"{state} -> {next} at {Time.time}");

        switch (next)
        {
            case GameState.Menu:
                break;

            case GameState.Playing:
                break;

            case GameState.Win:
            case GameState.Lose:
                timeOnEndEnter = Time.time;
                break;
        }

        return true;
    }
}