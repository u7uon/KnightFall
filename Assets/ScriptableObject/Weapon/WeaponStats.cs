using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponStats", menuName = "WeaponStats")]
public class WeaponStats : ScriptableObject
{

    public string Name;
    public float Damage;

    public Sprite Icon; 

    public float Cooldown;

    public float Range;

    public WeaponType Type = WeaponType.Sword;

    public Rarity Rarity;

    public int Cost;

    public List<StatBuff> Buffs;

    public WeaponBase weaponPrefab;

    public SpecialEffect specialEffect ; 

    public List<EffectConfig> effects ; 

}


public enum WeaponType
{
    Sword, Thrust, Bow
}


public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}
