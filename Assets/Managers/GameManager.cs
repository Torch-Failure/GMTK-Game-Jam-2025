using UnityEngine;
using UnityEngine.InputSystem;

public enum GameState
{
    MainMenu,
    GameSetup,
    Gameplay,
    Pause,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public GameState currentState;
    
    private InputAction pauseAction;
    private InputAction replayAction;

    [SerializeField]
    private PlayerManager playerManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pauseAction = InputSystem.actions.FindAction("Pause");
        replayAction = InputSystem.actions.FindAction("Replay");
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case GameState.MainMenu:
                MainMenu();
                break;
            case GameState.GameSetup:
                GameSetup();
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

    void FixedUpdate()
    {
        switch (currentState)
        {
            case GameState.Gameplay:
                GameplayFixedUpdate();
                break;
        }
    }

    void MainMenu()
    {
        Debug.Log("MainMenu");
        currentState = GameState.GameSetup;
    }

    void GameSetup()
    {
        Debug.Log("Game Setup");
        playerManager.PlayNextCharacter();
        currentState = GameState.Gameplay;
    }


    void Gameplay()
    {
        // Debug.Log("Gameplay");
        if (pauseAction.WasPressedThisFrame())
        {
            currentState = GameState.Pause;
        } 
        else if (replayAction.WasPressedThisFrame()) // Temp debug action.
        {
            playerManager.PlayNextCharacter();
        }
    }

    void GameplayFixedUpdate()
    {
        playerManager.InternalUpdate();
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