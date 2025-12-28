using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    #region UIReferences 
    [SerializeField] private TextMeshProUGUI balanceText;
    [SerializeField] private TextMeshProUGUI rerollCostText; // Add this to show cost
    [SerializeField] private List<ShopItemCard> itemCards;


    public static event Action<float, float, float, float, float> onRatioChange;
    public static event Action<List<WeaponStats>> onReloadWeapon;
    #endregion

    #region ComponentReferences
    private PlayerInventory _playerInventory;
    private WeaponManager _weaponManager;
    private StageManager _stageManager;

    [SerializeField] private WeaponDatabase weaponDB;
    [SerializeField] private ItemDb ItemDb;
    [SerializeField] private Button refreshButton;
    [SerializeField] private Button nextStageButton; 
    [SerializeField] private TextMeshProUGUI nextStageButtonText;
    #endregion

    [Header("Reroll Settings")]
    [SerializeField] private int baseRerollCost = 5;
    [SerializeField] private int incrementPerReroll = 5;
    [SerializeField] private int maxRerollCost = 50;
    
    private int currentRerollCost;
    private int rerollCount = 0;

    [Header("Rarity chances (%)")]
    public float commonChance = 55f;
    public float unCommonChance = 45f;
    public float rareChance = 0f;
    public float epicChance = 0f;
    public float legendaryChance = 0f;

    void Awake()
    {
        _stageManager = FindAnyObjectByType<StageManager>();
        _playerInventory = FindAnyObjectByType<PlayerInventory>();
        _weaponManager = FindAnyObjectByType<WeaponManager>();
        
        
        refreshButton.onClick.AddListener(TryReroll);
        nextStageButton.onClick.AddListener(NextStage);
        PlayerInventory.OnGoldChanged += HandleOnCoinChange ; 
        StageManager.OnStageEnd += ModifiRollRatioByStage;
        PlayerLevel.OnlevelUp += ModifiRollRatioByPlayerLevel; 

    }

    void OnEnable()
    {
        nextStageButtonText.text = LocalizationManager.Instance.Get("STAGE") + " " + (_stageManager.GetCurrentStage() + 1) ;
        AudioManager.Instance.PlayBGM("shop");

        // Reset reroll cost when shop opens
        rerollCount = 0;
        currentRerollCost = baseRerollCost;
        
        // Update balance display
        if (balanceText != null)
        {

            balanceText.text = _playerInventory?.GetBalance().ToString();
        }
        
        // Notify listeners and refresh
        onReloadWeapon?.Invoke(_weaponManager.getCurrentWeapons());
        UpdateRatio();
        
        // Generate initial shop items (FREE - no cost for first view)
        GenerateShopItems();
        foreach (var c in itemCards)
        {
            if (c != null)
            {
                c.ForceUnlock();
            }
        }
        // Update reroll button display
        UpdateRerollButton();
    }

    void OnDisable()
    {
        StopAllCoroutines() ; 
        ClearShopItems();
        AudioManager.Instance.StopBGM();
    }

    void OnDestroy()
    {
        PlayerInventory.OnGoldChanged -= HandleOnCoinChange ; 
        PlayerLevel.OnlevelUp -= ModifiRollRatioByPlayerLevel; 
        StageManager.OnStageEnd -= ModifiRollRatioByStage;
        
    }

