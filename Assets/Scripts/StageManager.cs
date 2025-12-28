using System;
using TMPro;    
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static event Action<int> OnStageEnd;
    public static event Action<int> OnStageStart ; 

    [Header("Stage Settings")]
    public float stageDuration = 30f;
    private float stageSpawnTime; // Thời gian thực tế spawn
    private float playedTime = 0f; // Total time survived in current game session
    public int currentStage = 1;
    [Header("References")]
    public EnemySpawner spawner;
    [SerializeField] private TextMeshProUGUI stageTimerText;
    [SerializeField] private GameObject shopUI; // Reference to ShopUI
    [SerializeField] private GameObject hudUI; 


    private const int STAGE_TO_WIN = 30;
    
    [Header("Difficulty Scaling")]

    private bool isPlaying = false;
    private bool isSpawning = false; // Track spawning state

    void Awake()
    {
        if (spawner == null)
        {
            spawner = FindAnyObjectByType<EnemySpawner>();
        }


        // Make sure shop is hidden at start
        if (shopUI != null)
        {
            shopUI.SetActive(false);
        }

    }

    public void Play()
    {
        currentStage = 1;
        stageDuration = 30f;
        stageSpawnTime = 0f;
        playedTime = 0f; // Reset time tracking for new game
        isPlaying = true;
        ApplyStage(currentStage);
    }

    public void Stop()
    {
        isPlaying = false;
        isSpawning = false;
        if (spawner != null)
        {
            spawner.StopSpawning();
            spawner.ClearAllEnemies();
        }
    }

    void Update()
    {
        if (!isPlaying) return;

        // Chỉ đếm thời gian khi đang spawn
        if (isSpawning)
        {
            stageSpawnTime += Time.deltaTime;
            playedTime += Time.deltaTime;
        }

        float remainingTime = Mathf.Max(0f, stageDuration - stageSpawnTime);

        // Update UI
        if (stageTimerText != null)
        {
            stageTimerText.text = $"{LocalizationManager.Instance.Get("STAGE")} {currentStage} \n {remainingTime:F0}s";}

        // Kiểm tra hết stage (chỉ dựa vào thời gian spawn thực tế)
        if (stageSpawnTime >= stageDuration)
        {
            if (!spawner.IsBossAlive())
            {
                CompleteStage();
                OnStageEnd?.Invoke(currentStage); 
            }
            else
            {
                //Tắt spawn trong khi boss còn sống
                isSpawning = false;
                spawner.StopSpawning(); 
                FindAnyObjectByType<CoinPool>()?.FlyAllCoins(currentStage);
            }
            
        }
    }

    private void CompleteStage()
    {
        // Dừng spawn và đếm thời gian
        isSpawning = false;
        
        // Thông báo hoàn thành stage
        UIEventManager.StageCompleted(currentStage);

        // Clear tất cả enemies hiện tại
        if (spawner != null)
        {
            spawner.StopSpawning();
            spawner.ClearAllEnemies();
        }

        if (currentStage == STAGE_TO_WIN)
        {
            UIEventManager.WinGame();
            return;
        }
        if(currentStage % 5 == 0)
        {
            stageDuration += 10f; // Tăng thời gian stage thêm 10 giây mỗi 5 stage
        }


        // Reset timer
        stageSpawnTime = 0f;

        // Pause timer during transition
        isPlaying = false;


        Invoke(nameof(ShowShop), 1.5f);
       
    }

    private void ShowShop()
    {
        if (shopUI != null)
        {
            shopUI.SetActive(true);
        }
        if (hudUI != null)
        {
            hudUI.SetActive(false);
        }

        Time.timeScale = 0f; 
    }

    // Call this method from ShopUI when player finishes shopping
    public void OnShopClosed()
    {
        Time.timeScale = 1f;
        hudUI.SetActive(true);
        StartNextStageTransition();
    }

    private void StartNextStageTransition()
    {
        currentStage++;
        OnStageStart?.Invoke(currentStage) ; 
        ApplyStage(currentStage);
    }

    void ApplyStage(int stage)
    {
        if (spawner != null)
        {

            spawner.SetStage(stage);
            
            // For stage 1, start immediately
            if (stage == 1)
            {
                spawner.StartSpawning();
                isSpawning = true;
                isPlaying = true;
            }
            
        }
    }

    // Called by EnemySpawner after transition delay completes (6 seconds)
    public void OnStageTransitionComplete()
    {
        isPlaying = true;
        isSpawning = true;
    }

    public float GetTimeSurvived()
    {
        return playedTime;
    }

    // Public methods để control từ UI
    public void PauseStage()
    {
        isPlaying = false;
        isSpawning = false;
        if (spawner != null)
        {
            spawner.StopSpawning();
        }
    }

    public void ResumeStage()
    {
        isPlaying = true;
        isSpawning = true;
        if (spawner != null)
        {
            spawner.StartSpawning();
        }
    }

    public int GetCurrentStage() => currentStage;
    public float GetRemainingTime() => Mathf.Max(0f, stageDuration - stageSpawnTime);
    public int GetCurrentEnemyCount() => spawner != null ? spawner.GetCurrentEnemyCount() : 0;

    public int GetPhase() => (GetCurrentStage() - 1) / 5 + 1;
}