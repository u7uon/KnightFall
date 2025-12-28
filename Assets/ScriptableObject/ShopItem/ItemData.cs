using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public string Name;                    // Tên vật phẩm
    public Sprite Icon; 
    public Rarity Rarity;              // Bậc: Common → Legendary
    public int Price;                      // 💰 Giá mua tại shop
    public string Description;             // Mô tả ngắn (hiệu ứng)
    public List<StatBuff> StatModifiers;   // Các chỉ số + hoặc -
    public SpecialEffect SpecialEffect;    // Hiệu ứng đặc biệt (nếu có)
    public ItemCategory Category;          // Loại item (ví dụ: Armor, Accessory)

    public enum ItemCategory { Armor, Accessory, WeaponMod, Utility, Consumable }

}


[System.Serializable]
public class SpecialEffect
{
    public string EffectName;
    public string Description;
}
    