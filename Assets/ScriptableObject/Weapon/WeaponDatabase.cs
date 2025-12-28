using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "WeaponDatabase", menuName = "Shop/WeaponDatabase")]
public class WeaponDatabase : ScriptableObject
{
    public List<WeaponStats> allWeapons;

    public WeaponStats GetRandomWeaponByRarity(Rarity rarity)
    {
        var list = allWeapons.Where(w => w.Rarity == rarity).ToList();
        if (list.Count == 0) return null;
        return list[Random.Range(0, list.Count)];
    }
}
