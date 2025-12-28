using TMPro;
using UnityEngine;

public class PlayerStatsUI : MonoBehaviour
{
    [Header("Player Stats UI References")] 
    [SerializeField] private TextMeshProUGUI damageMultiplier ;
    [SerializeField] private TextMeshProUGUI AttackSpeed ;
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




    void OnEnable()
    {
        DisplayPlayerStats();
    }

    public void DisplayPlayerStats()
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
}
