using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    public static  PlayerStatsManager Instance { get; private set; }

    [SerializeField] private  PlayerStats PlayerClassBaseStats;

    const float regenInterval = 1; 
    float regenTimer = 0f;

    private bool isDied = false ; 

    private InGamePlayerStat playerStats;

    // ====== STAT CAPS ======
    private const float MAX_CRIT_CHANCE = 1f;        // 100%
    private const float MAX_DODGE_CHANCE = 0.4f;       // 40%
    private const float MAX_LIFE_STEAL = 0.75f;        // 75%
    private const float MAX_ATTACK_SPEED = 2.5f;       // 2.5x
    private const float MIN_ATTACK_SPEED = 0.5f;       // 0.5x
    private const float MAX_DAMAGE_MULTIPLIER = 3.0f;  // 300%
    private const float MAX_CRIT_DAMAGE = 3.5f;        // 350%
    private const float MAX_MOVE_SPEED = 8.0f;
    private const float MIN_MOVE_SPEED = 2.0f;
    private const float MAX_EXP_MULTIPLIER = 2.5f;
    private const float MAX_GOLD_MULTIPLIER = 3.0f;
    private const float MAX_PICKUP_RANGE = 3f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        SetDefaultStats();
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;

        PlayerLevel.OnlevelUp -= LevelUp;
        StageManager.OnStageEnd -= OnStageEnd;
    }

    void OnDisable()
    {
        PlayerLevel.OnlevelUp -= LevelUp;
        StageManager.OnStageEnd -= OnStageEnd;
    }

    private float Health;

    public static event Action<float, float> OnHealthChanged;

    public static event Action OnPlayerDie; 
    public static event Action<StatType, float, float> OnStatChange;

    void Start()
    {
        PlayerLevel.OnlevelUp += LevelUp;
        StageManager.OnStageEnd += OnStageEnd;
        Health = playerStats.MaxHealth; // Set full HP at start
        OnHealthChanged?.Invoke(Health, playerStats.MaxHealth);
    }

    void Update()
    {
       HpAutoRegen(); 
    }

    void HpAutoRegen()
    {
         regenTimer += Time.deltaTime;
        if (regenTimer >= regenInterval)
        {
            HealthUp(playerStats.HealthRegen);
            regenTimer = 0f;
        }
    }

    public void TakeDame(float damage)
    {   
        if(isDied) return; 

        if (damage <= 0) return;

        // Dodge check
        if (UnityEngine.Random.value < playerStats.DodgeChance)
        {
            DamagePopupPool.Instance.Spawn(transform.position, 0 , Color.white);
            return;
        }

        // Apply armor reduction
        float finalDamage = damage * (100 / (100 + playerStats.Armor + (playerStats.MagicResist/ 2)));
        finalDamage = Math.Max(damage * 0.2f, finalDamage ); 
        Health -= finalDamage;
        Health = Mathf.Max(Health, 0);

        OnHealthChanged?.Invoke(Health, playerStats.MaxHealth);
        DamagePopupPool.Instance.Spawn(transform.position,finalDamage, Color.red);

        if (Health <= 0)
            Die();
    }

    private void OnStageEnd(int i)
    {
        //Đặt máu về 75% máu tối đa nếu thấp hơn 
        if( Health < playerStats.MaxHealth *0.75f)
        {
            Health = playerStats.MaxHealth *0.75f ; 
            OnHealthChanged?.Invoke(Health, playerStats.MaxHealth);
        }
    }

    private void Die()
    {   
        isDied=true ; 

        OnPlayerDie?.Invoke() ; 

        this.gameObject.SetActive(false);
    }

    public void HealthUp(float Value)
    {
        Health = Mathf.Min(Health + Value, playerStats.MaxHealth);
        OnHealthChanged?.Invoke(Health, playerStats.MaxHealth);
    }

    private void SetDefaultStats()
    {
        playerStats = new InGamePlayerStat
        {
            damageMultiplier = PlayerClassBaseStats.damageMultiplier,
            AttackSpeed = PlayerClassBaseStats.AttackSpeed,
            CriticalChance = PlayerClassBaseStats.CriticalChance,
            CriticalDamage = PlayerClassBaseStats.CriticalDamage,
            LifeSteal = PlayerClassBaseStats.LifeSteal,
            Armor = PlayerClassBaseStats.Armor,
            MagicResist = PlayerClassBaseStats.MagicResist,
            MaxHealth = PlayerClassBaseStats.MaxHealth,
            HealthRegen = PlayerClassBaseStats.HealthRegen,
            DodgeChance = PlayerClassBaseStats.DodgeChance,
            MoveSpeed = PlayerClassBaseStats.MoveSpeed,
            Luck = PlayerClassBaseStats.Luck,
            PickupRange = PlayerClassBaseStats.PickupRange,
            ExpMultiplier = PlayerClassBaseStats.ExpMultiplier,
            GoldMultiplier = PlayerClassBaseStats.GoldMultiplier,
            Class = PlayerClassBaseStats.Class
        };
    }

    public void BuffStat(List<StatBuff> stats)
    {
        if (stats == null)
            return;

        foreach (var stat in stats)
        {
            ApplyModifier(stat);
        }
    }

    public void DeBuffStat(List<StatBuff> stats)
    {
        if (stats == null)
            return;

        foreach (var stat in stats)
        {
            var debuff = new StatBuff
            {
                Type = stat.Type,
                Value = -stat.Value,
                BuffType = stat.BuffType
            };
            ApplyModifier(debuff);
        }
    }

    public void LevelUp(int lv)
    {
        float oldMaxHealth = playerStats.MaxHealth;

        switch (playerStats.Class)
        {
            case PlayerClass.Swordsman:
                playerStats.MaxHealth += 1f ;
                playerStats.damageMultiplier += 0.01f  ;
                playerStats.Armor += 0.5f ;
                playerStats.MagicResist += 0.3f;
                if (lv % 5 == 0) playerStats.CriticalChance += 0.005f; // Mỗi 5 level
                break;

            case PlayerClass.Assassin:
                playerStats.MaxHealth += 0.5f ;
                playerStats.CriticalChance += 0.005f ;
                playerStats.CriticalDamage += 0.01f ;
                playerStats.MoveSpeed += 0.01f ;
                playerStats.DodgeChance += 0.002f ;
                break;

            case PlayerClass.Archer:
                playerStats.MaxHealth += 0.5f;
                playerStats.AttackSpeed += 0.025f;
                playerStats.MoveSpeed += 0.05f;
                if (lv % 3 == 0) playerStats.CriticalChance += 0.005f;
                if (lv % 3 == 0) playerStats.PickupRange += 0.01f;
                break;
        }

        // Apply caps sau khi level up
        ApplyCaps();

        // Heal theo % HP tăng thêm
        float healthGained = playerStats.MaxHealth - oldMaxHealth;
        Health += healthGained;
        Health = Mathf.Min(Health, playerStats.MaxHealth);

        OnHealthChanged?.Invoke(Health, playerStats.MaxHealth);
    }

    public void ApplyModifier(StatBuff modifier)
    {
        if (modifier == null) return;

        float finalValue = modifier.Value;

        // ✅ Calculate percentage-based buffs
        if (modifier.BuffType == BuffType.Percentage)
        {
            switch (modifier.Type)
            {
                case StatType.MaxHealth:
                    finalValue = PlayerClassBaseStats.MaxHealth * modifier.Value;
                    break;
                case StatType.Armor:
                    finalValue = PlayerClassBaseStats.Armor * modifier.Value;
                    break;
                case StatType.MagicResist:
                    finalValue = PlayerClassBaseStats.MagicResist * modifier.Value;
                    break;
                case StatType.HealthRegen:
                    finalValue = PlayerClassBaseStats.HealthRegen * modifier.Value;
                    break;
                case StatType.PickupRange:
                    finalValue = PlayerClassBaseStats.PickupRange * modifier.Value;
                    break;
                default:
                    finalValue = modifier.Value;
                    break;
            }
        }

        // Helper to get base value from PlayerClassBaseStats
        float GetBaseValue(StatType type)
        {
            switch (type)
            {
                case StatType.DamageMultiplier: return PlayerClassBaseStats.damageMultiplier;
                case StatType.AttackSpeed: return PlayerClassBaseStats.AttackSpeed;
                case StatType.CriticalChance: return PlayerClassBaseStats.CriticalChance;
                case StatType.CriticalDamage: return PlayerClassBaseStats.CriticalDamage;
                case StatType.LifeSteal: return PlayerClassBaseStats.LifeSteal;
                case StatType.Armor: return PlayerClassBaseStats.Armor;
                case StatType.MagicResist: return PlayerClassBaseStats.MagicResist;
                case StatType.MaxHealth: return PlayerClassBaseStats.MaxHealth;
                case StatType.HealthRegen: return PlayerClassBaseStats.HealthRegen;
                case StatType.DodgeChance: return PlayerClassBaseStats.DodgeChance;
                case StatType.MoveSpeed: return PlayerClassBaseStats.MoveSpeed;
                case StatType.Luck: return PlayerClassBaseStats.Luck;
                case StatType.PickupRange: return PlayerClassBaseStats.PickupRange;
                case StatType.ExpMultiplier: return PlayerClassBaseStats.ExpMultiplier;
                case StatType.GoldMultiplier: return PlayerClassBaseStats.GoldMultiplier;
                default: return 0f;
            }
        }

        float baseValue = GetBaseValue(modifier.Type);

        // Apply the buff
        switch (modifier.Type)
        {
            case StatType.DamageMultiplier:
                playerStats.damageMultiplier += finalValue;
                playerStats.damageMultiplier = Mathf.Clamp(playerStats.damageMultiplier, 0.1f, 3.0f);
                OnStatChange?.Invoke(modifier.Type, baseValue, playerStats.damageMultiplier);
                break;

            case StatType.AttackSpeed:
                playerStats.AttackSpeed += finalValue;
                playerStats.AttackSpeed = Mathf.Clamp(playerStats.AttackSpeed, 0.5f, 2.5f);
                OnStatChange?.Invoke(modifier.Type, baseValue, playerStats.AttackSpeed);
                break;

            case StatType.CriticalChance:
                playerStats.CriticalChance += finalValue;
                playerStats.CriticalChance = Mathf.Clamp(playerStats.CriticalChance, 0f, 1.0f);
                OnStatChange?.Invoke(modifier.Type, baseValue, playerStats.CriticalChance);
                break;

            case StatType.CriticalDamage:
                playerStats.CriticalDamage += finalValue;
                playerStats.CriticalDamage = Mathf.Clamp(playerStats.CriticalDamage, 1.0f, 3.5f);
                OnStatChange?.Invoke(modifier.Type, baseValue, playerStats.CriticalDamage);
                break;

            case StatType.LifeSteal:
                playerStats.LifeSteal += finalValue;
                playerStats.LifeSteal = Mathf.Clamp(playerStats.LifeSteal, 0f, 0.75f);
                OnStatChange?.Invoke(modifier.Type, baseValue, playerStats.LifeSteal);
                break;

            case StatType.Armor:
                playerStats.Armor += finalValue;
                playerStats.Armor = Mathf.Max(playerStats.Armor, 0f);
                OnStatChange?.Invoke(modifier.Type, baseValue, playerStats.Armor);
                break;

            case StatType.MagicResist:
                playerStats.MagicResist += finalValue;
                playerStats.MagicResist = Mathf.Max(playerStats.MagicResist, 0f);
                OnStatChange?.Invoke(modifier.Type, baseValue, playerStats.MagicResist);
                break;

            case StatType.MaxHealth:
                playerStats.MaxHealth += finalValue;
                playerStats.MaxHealth = Mathf.Max(playerStats.MaxHealth, 10f);
                OnStatChange?.Invoke(modifier.Type, baseValue, playerStats.MaxHealth);
                //Hiển thị số máu tăng thêm ngay lập tức
                OnHealthChanged?.Invoke(Health, playerStats.MaxHealth + finalValue);
                break;

            case StatType.HealthRegen:
                playerStats.HealthRegen += finalValue;
                playerStats.HealthRegen = Mathf.Max(playerStats.HealthRegen, 0f);
                OnStatChange?.Invoke(modifier.Type, baseValue, playerStats.HealthRegen);
                break;

            case StatType.DodgeChance:
                playerStats.DodgeChance += finalValue;
                playerStats.DodgeChance = Mathf.Clamp(playerStats.DodgeChance, 0f, 0.4f);
                OnStatChange?.Invoke(modifier.Type, baseValue, playerStats.DodgeChance);
                break;

            case StatType.MoveSpeed:
                playerStats.MoveSpeed += finalValue;
                playerStats.MoveSpeed = Mathf.Clamp(playerStats.MoveSpeed, 2.0f, 8.0f);
                OnStatChange?.Invoke(modifier.Type, baseValue, playerStats.MoveSpeed);
                break;

            case StatType.Luck:
                playerStats.Luck += finalValue;
                playerStats.Luck = Mathf.Max(playerStats.Luck, 0f);
                OnStatChange?.Invoke(modifier.Type, baseValue, playerStats.Luck);
                break;

            case StatType.PickupRange:
                playerStats.PickupRange += finalValue;
                playerStats.PickupRange = Mathf.Clamp(playerStats.PickupRange, 1f, 8.0f);
                OnStatChange?.Invoke(modifier.Type, baseValue, playerStats.PickupRange);
                break;

            case StatType.ExpMultiplier:
                playerStats.ExpMultiplier += finalValue;
                playerStats.ExpMultiplier = Mathf.Clamp(playerStats.ExpMultiplier, 0.1f, 2.5f);
                OnStatChange?.Invoke(modifier.Type, baseValue, playerStats.ExpMultiplier);
                break;

            case StatType.GoldMultiplier:
                playerStats.GoldMultiplier += finalValue;
                playerStats.GoldMultiplier = Mathf.Clamp(playerStats.GoldMultiplier, 0.1f, 3.0f);
                OnStatChange?.Invoke(modifier.Type, baseValue, playerStats.GoldMultiplier);
                break;
        }

        ApplyCaps();
    }

    // Apply tất cả caps - gọi sau khi load game hoặc level up
    private void ApplyCaps()
    {
        playerStats.CriticalChance = Mathf.Clamp(playerStats.CriticalChance, 0f, MAX_CRIT_CHANCE);
        playerStats.DodgeChance = Mathf.Clamp(playerStats.DodgeChance, 0f, MAX_DODGE_CHANCE);
        playerStats.LifeSteal = Mathf.Clamp(playerStats.LifeSteal, 0f, MAX_LIFE_STEAL);
        playerStats.AttackSpeed = Mathf.Clamp(playerStats.AttackSpeed, MIN_ATTACK_SPEED, MAX_ATTACK_SPEED);
        playerStats.damageMultiplier = Mathf.Clamp(playerStats.damageMultiplier, 0.1f, MAX_DAMAGE_MULTIPLIER);
        playerStats.CriticalDamage = Mathf.Clamp(playerStats.CriticalDamage, 1.0f, MAX_CRIT_DAMAGE);
        playerStats.MoveSpeed = Mathf.Clamp(playerStats.MoveSpeed, MIN_MOVE_SPEED, MAX_MOVE_SPEED);
        playerStats.ExpMultiplier = Mathf.Clamp(playerStats.ExpMultiplier, 0.1f, MAX_EXP_MULTIPLIER);
        playerStats.GoldMultiplier = Mathf.Clamp(playerStats.GoldMultiplier, 0.1f, MAX_GOLD_MULTIPLIER);
        playerStats.PickupRange = Mathf.Clamp(playerStats.PickupRange, 1f, MAX_PICKUP_RANGE);


    }

    // ====== GETTERS ======
    public float GetPlayerDameMutilplier() => playerStats.damageMultiplier;
    public float GetPlayerSpeed() => playerStats.MoveSpeed;
    public float GetPlayerCriticialDame() => playerStats.CriticalDamage;
    public float GetPlayerDameCriticialChance() => playerStats.CriticalChance;
    public float GetExpMutiplier() => playerStats.ExpMultiplier;
    public float GetPickupRange() => playerStats.PickupRange;
    public float GetLifeSteal() => playerStats.LifeSteal;
    public PlayerStats GetBaseStat() => PlayerClassBaseStats;
    public InGamePlayerStat GetPlayerStats() => playerStats;
    public float GetCurrentHealth() => Health;
    public float GetArmor() => playerStats.Armor;
    public float GetAttackSpeed() => playerStats.AttackSpeed;
    public float GetCoinsMutil() => playerStats.GoldMultiplier; 
    public float GetPlayerLuck() => playerStats.Luck ; 

    public string GetPlayerClassName()
    {
        return  PlayerClassBaseStats.name ;
    }
}

