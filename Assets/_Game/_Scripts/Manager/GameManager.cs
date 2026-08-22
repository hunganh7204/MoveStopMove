//using UnityEngine;

//public enum GameState
//{
//    MainMenu,
//    Playing,
//    Paused,
//    GameOver,
//    Win
//}

//public class GameManager : Singleton<GameManager>
//{
//    private GameState currentState;

//    private int currentLevel = 1;

//    private void Start()
//    {
//        ChangeState(GameState.MainMenu);
//    }

//    public void ChangeState(GameState newState)
//    {
//        currentState = newState;

//        switch (newState)
//        {
//            case GameState.MainMenu:
//                Time.timeScale = 1f;
//                // TODO: call menuUI
//                break;

//            case GameState.Playing:
//                Time.timeScale = 1f;
//                // TODO: call gameplayUI
//                break;

//            case GameState.Paused:
//                Time.timeScale = 0f;
//                // TODO: call PauseUI
//                break;

//            case GameState.GameOver:
//                Time.timeScale = 1f;
//                BotManager.Instance.ClearAllBots();
//                // TODO: call LoseUI
//                break;

//            case GameState.Win:
//                Time.timeScale = 1f;
//                BotManager.Instance.ClearAllBots();
//                // TODO: call WInUI
//                break;
//        }
//    }

//    public void StartGame()
//    {
//        LevelManager.Instance.OnInit(currentLevel);
//        ChangeState(GameState.Playing);
//    }

//    public void PauseGame()
//    {
//        ChangeState(GameState.Paused);
//    }

//    public void ResumeGame()
//    {
//        ChangeState(GameState.Playing);
//    }

//    public void Lose()
//    {
//        ChangeState(GameState.GameOver);
//    }

//    public void Win()
//    {
//        currentLevel++;
//        ChangeState(GameState.Win);
//    }

//    public GameState GetGameState()
//    {
//        return currentState;
//    }
//}
using UnityEngine;

public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver,
    Win
}

public class GameManager : Singleton<GameManager>
{
    private GameState currentState;
    private int currentLevel = 1;

    private void Start()
    {
        StartGame();
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case GameState.MainMenu:
                Time.timeScale = 1f;
                break;
            case GameState.Playing:
                Time.timeScale = 1f;
                break;
            case GameState.Paused:
                Time.timeScale = 0f;
                break;
            case GameState.GameOver:
                Time.timeScale = 1f;
                BotManager.Instance.ClearAllBots();
                break;
            case GameState.Win:
                Time.timeScale = 1f;
                BotManager.Instance.ClearAllBots();
                break;
        }
    }


    [ContextMenu("Test: Start/Restart Game")]
    public void StartGame()
    {
        BotManager.Instance.ClearAllBots();

        LevelManager.Instance.OnInit(currentLevel);
        ChangeState(GameState.Playing);
    }

    [ContextMenu("Test: Pause Game")]
    public void PauseGame() => ChangeState(GameState.Paused);

    [ContextMenu("Test: Resume Game")]
    public void ResumeGame() => ChangeState(GameState.Playing);

    [ContextMenu("Test: Force Lose")]
    public void Lose() => ChangeState(GameState.GameOver);

    [ContextMenu("Test: Force Win")]
    public void Win()
    {
        currentLevel++; 
        ChangeState(GameState.Win);
        Debug.Log($"<color=green>win,next level: {currentLevel}</color>");
    }

    public GameState GetGameState() => currentState;
}