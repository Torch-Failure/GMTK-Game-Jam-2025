using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameManager gameManager;
    
    void Start() {
        CloseMenus();
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
