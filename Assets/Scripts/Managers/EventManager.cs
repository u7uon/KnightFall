using System;
using UnityEngine;

public class UIEventManager : MonoBehaviour
{

    public static event Action<int> OnStageCompleted;
    public static event Action OnGameStart;

    public static event Action OnWinGame; 

    public static event Action OnGamePaused;
    public static event Action OnGameResume;
    public static event Action OnGameOver;
    public static event Action OnMainmenuShow;

    public static event Action<int> OnLevelUp; 

       
    // Gọi event
    public static void StageCompleted(int stage)
    {
        OnStageCompleted?.Invoke(stage);
    }

    public static void GameStart()
    {
        OnGameStart?.Invoke();
    }

    public static void PauseGame()
    {
        OnGamePaused?.Invoke();
    }

    public static void GameOver()
    {
        OnGameOver?.Invoke();
    }
    public static void ResumeGame()
    {
        OnGameResume?.Invoke();
    }
    public static void MainMenu()
    {
        OnMainmenuShow?.Invoke();
    }

    public static void WinGame()
    {
        OnWinGame?.Invoke();
    }
    
    public static void LevelUp(int lv)
    {
        OnLevelUp?.Invoke(lv); 
    }
}
