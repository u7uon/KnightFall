using System;
using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    public int level = 1;
    public float currentXp = 0;
    public float xpToNext => CalcXpToLevel(level);

    // ===== XP BALANCE FOR 30 ROUNDS =====
    // Goal: Reach level 25-30 by round 30
    // Average 1 level per round early, slower late
    
    private const float baseXP = 10f;
    private const float levelScaling = 15f;      // Tăng tuyến tính
    private const float exponentialFactor = 0.3f; // Thêm curve nhẹ

    public static event Action<int> OnlevelUp;
    public static event Action<float, float> OnExpChange;

    /// <summary>
    /// Công thức XP cân bằng cho 30 rounds
    /// Level 1→2: 20 XP
    /// Level 10→11: 110 XP
    /// Level 20→21: 230 XP
    /// Level 30→31: 370 XP
    /// </summary>
    float CalcXpToLevel(int lvl)
    {
        // Linear + slight exponential
        float linear = baseXP + (lvl - 1) * levelScaling;
        float exponential = exponentialFactor * Mathf.Pow(lvl - 1, 1.5f);
        return Mathf.Round(linear + exponential);
    }

    public void AddXp(float amount)
    {
        if (amount <= 0) return;

        float finalXp = amount * PlayerStatsManager.Instance.GetExpMutiplier() ;
        currentXp += finalXp;

        OnExpChange?.Invoke(currentXp, xpToNext);

        // Check level up (có thể level nhiều lần nếu XP đủ lớn)
        while (currentXp >= xpToNext)
        {
            currentXp -= xpToNext;
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        OnlevelUp?.Invoke(level);
        UIEventManager.LevelUp(level);

        // Update UI với XP mới
        OnExpChange?.Invoke(currentXp, xpToNext);
    
    }

    public int GetCurrentLevel() => level;
    public float GetCurrentXp() => currentXp;
    public float GetXpProgress() => currentXp / xpToNext; // 0.0 → 1.0
    public float GetXpProgressPercent() => (currentXp / xpToNext) * 100f; // 0 → 100

}