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
                if (isPlay) return ChangeState(GameState.Playing); // restart
                break;
        }
        return false;
    }

    bool ChangeState(GameState next)
    {
        if (state == next) return false;
        state = next;

        switch (next)
        {
            case GameState.Menu:
                break;

            case GameState.Playing:
                break;

            case GameState.Win:
                break;

            case GameState.Lose:
                break;
        }

        return true;
    }
}