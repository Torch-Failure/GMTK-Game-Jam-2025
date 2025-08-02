using UnityEngine;
using UnityEngine.UI;

public enum UIState {
    None,
    MainMenu,
    PauseMenu,
    PlayerSelection,
    ThreadPlaying,
}

public class UI : MonoBehaviour
{
    static UI instance;

    [SerializeField] GameManager gameManager;

    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject playerSelectionMenu;

    [SerializeField] Button playButton;
    [SerializeField] Button resumeButton;

    [SerializeField] Button selectCharacterButton;
    [SerializeField] Button previousCharacterButton;
    [SerializeField] Button nextCharacterButton;

    private UIState currentState = UIState.None;
    private UIState previousState = UIState.None; 
    
    void Start() {
        instance = this;

        playButton.onClick.AddListener(gameManager.PlayGame);
        resumeButton.onClick.AddListener(gameManager.ResumeGame);

        selectCharacterButton.onClick.AddListener(gameManager.loopManager.playerManager.SelectCharacter);
        previousCharacterButton.onClick.AddListener(gameManager.loopManager.playerManager.PreviousCharacter);
        nextCharacterButton.onClick.AddListener(gameManager.loopManager.playerManager.NextCharacter);
    }

    public static void SetState(UIState newState)
    {
        instance.CloseAllScreens();
        switch (newState)
        {
            case UIState.MainMenu:
                instance.mainMenu.SetActive(true);
                break;
            case UIState.PauseMenu:
                instance.pauseMenu.SetActive(true);
                break;
            case UIState.PlayerSelection:
                instance.playerSelectionMenu.SetActive(true);
                break;
        }
        instance.previousState = instance.currentState;
        instance.currentState = newState;
    }

    public static void Pause() {
        SetState(UIState.PauseMenu);
    }

    public static void Unpause() {
        SetState(instance.previousState);
    }

    public void CloseAllScreens() {
        mainMenu.SetActive(false);
        pauseMenu.SetActive(false);
        playerSelectionMenu.SetActive(false);
    }
}
