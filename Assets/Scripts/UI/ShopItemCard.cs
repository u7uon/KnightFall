using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemCard : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Image frame;

    //Shop Item Frames
    [SerializeField] private Sprite commonFrame;
    [SerializeField] private Sprite uncommonFrame;
    [SerializeField] private Sprite rareFrame;
    [SerializeField] private Sprite epicFrame;
    [SerializeField] private Sprite legendFrame;
    [SerializeField] private Button Buybutton;
    [SerializeField] private Button lockBtn;
    
    private bool isLocked = false;
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite unlockedSprite; 

    private int cost;
    public object currentInfo;

    public static event Action<List<WeaponStats>> onWeaponChange;
    public static event Action<ItemData> onItemChange; 
    
    // ✅ Cache references
    private PlayerInventory _playerInventory;
    private WeaponManager _weaponManager;
    private ShopManager _shopManager;
    private LocalizationManager _localization;

    // ✅ Properties với lazy loading
    private PlayerInventory PlayerInventory
    {
        get
        {
            if (_playerInventory == null)
                _playerInventory = FindAnyObjectByType<PlayerInventory>();
            return _playerInventory;
        }
    }

    private WeaponManager WeaponManager
    {
        get
        {
            if (_weaponManager == null)
                _weaponManager = FindAnyObjectByType<WeaponManager>();
            return _weaponManager;
        }
    }

    private ShopManager ShopManager
    {
        get
        {
            if (_shopManager == null)
                _shopManager = FindAnyObjectByType<ShopManager>();
            return _shopManager;
        }
    }

    private LocalizationManager Localization
    {
        get
        {
            if (_localization == null)
                _localization = LocalizationManager.Instance;
            return _localization;
        }
    }

    void Awake()
    {
        // ✅ CHỈ đăng ký sự kiện một lần
        PlayerInventory.OnGoldChanged += HandleBuyButton;
        // ✅ CHỈ setup button listener
        if (lockBtn != null)
            lockBtn.onClick.AddListener(ToggleLock);
    }

    void OnEnable()
    {
        if (Localization != null)
            Localization.OnLanguageChanged += RefreshDisplay;
    }

    void OnDisable()
    {
        if (Localization != null)
            Localization.OnLanguageChanged -= RefreshDisplay;
    }
    public bool IsLocked() => isLocked;
    private void HandleBuyButton(int coin)
    {
        if (Buybutton == null || priceText == null) return;
        
        Buybutton.enabled = coin > cost;
        priceText.color = coin > cost ? Color.black : Color.red;
    }

    private void RefreshDisplay()
    {
        if (currentInfo != null)
            Setup(currentInfo);
    }

    public void Setup(object info)
    {
        currentInfo = info;

        if (info is WeaponStats data)
        {
            SetupWeapon(data);
        }
        else if (info is ItemData itemData)
        {
            SetupItem(itemData);
        }
        
        gameObject.SetActive(true);
    }

    void OnDestroy()
    {
        PlayerInventory.OnGoldChanged -= HandleBuyButton;
    }

    private void SetupWeapon(WeaponStats data)
    {
        if (data == null) return;

        Buybutton.onClick.RemoveAllListeners();
        Buybutton.onClick.AddListener(() => BuyWeapon(data));

        if (frame != null)
            frame.sprite = GetRarityFrame(data.Rarity);

        if (iconImage != null)
        {
            iconImage.sprite = data.Icon;
            iconImage.preserveAspect = false;
        }

        if (nameText != null)
        {
            nameText.text = Localization != null 
                ? Localization.Get(data.Name) 
                : data.Name;
                
            if (UnityEngine.ColorUtility.TryParseHtmlString(GetRarityColor(data.Rarity), out Color col))
                nameText.color = col;
        }

        if (typeText != null)
        {
            typeText.text = Localization != null 
                ? Localization.Get("WEAPON") 
                : "WEAPON";
        }

        if (statsText != null)
            statsText.text = BuildStatsText(data);

        if (priceText != null && PlayerInventory != null)
        {
            priceText.text = data.Cost.ToString();
            priceText.color = data.Cost < PlayerInventory.GetBalance() ? Color.black : Color.red;
        }
        
        if (Buybutton != null && PlayerInventory != null)
        {
            Buybutton.enabled = data.Cost < PlayerInventory.GetBalance();
            cost = data.Cost;
        }
    }

    private void SetupItem(ItemData itemData)
    {
        if (itemData == null) return;

        Buybutton.onClick.RemoveAllListeners();
        Buybutton.onClick.AddListener(() => BuyItem(itemData));
        
        if (frame != null)
            frame.sprite = GetRarityFrame(itemData.Rarity);

        if (iconImage != null)
        {
            iconImage.sprite = itemData.Icon;
            iconImage.preserveAspect = true;
        }

        if (nameText != null)
        {
            nameText.text = Localization != null 
                ? Localization.Get(itemData.Name) 
                : itemData.Name;
                
            if (UnityEngine.ColorUtility.TryParseHtmlString(GetRarityColor(itemData.Rarity), out Color col))
                nameText.color = col;
        }

        if (typeText != null)
        {
            typeText.text = Localization != null 
                ? Localization.Get("ITEM") 
                : "ITEM";
        }

        if (statsText != null)
            statsText.text = BuildItemStatsText(itemData);

        if (priceText != null && PlayerInventory != null)
        {
            priceText.text = itemData.Price.ToString();
            priceText.color = itemData.Price < PlayerInventory.GetBalance() ? Color.black : Color.red;
        }
        
        if (Buybutton != null && PlayerInventory != null)
        {
            Buybutton.enabled = itemData.Price < PlayerInventory.GetBalance();
            cost = itemData.Price;
        }
    }

    private void BuyWeapon(WeaponStats item)
    {
        if (PlayerInventory == null || item == null) return;
        
        if (item.Cost > PlayerInventory.GetBalance())
            return;

        if (PlayerInventory.AddWeapon(item))
        {
            PlayerInventory.Spend(item.Cost);
            onWeaponChange?.Invoke(PlayerInventory.GetWeapons()); 
            ForceUnlock();
            gameObject.SetActive(false); 
 
        }
    }

    private void BuyItem(ItemData item)
    {
        if (PlayerInventory == null || item == null) return;
        
        if (item.Price > PlayerInventory.GetBalance())
            return;

        PlayerInventory.Spend(item.Price);
        PlayerInventory.AddItem(item);
        onItemChange?.Invoke(item);
        ForceUnlock() ;
        gameObject.SetActive(false);
    }
    
    public void HideCard()
    {
       gameObject.SetActive(false);
    }   

    private string BuildStatsText(WeaponStats data)
    {
        if (data?.Buffs == null) return "";
        
        StringBuilder s = new StringBuilder();
        s.AppendLine(Localization.Get("DAMAGE") + ": " +  data.Damage );
        foreach (var stat in data.Buffs)
        {
            string color = stat.Value >= 0 ? "#00FF00" : "#FF0000";
            string valueText;

            if (stat.BuffType == BuffType.Flat)
                valueText = $"{stat.Value}";
            else
                valueText = $"{stat.Value * 100 }%";

            string statName = Localization != null 
                ? Localization.Get(GetStatDisplayName(stat.Type))
                : GetStatDisplayName(stat.Type);
                
            s.AppendLine($"<color={color}>{valueText}</color> {statName}");
        }

        if(data.specialEffect != null && !string.IsNullOrEmpty(data.specialEffect.EffectName) && 
            !string.IsNullOrEmpty(data.specialEffect.Description))
        { 
            s.AppendLine( $"<color=#FF0000>{Localization.Get(data.specialEffect.EffectName)} </color> "  + ": " + Localization.Get(data.specialEffect.Description) ); 

        }
        
        return s.ToString();
    }

    private string BuildItemStatsText(ItemData data)
    {
        if (data == null) return "";
        
        StringBuilder s = new StringBuilder();
        
        if (data.StatModifiers != null)
        {
            foreach (var stat in data.StatModifiers)
            {
                string color = stat.Value >= 0 ? "#00FF00" : "#FF0000";
                string valueText;

               if (stat.BuffType == BuffType.Flat)
                valueText = $"{stat.Value}";
                else
                valueText = $"{stat.Value * 100 }%";


                string statName = Localization != null 
                    ? Localization.Get(GetStatDisplayName(stat.Type))
                    : GetStatDisplayName(stat.Type);
                    
                s.AppendLine($"<color={color}>{valueText}</color> {statName}");
            }
            
        }

        if (data.SpecialEffect != null && 
            !string.IsNullOrEmpty(data.SpecialEffect.EffectName) && 
            !string.IsNullOrEmpty(data.SpecialEffect.Description))
        {
            s.AppendLine($"<color=#FF0000>{Localization.Get(data.SpecialEffect.EffectName)} </color>: {data.SpecialEffect.Description}");
        }

        return s.ToString();
    }

    public string GetStatDisplayName(StatType stat)
    {
        switch (stat)
        {
            case StatType.MaxHealth: return "STAT_MAXHEALTH";
            case StatType.DamageMultiplier: return "STAT_DAMAGEMULTIPLIER";
            case StatType.AttackSpeed: return "STAT_ATTACKSPEED";
            case StatType.CriticalChance: return "STAT_CRITICALCHANCE";
            case StatType.CriticalDamage: return "STAT_CRITICALDAMAGE";
            case StatType.LifeSteal: return "STAT_LIFESTEAL";
            case StatType.Armor: return "STAT_ARMOR";
            case StatType.MagicResist: return "STAT_MAGICRESIST";
            case StatType.HealthRegen: return "STAT_HEALTHREGEN";
            case StatType.DodgeChance: return "STAT_DODGECHANCE";
            case StatType.MoveSpeed: return "STAT_MOVESPEED";
            case StatType.Luck: return "STAT_LUCK";
            case StatType.PickupRange: return "STAT_PICKUPRANGE";
            case StatType.ExpMultiplier: return "STAT_EXPMULTIPLIER";
            case StatType.GoldMultiplier: return "STAT_GOLDMULTIPLIER";
            default: return "STAT_" + stat.ToString().ToUpper();
        }
    }

    private string GetRarityColor(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common: return "#9a9c9eff";
            case Rarity.Uncommon: return "#0ce30cff";
            case Rarity.Rare: return "#2f00ffff";
            case Rarity.Epic: return "#9e06b9ff";
            case Rarity.Legendary: return "#ff0000ff";
            default: return "#FFFFFF";
        }
    }

    private Sprite GetRarityFrame(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common: return commonFrame;
            case Rarity.Uncommon: return uncommonFrame;
            case Rarity.Rare: return rareFrame;
            case Rarity.Epic: return epicFrame;
            case Rarity.Legendary: return legendFrame;
            default: return commonFrame;
        }
    }

    private void ToggleLock()
    {
        isLocked = !isLocked;
        if (lockBtn != null && lockBtn.image != null)
            lockBtn.image.sprite = isLocked ? lockedSprite : unlockedSprite;
    }

    public void ForceUnlock()
    {
        isLocked = false;
        if (lockBtn != null && lockBtn.image != null)
            lockBtn.image.sprite = unlockedSprite;
    }
}