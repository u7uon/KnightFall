using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ItemDataCreator
{
    [MenuItem("Tools/Generate Items")]
    public static void GenerateItems()
    {
        string folderPath = "Assets/ScriptableObject/ShopItem";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets/ScriptableObject", "ShopItem");
        }

        List<ItemData> items = new List<ItemData>();

        // Icon1.png - Enchanted Dagger (Dao phép thuật)
        items.Add(CreateItem("ITEM_ENCHANTEDDAGGER_NAME", Rarity.Uncommon, 140,
            new List<StatBuff> {
                NewBuff(StatType.DamageMultiplier, 0.12f),
                NewBuff(StatType.AttackSpeed, 0.08f),
                NewBuff(StatType.CriticalChance, 0.05f)
            }
        ));

        // Icon2.png - Magic Scroll (Cuộn phép)
        items.Add(CreateItem("ITEM_MAGICSCROLL_NAME", Rarity.Rare, 250,
            new List<StatBuff> {
                NewBuff(StatType.DamageMultiplier, 0.15f),
                NewBuff(StatType.MagicResist, 0.10f),
                NewBuff(StatType.Luck, 0.08f)
            }
        ));

        // Icon6.png - Dark Charm (Bùa đen)
        items.Add(CreateItem("ITEM_DARKCHARM_NAME", Rarity.Epic, 330,
            new List<StatBuff> {
                NewBuff(StatType.CriticalDamage, 0.25f),
                NewBuff(StatType.LifeSteal, 0.10f),
                NewBuff(StatType.MaxHealth, -0.05f)
            }
        ));

        // Icon7.png - Flame Essence (Tinh chất lửa)
        items.Add(CreateItem("ITEM_FLAMEESSENCE_NAME", Rarity.Rare, 260,
            new List<StatBuff> {
                NewBuff(StatType.DamageMultiplier, 0.18f),
                NewBuff(StatType.CriticalChance, 0.12f),
                NewBuff(StatType.MagicResist, -0.08f)
            }
        ));

        // Icon11.png - Cursed Relic (Di vật bị nguyền)
        items.Add(CreateItem("ITEM_CURSEDRELIC_NAME", Rarity.Epic, 380,
            new List<StatBuff> {
                NewBuff(StatType.DamageMultiplier, 0.28f),
                NewBuff(StatType.CriticalDamage, 0.15f),
                NewBuff(StatType.HealthRegen, -0.05f),
                NewBuff(StatType.Armor, -0.10f)
            }
        ));

        // Icon12.png - Phoenix Feather (Lông phượng hoàng)
        items.Add(CreateItem("ITEM_PHOENIXFEATHER_NAME", Rarity.Legendary, 550,
            new List<StatBuff> {
                NewBuff(StatType.HealthRegen, 0.20f),
                NewBuff(StatType.MaxHealth, 0.15f),
                NewBuff(StatType.MagicResist, 0.18f),
                NewBuff(StatType.DamageMultiplier, 0.10f)
            }
        ));

        // Icon16.png - Mystic Orb (Quả cầu huyền bí)
        items.Add(CreateItem("ITEM_MYSTICORB_NAME", Rarity.Epic, 340,
            new List<StatBuff> {
                NewBuff(StatType.MagicResist, 0.22f),
                NewBuff(StatType.AttackSpeed, 0.10f),
                NewBuff(StatType.Luck, 0.12f)
            }
        ));

        // Icon17.png - Dragon Scale (Vảy rồng)
        items.Add(CreateItem("ITEM_DRAGONSCALE_NAME", Rarity.Legendary, 580,
            new List<StatBuff> {
                NewBuff(StatType.Armor, 0.30f),
                NewBuff(StatType.MagicResist, 0.25f),
                NewBuff(StatType.MaxHealth, 0.12f),
                NewBuff(StatType.AttackSpeed, -0.08f)
            }
        ));

        // Icon21.png - Venom Blade (Lưỡi dao độc)
        items.Add(CreateItem("ITEM_VENOMBLADE_NAME", Rarity.Rare, 280,
            new List<StatBuff> {
                NewBuff(StatType.DamageMultiplier, 0.16f),
                NewBuff(StatType.LifeSteal, 0.08f),
                NewBuff(StatType.CriticalChance, 0.10f),
                NewBuff(StatType.HealthRegen, -0.03f)
            }
        ));

        // Icon22.png - Emerald Pendant (Mặt dây chuyền ngọc lục bảo)
        items.Add(CreateItem("ITEM_EMERALDPENDANT_NAME", Rarity.Epic, 360,
            new List<StatBuff> {
                NewBuff(StatType.HealthRegen, 0.15f),
                NewBuff(StatType.MaxHealth, 0.10f),
                NewBuff(StatType.Luck, 0.15f),
                NewBuff(StatType.MagicResist, 0.12f)
            }
        ));

        // Icon26.png - Ancient Torch (Ngọn đuốc cổ)
        items.Add(CreateItem("ITEM_ANCIENTTORCH_NAME", Rarity.Uncommon, 170,
            new List<StatBuff> {
                NewBuff(StatType.DamageMultiplier, 0.14f),
                NewBuff(StatType.Luck, 0.10f),
                NewBuff(StatType.AttackSpeed, 0.08f)
            }
        ));

        // Icon27.png - Golden Ring (Nhẫn vàng)
        items.Add(CreateItem("ITEM_GOLDENRING_NAME", Rarity.Rare, 240,
            new List<StatBuff> {
                NewBuff(StatType.GoldMultiplier, 0.25f),
                NewBuff(StatType.Luck, 0.18f),
                NewBuff(StatType.ExpMultiplier, 0.10f)
            }
        ));

        // Icon31.png - Shadow Shard (Mảnh bóng tối)
        items.Add(CreateItem("ITEM_SHADOWSHARD_NAME", Rarity.Legendary, 520,
            new List<StatBuff> {
                NewBuff(StatType.CriticalChance, 0.20f),
                NewBuff(StatType.CriticalDamage, 0.30f),
                NewBuff(StatType.DamageMultiplier, 0.18f),
                NewBuff(StatType.MaxHealth, -0.12f)
            }
        ));

        // Icon32.png - Royal Crown (Vương miện hoàng gia)
        items.Add(CreateItem("ITEM_ROYALCROWN_NAME", Rarity.Legendary, 650,
            new List<StatBuff> {
                NewBuff(StatType.GoldMultiplier, 0.35f),
                NewBuff(StatType.ExpMultiplier, 0.20f),
                NewBuff(StatType.Luck, 0.25f),
                NewBuff(StatType.MaxHealth, 0.10f)
            }
        ));


        foreach (var item in items)
        {
            string assetPath = $"{folderPath}/{item.Name}.asset";
            AssetDatabase.CreateAsset(item, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }


    private static ItemData CreateItem(
        string name,
        Rarity rarity,
        int price,
        List<StatBuff> buffs,
        SpecialEffect specialEffect = null,
        ItemData.ItemCategory category = ItemData.ItemCategory.Accessory)
    {
        ItemData item = ScriptableObject.CreateInstance<ItemData>();
        item.Name = name;
        item.Rarity = rarity;
        item.Price = price;
        item.Description = "";
        item.StatModifiers = buffs;
        item.SpecialEffect = specialEffect;
        item.Category = category;
        item.Icon = null;
        return item;
    }

    private static StatBuff NewBuff(StatType type, float value)
    {
        return new StatBuff
        {
            Type = type,
            Value = value
        };
    }
}