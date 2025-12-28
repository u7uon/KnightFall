using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Player/Stats")]
public class PlayerStats : ScriptableObject
{
    [Header("Display Info")]
    public string Name ;
    public string Description ;
    public Sprite Icon;
    
    public GameObject characterPrefab; // Prefab có sẵn Animator



    [Header("⚔️ Combat Stats")]
    public float damageMultiplier = 1f;          // Nhân sát thương vũ khí
    public float AttackSpeed = 1f;               // Tốc độ tấn công
    public float CriticalChance = 0.05f;         // % chí mạng
    public float CriticalDamage = 1.5f;          // Hệ số chí mạng
    public float LifeSteal = 0f;                 // % hút máu
    public float Armor = 0f;                     // Giảm sát thương vật lý
    public float MagicResist = 0f;               // Giảm sát thương phép

    [Header("❤️ Survival Stats")]
    public float MaxHealth = 100f;
    public float HealthRegen = 0f;
    public float DodgeChance = 0f;

    [Header("🏃 Utility Stats")]
    public float MoveSpeed = 5f;
    public float Luck = 0f;
    public float PickupRange = 2f;
    public float ExpMultiplier = 1f;
    public float GoldMultiplier = 1f;

    [Header("🎭 Class Info")]
    public PlayerClass Class;


}
        

public enum PlayerClass
{
    Swordsman , Assassin , Archer
}