public enum StatType
{
    DamageMultiplier,
    AttackSpeed,
    CriticalChance,
    CriticalDamage,
    LifeSteal,
    Armor,
    MagicResist,
    MaxHealth,
    HealthRegen,
    DodgeChance,
    MoveSpeed,
    Luck,
    PickupRange,
    ExpMultiplier,
    GoldMultiplier
}

[System.Serializable]
public class StatBuff
{
    public StatType Type;
    public float Value;
    public BuffType BuffType ;
}

public enum BuffType
{
    Percentage, Flat
}

[System.Serializable]
public class InGamePlayerStat
{
    [Header("⚔️ Combat Stats")]
    public float damageMultiplier ;          // Nhân sát thương vũ khí
    public float AttackSpeed ;               // Tốc độ tấn công
    public float CriticalChance ;         // % chí mạng
    public float CriticalDamage ;          // Hệ số chí mạng
    public float LifeSteal ;                 // % hút máu
    public float Armor ;                    // Giảm sát thương vật lý (đừng để 0)
    public float MagicResist ;               // Giảm sát thương phép

    [Header("❤️ Survival Stats")]
    public float MaxHealth ;
    public float HealthRegen ;               // HP/giây
    public float DodgeChance ;

    [Header("🏃 Utility Stats")]
    public float MoveSpeed ;
    public float Luck ;
    public float PickupRange ;
    public float ExpMultiplier ;
    public float GoldMultiplier ;

    [Header("🎭 Class Info")]
    public PlayerClass Class;
}