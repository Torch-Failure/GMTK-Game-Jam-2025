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
    public static bool enableFriendlyFire = true;

    public GameState currentState = GameState.MainMenu;
    
    private InputAction pauseAction;
    private InputAction replayAction;

    public LoopManager loopManager;
    [SerializeField] private UI ui;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pauseAction = InputSystem.actions.FindAction("Pause");
        replayAction = InputSystem.actions.FindAction("Replay");

        UI.SetState(UIState.MainMenu);
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

    public void PlayGame() {
        if (currentState == GameState.MainMenu) {
            currentState = GameState.GameSetup;
            ui.CloseAllScreens();
            Debug.Log("Game Setup");
        }
    }

    public void ResumeGame() {
        if (currentState == GameState.Pause) {
            currentState = GameState.Gameplay;
            ui.CloseAllScreens();
            Debug.Log("Game Setup");
        }
    }

    void MainMenu()
    {
    }

    void GameSetup()
    {
        loopManager.RestartLoop();
        currentState = GameState.Gameplay;
        Debug.Log("Gameplay");
    }


    void Gameplay()
    {
        if (pauseAction.WasPressedThisFrame())
        {
            currentState = GameState.Pause;
            UI.Pause();
            Debug.Log("Pause");
        } 
        else if (replayAction.WasPressedThisFrame()) // Temp debug action.
        {
            loopManager.RestartLoop();
        }
        loopManager.InternalUpdate();
    }

    void GameplayFixedUpdate()
    {
        loopManager.InternalFixedUpdate();
    }

    void Pause()
    {
        if (pauseAction.WasPressedThisFrame())
        {
            currentState = GameState.Gameplay;
            UI.Unpause();
        }
    }

    void GameOver()
    {
        Debug.Log("GameOver");
    }
}