void HandleOnCoinChange(int balance)
    {
        refreshButton.enabled = currentRerollCost <= balance;
        rerollCostText.color = currentRerollCost <= balance ? Color.white : Color.red ; 
    }

    private void ClearShopItems()
    {
        foreach (var c in itemCards)
        {
            if (c != null)
            {
                //c.HideCard();
            }
        }
    }

    private void TryReroll()
    {
        if (_playerInventory == null)
        {
            return;
        }

        if (_playerInventory.GetBalance() < currentRerollCost)
        {
            return;
        }
        
        if(itemCards.Count( x => x.IsLocked()) >= itemCards.Count)
        {
            return;
        }
        
    
        // ✅ Trừ tiền
        _playerInventory.Spend(currentRerollCost);

        // ✅ Tăng giá reroll
        rerollCount++;

        if(rerollCount % 2 == 0)
        {
            int nextCost = baseRerollCost + ( rerollCount / 2 * incrementPerReroll);
            currentRerollCost = Mathf.Min(nextCost, maxRerollCost);
        }


        // ✅ Cập nhật UI
        balanceText.text = _playerInventory.GetBalance().ToString();
        UpdateRerollButton();


        // ✅ Sinh thêm các item mới để đủ 4 ô
        GenerateShopItems();

    }


    private void GenerateShopItems()
{
    //ClearShopItems();

    for (int i = 0; i < itemCards.Count; i++)
    {
        if (itemCards[i] != null && itemCards[i].IsLocked())
        {
            continue; // Giữ nguyên item đã khóa
        }

        itemCards[i].gameObject.SetActive(true);

        Rarity rarity = RollRarity();

        if (UnityEngine.Random.value > 0.5f)
        {
            var randomWeapon = weaponDB.GetRandomWeaponByRarity(rarity);
            if (randomWeapon != null)
            {
                itemCards[i].Setup(randomWeapon);
            }
        }
        else
        {
            var randomItem = ItemDb.GetRandomWeaponByRarity(rarity);
            if (randomItem != null)
            {
                itemCards[i].Setup(randomItem);
            }
        }
    }

}

    private void UpdateRerollButton()
    {
        if (rerollCostText != null)
        {
            rerollCostText.text = currentRerollCost.ToString();
        }
        
        // Disable button if can't afford
        if (refreshButton != null && _playerInventory != null)
        {
            bool canAfford = _playerInventory.GetBalance() >= currentRerollCost;
            refreshButton.interactable = canAfford;
            
            // Optional: Change button text color based on affordability
            if (rerollCostText != null)
            {
                rerollCostText.color = canAfford ? Color.white : Color.red;
            }
        }
    }

    private Rarity RollRarity()
    {
        float roll = UnityEngine.Random.Range(0f, 100f);
        float cumulative = 0f;
        
        // Roll theo xác suất base (không điều chỉnh theo Luck)
        cumulative += legendaryChance;
        if (roll < cumulative) return Rarity.Legendary;
        
        cumulative += epicChance;
        if (roll < cumulative) return Rarity.Epic;
        
        cumulative += rareChance;
        if (roll < cumulative) return Rarity.Rare;
        
        cumulative += unCommonChance;
        if (roll < cumulative) return Rarity.Uncommon;
        
        return Rarity.Common;
    }

    void UpdateRatio()
    {
        onRatioChange?.Invoke(commonChance, unCommonChance, rareChance, epicChance, legendaryChance);
    }

    private void NextStage()
    {
        _stageManager.OnShopClosed();
        gameObject.SetActive(false);
    }

    void ModifiRollRatioByStage(int stage)
    {

        // Reset base
        commonChance = 80f;
        unCommonChance = 20f;
        rareChance = 0f;
        epicChance = 0f;
        legendaryChance = 0f;

        // Stage 1-5: Mostly common + uncommon
        // Stage 6-15: Rare slowly unlocks
        // Stage 16-25: Epic becomes significant
        // Stage 26-30: Legendary appears

        // 🟢 Common giảm dần (80% → 25%)
        commonChance = Mathf.Max(25f, 80f - (stage - 1) * 1.8f);

        // 🔵 Uncommon giữ tương đối ổn định (20% → 15%)
        unCommonChance = Mathf.Max(15f, 20f - (stage - 1) * 0.15f);

        // 🟣 Rare: unlock từ stage 5, tăng từ stage 10 (0% → 20%)
        if (stage >= 5)
        {
            if (stage < 10)
                rareChance = (stage - 4) * 1.2f;       // stage 5-9: từ 1.2% → 7.2%
            else
                rareChance = 7.2f + (stage - 10) * 1.8f; // stage 10-30: từ 7.2% → 43.2%
            rareChance = Mathf.Min(rareChance, 30f);
        }

        // 🟠 Epic: unlock từ stage 12 (0% → 25%)
        if (stage >= 12)
        {
            epicChance = (stage - 11) * 1.5f; // mỗi stage +1.5%
            epicChance = Mathf.Min(epicChance, 25f);
        }

        // 🔴 Legendary: unlock từ stage 20 (0% → 15%)
        if (stage >= 20)
        {
            legendaryChance = (stage - 19) * 0.75f; // mỗi stage +0.75%
            legendaryChance = Mathf.Min(legendaryChance, 15f);
        }

        // Đảm bảo tổng = 100%
        float total = commonChance + unCommonChance + rareChance + epicChance + legendaryChance;
        if (total > 100f)
        {
            // Giảm common trước tiên
            float excess = total - 100f;
            commonChance = Mathf.Max(0f, commonChance - excess);
        }
        else if (total < 100f)
        {
            // Thêm vào uncommon nếu thiếu
            float deficit = 100f - total;
            unCommonChance += deficit;
        }
        UpdateRatio();
    }

    void ModifiRollRatioByPlayerLevel(int lv)
    {
        if (lv >= 5)
            epicChance += (lv - 4) * 0.5f;       // +0.5% mỗi level sau level 5
        if (lv >= 10)
            legendaryChance += (lv - 9) * 0.25f; // +0.25% mỗi level sau level 10

        // --- Giới hạn để không vượt mức an toàn ---
        epicChance = Mathf.Min(epicChance, 15f);
        legendaryChance = Mathf.Min(legendaryChance, 10f);

        // --- Đảm bảo tổng = 100% ---
        float total = commonChance + unCommonChance + rareChance + epicChance + legendaryChance;
        if (total > 100f)
        {
            // Giảm common để cân bằng tổng
            commonChance = Mathf.Max(0f, commonChance - (total - 100f));
        }
        else if (total < 100f)
        {
            // Nếu tổng <100, thêm vào uncommon
            unCommonChance += (100f - total);
        }

        UpdateRatio();
    }


}