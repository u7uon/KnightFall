using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinGameUIManager : MonoBehaviour
{
    [SerializeField] private Button tryAgainButton;
    [SerializeField] private Button mainMenuButton;

    private void Awake()
    {
        tryAgainButton.onClick.AddListener(OnTryAgainClicked);
        mainMenuButton.onClick.AddListener(OnMainMenuClicked);
    }

    private void OnTryAgainClicked()
    {
        Time.timeScale = 1f ;
        // Load the game scene again
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    private void OnMainMenuClicked()
    {
        Time.timeScale = 1f ;
        // Load the main menu scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    void OnDestroy()
    {
        tryAgainButton.onClick.RemoveListener(OnTryAgainClicked);
        mainMenuButton.onClick.RemoveListener(OnMainMenuClicked);
    }


}
