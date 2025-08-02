using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameManager gameManager;

    [SerializeField] Button playButton;
    [SerializeField] Button resumeButton;
    
    void Start() {
        playButton.onClick.AddListener(gameManager.PlayGame);
        resumeButton.onClick.AddListener(gameManager.ResumeGame);
    }

    public void OpenMainMenu() {
        mainMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void OpenPauseMenu() {
        mainMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void CloseMenus() {
        mainMenu.SetActive(false);
        pauseMenu.SetActive(false);
    }
}
