using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class WeaponDialog : MonoBehaviour
{
    // Singleton instance
    private static WeaponDialog _instance;
    public static WeaponDialog Instance
    {
        get
        {
            if (_instance == null)
            {
                return null;
            }
            return _instance;
        }
    }

    [Header("UI References")]
    [SerializeField] private GameObject dialogPanel; // NEW: Reference to the dialog panel
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Image frame;
    [SerializeField] private Button sellButton;
    [SerializeField] private Button closeButton;

    [Header("Rarity Frames")]
    [SerializeField] private Sprite commonFrame;
    [SerializeField] private Sprite uncommonFrame;
    [SerializeField] private Sprite rareFrame;
    [SerializeField] private Sprite epicFrame;
    [SerializeField] private Sprite legendFrame;

    private PlayerInventory _playerInventory;
    private LocalizationManager _localization;
    private WeaponStats currentWeaponData;

    void Awake()
    {
        // Singleton setup - KEEP GAMEOBJECT ACTIVE
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        // Initialize references
        _playerInventory = FindAnyObjectByType<PlayerInventory>();
        _localization = LocalizationManager.Instance;

        // Setup close button if exists
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Close);
        }

        // Hide the dialog panel instead of the entire GameObject
        if (dialogPanel != null)
        {
            dialogPanel.SetActive(false);
        }
    }

    void OnDestroy()
    {
        // Clear singleton reference
        if (_instance == this)
        {
            _instance = null;
        }

        // Cleanup
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
        }
    }

    public void Open(WeaponStats stats)
    {
        if (stats == null)
        {
            return;
        }

        currentWeaponData = stats;

        // Setup sell button
        sellButton.onClick.RemoveAllListeners();
        sellButton.onClick.AddListener(() => Sell(stats));

        // Update UI
        frame.sprite = GetRarityFrame(stats.Rarity);
        iconImage.sprite = stats.Icon;

        if (nameText != null)
        {
            nameText.text = _localization.Get(stats.Name);
            if (ColorUtility.TryParseHtmlString(GetRarityColor(stats.Rarity), out Color col))
                nameText.color = col;
        }

        typeText?.SetText(_localization.Get("WEAPON"));
        statsText?.SetText(BuildStatsText(stats));
        priceText?.SetText($"{GetSellPrice(stats.Cost)} {_localization.Get("SELL")}");

        // Show dialog panel
        if (dialogPanel != null)
        {
            dialogPanel.SetActive(true);
        }
    }

    public void Close()
    {
        currentWeaponData = null;
        
        // Hide dialog panel
        if (dialogPanel != null)
        {
            dialogPanel.SetActive(false);
        }
        
    }

    private void Sell(WeaponStats data)
    {
        if (_playerInventory == null)
        {
            return;
        }

        if (_playerInventory.Sell(data))
        {
            Close();
        }

    }

    private string BuildStatsText(WeaponStats data)
    {
        StringBuilder s = new StringBuilder();
        s.AppendLine(_localization.Get("DAMAGE") + ": " +  data.Damage); 

        if (data.Buffs != null)
        {
            foreach (var stat in data.Buffs)
            {
                string color = stat.Value >= 0 ? "#00FF00" : "#FF0000";
                string valueText = "" ;
                if (stat.BuffType == BuffType.Flat)
                valueText = $"{stat.Value}";
                else
                valueText = $"{stat.Value * 100 }%";


                string statName = _localization.Get(GetStatDisplayName(stat.Type));
                s.AppendLine($"<color={color}>{valueText}</color> {statName}");
            }

            if(data.specialEffect != null && !string.IsNullOrEmpty(data.specialEffect.EffectName) && 
            !string.IsNullOrEmpty(data.specialEffect.Description) ) 
            {
                 s.AppendLine( $"<color=#FF0000>{_localization.Get(data.specialEffect.EffectName)} </color> "  + ": " + _localization.Get(data.specialEffect.Description) ); 
            }

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

    private int GetSellPrice(int price) => (int)(price * 0.7f);

    private string GetRarityColor(Rarity rarity)
    {
        return rarity switch
        {
            Rarity.Common => "#9a9c9eff",
            Rarity.Uncommon => "#0ce30cff",
            Rarity.Rare => "#2f00ffff",
            Rarity.Epic => "#9e06b9ff",
            Rarity.Legendary => "#ff0000ff",
            _ => "#FFFFFF"
        };
    }

    private Sprite GetRarityFrame(Rarity rarity)
    {
        return rarity switch
        {
            Rarity.Common => commonFrame,
            Rarity.Uncommon => uncommonFrame,
            Rarity.Rare => rareFrame,
            Rarity.Epic => epicFrame,
            Rarity.Legendary => legendFrame,
            _ => commonFrame
        };
    }

    // Public helpers
    public bool IsOpen() => dialogPanel != null ? dialogPanel.activeSelf : false;
    public WeaponStats GetCurrentWeapon() => currentWeaponData;
}