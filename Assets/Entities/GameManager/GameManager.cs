using UnityEngine;
using UnityEngine.InputSystem;

public enum GameState
{
    MainMenu,
    Gameplay,
    Pause,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public GameState currentState;
    private InputAction pauseAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pauseAction = InputSystem.actions.FindAction("Pause");
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case GameState.MainMenu:
                MainMenu();
                break;
            case GameState.Gameplay:
                Gameplay();
                break;
            case GameState.Pause:
                Pause();
                break;
            case GameState.GameOver:
                GameOver();
                break;
        }
    }

    void MainMenu()
    {
        Debug.Log("MainMenu");
        currentState = GameState.Gameplay;
    }

    void Gameplay()
    {
        Debug.Log("Gameplay");
        if (pauseAction.WasPressedThisFrame())
        {
            currentState = GameState.Pause;
        }
    }

    void Pause()
    {
        Debug.Log("Pause");
        if (pauseAction.WasPressedThisFrame())
        {
            currentState = GameState.Gameplay;
        }
    }

    void GameOver()
    {
        Debug.Log("GameOver");
    }
}