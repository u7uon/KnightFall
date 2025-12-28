using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class AllWeaponExportTool : EditorWindow
{
    private bool exportBow = true;
    private bool exportSword = true;
    private bool exportThrust = true;
    
    [MenuItem("Tools/Export All Weapons to JSON")]
    public static void ShowWindow()
    {
        GetWindow<AllWeaponExportTool>("All Weapon Export Tool");
    }

    void OnGUI()
    {
        GUILayout.Label("Export All Weapons", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.LabelField("Chọn loại vũ khí cần export:", EditorStyles.boldLabel);
        exportBow = EditorGUILayout.Toggle("Bow", exportBow);
        exportSword = EditorGUILayout.Toggle("Sword", exportSword);
        exportThrust = EditorGUILayout.Toggle("Thrust", exportThrust);
        
        GUILayout.Space(10);

        if (GUILayout.Button("Export to JSON", GUILayout.Height(40)))
        {
            ExportAllWeapons();
        }
        
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox(
            "Xuất tất cả vũ khí từ các thư mục:\n" +
            "• Assets/ScriptableObject/Weapon/Bow\n" +
            "• Assets/ScriptableObject/Weapon/Sword\n" +
            "• Assets/ScriptableObject/Weapon/Thrust\n\n" +
            "File sẽ được lưu tại: Assets/ExportedData/AllWeapons.json", 
            MessageType.Info
        );
    }

    private void ExportAllWeapons()
    {
        if (!exportBow && !exportSword && !exportThrust)
        {
            EditorUtility.DisplayDialog("Warning", 
                "Vui lòng chọn ít nhất một loại vũ khí để export!", "OK");
            return;
        }

        List<WeaponData> allWeaponsList = new List<WeaponData>();
        int bowCount = 0, swordCount = 0, thrustCount = 0;

        // Export Bow
        if (exportBow)
        {
            var bowWeapons = ExportWeaponsByType("Assets/ScriptableObject/Weapon/Bow", WeaponType.Bow);
            allWeaponsList.AddRange(bowWeapons);
            bowCount = bowWeapons.Count;
        }

        // Export Sword
        if (exportSword)
        {
            var swordWeapons = ExportWeaponsByType("Assets/ScriptableObject/Weapon/Sword", WeaponType.Sword);
            allWeaponsList.AddRange(swordWeapons);
            swordCount = swordWeapons.Count;
        }

        // Export Thrust
        if (exportThrust)
        {
            var thrustWeapons = ExportWeaponsByType("Assets/ScriptableObject/Weapon/Thrust", WeaponType.Thrust);
            allWeaponsList.AddRange(thrustWeapons);
            thrustCount = thrustWeapons.Count;
        }

        if (allWeaponsList.Count == 0)
        {
            EditorUtility.DisplayDialog("Warning", 
                "Không tìm thấy vũ khí nào!", "OK");
            return;
        }

        // Tạo wrapper object với thống kê chi tiết
        AllWeaponCollection collection = new AllWeaponCollection
        {
            TotalCount = allWeaponsList.Count,
            BowCount = bowCount,
            SwordCount = swordCount,
            ThrustCount = thrustCount,
            ExportDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            Weapons = allWeaponsList
        };

        // Convert to JSON
        string json = JsonUtility.ToJson(collection, true);

        // Tạo thư mục nếu chưa có
        string outputFolder = "Assets/ExportedData";
        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }

        // Ghi file
        string outputPath = Path.Combine(outputFolder, "AllWeapons.json");
        File.WriteAllText(outputPath, json);

        AssetDatabase.Refresh();

        string message = $"Đã export thành công!\n\n" +
                        $"Tổng: {allWeaponsList.Count} vũ khí\n" +
                        $"• Bow: {bowCount}\n" +
                        $"• Sword: {swordCount}\n" +
                        $"• Thrust: {thrustCount}\n\n" +
                        $"File: {outputPath}";

        EditorUtility.DisplayDialog("Success", message, "OK");

        Debug.Log($"[AllWeaponExportTool] Exported {allWeaponsList.Count} weapons (Bow: {bowCount}, Sword: {swordCount}, Thrust: {thrustCount}) to: {outputPath}");
    }

    private List<WeaponData> ExportWeaponsByType(string folderPath, WeaponType weaponType)
    {
        List<WeaponData> weaponDataList = new List<WeaponData>();

        // Tìm tất cả WeaponStats trong folder
        string[] guids = AssetDatabase.FindAssets("t:WeaponStats", new[] { folderPath });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            WeaponStats weapon = AssetDatabase.LoadAssetAtPath<WeaponStats>(path);
            
            if (weapon != null && weapon.Type == weaponType)
            {
                WeaponData data = new WeaponData
                {
                    Name = weapon.Name,
                    Damage = weapon.Damage,
                    Cooldown = weapon.Cooldown,
                    Range = weapon.Range,
                    Type = weapon.Type.ToString(),
                    Rarity = weapon.Rarity.ToString(),
                    Cost = weapon.Cost,
                    IconPath = weapon.Icon != null ? AssetDatabase.GetAssetPath(weapon.Icon) : "null",
                    WeaponPrefabPath = weapon.weaponPrefab != null ? AssetDatabase.GetAssetPath(weapon.weaponPrefab) : "null",
                    SpecialEffect = weapon.specialEffect != null ? weapon.specialEffect.ToString() : "null",
                    Buffs = new List<BuffData>(),
                    Effects = new List<EffectData>()
                };

                // Export Buffs
                if (weapon.Buffs != null && weapon.Buffs.Count > 0)
                {
                    foreach (var buff in weapon.Buffs)
                    {
                        data.Buffs.Add(new BuffData
                        {
                            StatType = buff.Type.ToString(),
                            Value = buff.Value,
                            buffType = buff.BuffType.ToString()
                        });
                    }
                }

                // Export Effects
                if (weapon.effects != null && weapon.effects.Count > 0)
                {
                    foreach (var effect in weapon.effects)
                    {
                        data.Effects.Add(new EffectData
                        {
                            EffectType = effect.type.ToString(),
                            Value = effect.value,
                            Duration = effect.duration,
                            TickRate = effect.tickInterval
                        });
                    }
                }

                weaponDataList.Add(data);
            }
        }

        return weaponDataList;
    }
}

// ===== DATA CLASSES =====

[System.Serializable]
public class AllWeaponCollection
{
    public int TotalCount;
    public int BowCount;
    public int SwordCount;
    public int ThrustCount;
    public string ExportDate;
    public List<WeaponData> Weapons;
}

[System.Serializable]
public class WeaponData
{
    public string Name;
    public float Damage;
    public float Cooldown;
    public float Range;
    public string Type;
    public string Rarity;
    public int Cost;
    public string IconPath;
    public string WeaponPrefabPath;
    public string SpecialEffect;
    public List<BuffData> Buffs;
    public List<EffectData> Effects;
}

[System.Serializable]
public class BuffData
{
    public string StatType;
    public float Value;

    public string buffType;
}

[System.Serializable]
public class EffectData
{
    public string EffectType;
    public float Value;
    public float Duration;
    public float TickRate;
}