using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDb", menuName = "Scriptable Objects/ItemDb")]
public class ItemDb : ScriptableObject
{
   public List<ItemData> itemDatas; 
    
   public ItemData GetRandomWeaponByRarity(Rarity rarity)
    {
        var list = itemDatas.Where(w => w.Rarity == rarity).ToList();
        if (list.Count == 0) return null;
        return list[Random.Range(0, list.Count)];
    }

}
