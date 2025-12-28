using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject stageCompletePanel;
    [SerializeField] private TextMeshProUGUI stageCompleteText;


    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject pauseGameUI;

    void OnEnable()
    {
        UIEventManager.OnStageCompleted += ShowStageCompleteUI;
        UIEventManager.OnGameStart +=  ShowStartGamePanel  ;
        UIEventManager.OnGameOver += GameOver ; 
        UIEventManager.OnGamePaused += PauseGame;
        UIEventManager.OnGameResume += ResumeGame; 
        UIEventManager.OnMainmenuShow += ShowMainMenu; 
    }
    void OnDisable()
    {
        UIEventManager.OnStageCompleted -= ShowStageCompleteUI;
        UIEventManager.OnGameStart -= ShowStartGamePanel; 
        UIEventManager.OnGameOver -= GameOver ; 
        UIEventManager.OnGamePaused -= PauseGame;
        UIEventManager.OnGameResume -= ResumeGame; 
        UIEventManager.OnMainmenuShow -= ShowMainMenu; 
    }

    private void ShowStartGamePanel()
    {
        StartGame();

        stageCompletePanel.SetActive(true);
        stageCompleteText.text = "Start Game \n Stage 1";

        CancelInvoke();
        Invoke(nameof(HidePanel), 4f);
    }

    private void ShowStageCompleteUI(int stage)
    {
        stageCompletePanel.SetActive(true);
        stageCompleteText.text = "Enemies Cleared!";
        
        CancelInvoke();
        Invoke(nameof(ShowNextStageText), 2f);  

        Invoke(nameof(HidePanel), 4f);
    }

    private void ShowNextStageText()
    {
        // Lấy Stage hiện tại từ StageManager hoặc lưu tạm từ event
        int nextStage = FindAnyObjectByType<StageManager>().currentStage;

        if (nextStage % 5 == 0)
        {
            stageCompleteText.color = Color.red;
            stageCompleteText.text = $"Stage {nextStage} \n Boss coming";
        }
        else
        {
            stageCompleteText.color = Color.white;
            stageCompleteText.text = $"Stage {nextStage}";
        }
    }

    private void HidePanel()
    {
        stageCompletePanel.SetActive(false);
    }

    public void StartGame()
    {
        mainMenuUI.SetActive(false);
        gameOverUI.SetActive(false);
        pauseGameUI.SetActive(false);
    }

    private void Mainmenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ShowMainMenu()
    {
        mainMenuUI.SetActive(true);
        gameOverUI.SetActive(false);
        pauseGameUI.SetActive(false);
    }

    private void PauseGame()
    {
        pauseGameUI.SetActive(true);
        mainMenuUI.SetActive(false);
        gameOverUI.SetActive(false);
    }

    private void GameOver()
    {
        gameOverUI.SetActive(true);
        pauseGameUI.SetActive(false);
        mainMenuUI.SetActive(false);
    }
    
    private void ResumeGame()
    {
        mainMenuUI.SetActive(false);
        gameOverUI.SetActive(false);
        pauseGameUI.SetActive(false);
    }
    
}
    

