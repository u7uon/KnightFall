using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatPanel : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI damageMultiplier ;
    [SerializeField]private TextMeshProUGUI AttackSpeed ;
    [SerializeField]private TextMeshProUGUI CriticalChance;
    [SerializeField]private TextMeshProUGUI CriticalDamage ;
    [SerializeField]private TextMeshProUGUI LifeSteal ;
    [SerializeField]private TextMeshProUGUI Armor ;
    [SerializeField]private TextMeshProUGUI MagicResist ;
    [SerializeField]private TextMeshProUGUI MaxHealth ;
    [SerializeField]private TextMeshProUGUI HealthRegen ;
    [SerializeField]private TextMeshProUGUI DodgeChance ;
    [SerializeField]private TextMeshProUGUI MoveSpeed ;
    [SerializeField]private TextMeshProUGUI Luck ;
    [SerializeField]private TextMeshProUGUI PickupRange ;
    [SerializeField]private TextMeshProUGUI ExpMultiplier ;
    [SerializeField]private TextMeshProUGUI GoldMultiplier; 

    [SerializeField] private Button slideBtn ; 
    [SerializeField]private Sprite openedSptite ; 
    [SerializeField] private Sprite closedSprite ; 
 
    [SerializeField] private RectTransform panel;
    private bool isOpen = false;
    private float panelWidth;


    void Awake()
    {
        panelWidth = panel.rect.width;
        slideBtn.onClick.AddListener(TogglePanel);

    }

    void OnEnable()
    {
        PlayerStatsManager.OnStatChange += UpdateStat ;  
        isOpen = false;
        slideBtn.image.sprite = closedSprite;
        // Set vị trí ban đầu ngay lập tức
        panel.anchoredPosition = new Vector2(-panelWidth, 0);
    }

    void OnDestroy()
    {
         PlayerStatsManager.OnStatChange -= UpdateStat ;  
    }


    public void Refresh()
    {
        DisplayPlayerStats() ; 
    }

    void DisplayPlayerStats()
    {
        var baseStat = PlayerStatsManager.Instance.GetBaseStat();
        var stat = PlayerStatsManager.Instance.GetPlayerStats();

        SetStat(damageMultiplier, baseStat.damageMultiplier, stat.damageMultiplier , true);
        SetStat(AttackSpeed, baseStat.AttackSpeed, stat.AttackSpeed , false);
        SetStat(CriticalChance, baseStat.CriticalChance, stat.CriticalChance , true);
        SetStat(CriticalDamage, baseStat.CriticalDamage, stat.CriticalDamage , true);
        SetStat(LifeSteal, baseStat.LifeSteal, stat.LifeSteal , true);
        SetStat(Armor, baseStat.Armor, stat.Armor , false);
        SetStat(MagicResist, baseStat.MagicResist, stat.MagicResist , false);
        SetStat(MaxHealth, baseStat.MaxHealth,stat.MaxHealth , false);
        SetStat(HealthRegen, baseStat.HealthRegen, stat.HealthRegen , false);
        SetStat(DodgeChance, baseStat.DodgeChance, stat.DodgeChance , true);
        SetStat(MoveSpeed, baseStat.MoveSpeed, stat.MoveSpeed , false);
        SetStat(Luck, baseStat.Luck, stat.Luck , false);
        SetStat(PickupRange, baseStat.PickupRange, stat.PickupRange , false);
        SetStat(ExpMultiplier, baseStat.ExpMultiplier, stat.ExpMultiplier , false);
        SetStat(GoldMultiplier, baseStat.GoldMultiplier, stat.GoldMultiplier , false);
    }


    void UpdateStat(StatType type , float basevalue, float newValue )
{   
    switch (type)
    {
        case StatType.DamageMultiplier:
            SetStat(damageMultiplier, basevalue, newValue ,true );
            break;
        case StatType.AttackSpeed:
            SetStat(AttackSpeed, basevalue, newValue ,false );
            break;
        case StatType.CriticalChance:
            SetStat(CriticalChance, basevalue, newValue ,true);
            break;
        case StatType.CriticalDamage:
            SetStat(CriticalDamage, basevalue, newValue, true);
            break;
        case StatType.LifeSteal:
            SetStat(LifeSteal, basevalue, newValue , true);
            break;
        case StatType.Armor:
            SetStat(Armor, basevalue, newValue, false);
            break;
        case StatType.MagicResist:
            SetStat(MagicResist, basevalue, newValue, false);
            break;
        case StatType.MaxHealth:
            SetStat(MaxHealth, basevalue, newValue, false);
            break;
        case StatType.HealthRegen:
            SetStat(HealthRegen, basevalue, newValue, false);
            break;
        case StatType.DodgeChance:
            SetStat(DodgeChance, basevalue, newValue , true);
            break;
        case StatType.MoveSpeed:
            SetStat(MoveSpeed, basevalue, newValue , false);
            break;
        case StatType.Luck:
            SetStat(Luck, basevalue, newValue   , false);
            break;
        case StatType.PickupRange:
            SetStat(PickupRange, basevalue, newValue , false);
            break;
        case StatType.ExpMultiplier:
            SetStat(ExpMultiplier, basevalue, newValue , true);
            break;
        case StatType.GoldMultiplier:
            SetStat(GoldMultiplier, basevalue, newValue , true);
            break;
        default:
            // Unknown stat — optionally refresh full panel
            Refresh();
            break;
    }
}

    private void SetStat(TextMeshProUGUI tmp, float baseValue, float currentValue, bool isPercentage )
    {
        // giá trị % .##
        tmp.text = isPercentage ? $"{currentValue * 100f:0.##}%" : $"{currentValue:0.##}" ;

        if (currentValue > baseValue)
            tmp.color = Color.green ; 
        else if (currentValue < baseValue)
            tmp.color = Color.red ; 
        else
            tmp.color = Color.gray ; 
    }

    public void TogglePanel()
    {
        isOpen = !isOpen;
        slideBtn.image.sprite = isOpen ? openedSptite : closedSprite;

        float targetX = isOpen ? 0 : -panelWidth;

        panel.anchoredPosition = new Vector2( targetX , 0  ) ; 

    }
}