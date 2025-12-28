using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class WeaponUpdateTool : EditorWindow
{
    private string jsonFilePath = "Assets/ExportedData/AllWeapons.json";
    private bool backupBeforeImport = true;
    private bool updateBuffs = true;
    private bool updateEffects = true;
    
    private bool updateThrust = true;
    private bool updateBow = true;
    private bool updateSword = true;
    
    private Vector2 scrollPosition;
    private string lastImportLog = "";
    
    [MenuItem("Tools/Update Weapons from JSON")]
    public static void ShowWindow()
    {
        GetWindow<WeaponUpdateTool>("Weapon Update Tool");
    }

    void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        GUILayout.Label("Update Weapons from JSON", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // File selection
        EditorGUILayout.BeginHorizontal();
        jsonFilePath = EditorGUILayout.TextField("JSON File:", jsonFilePath);
        if (GUILayout.Button("Browse", GUILayout.Width(70)))
        {
            string path = EditorUtility.OpenFilePanel("Select JSON File", "Assets/ExportedData", "json");
            if (!string.IsNullOrEmpty(path))
            {
                jsonFilePath = "Assets" + path.Replace(Application.dataPath, "");
            }
        }
        EditorGUILayout.EndHorizontal();
        
        GUILayout.Space(10);
        
        // Weapon Type Selection
        GUILayout.Label("Weapon Types to Update:", EditorStyles.boldLabel);
        updateThrust = EditorGUILayout.Toggle("Update Thrust Weapons", updateThrust);
        updateBow = EditorGUILayout.Toggle("Update Bow Weapons", updateBow);
        updateSword = EditorGUILayout.Toggle("Update Sword Weapons", updateSword);
        
        GUILayout.Space(10);
        
        // Options
        GUILayout.Label("Update Options:", EditorStyles.boldLabel);
        backupBeforeImport = EditorGUILayout.Toggle("Backup Before Import", backupBeforeImport);
        updateBuffs = EditorGUILayout.Toggle("Update Buffs", updateBuffs);
        updateEffects = EditorGUILayout.Toggle("Update Effects", updateEffects);
        
        GUILayout.Space(10);

        // Import button
        if (GUILayout.Button("Update Weapons Now", GUILayout.Height(40)))
        {
            UpdateWeapons();
        }
        
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "Tool sẽ đọc file JSON định dạng mới (một mảng Weapons duy nhất):\n" +
            "- Thrust → Assets/ScriptableObject/Weapon/Thrust\n" +
            "- Bow → Assets/ScriptableObject/Weapon/Bow\n" +
            "- Sword → Assets/ScriptableObject/Weapon/Sword\n\n" +
            "Chọn loại vũ khí muốn update ở trên!\n\n" +
            "Sẽ update: Damage, Cooldown, Range, Cost, Rarity, Buffs, Effects\n" +
            "Không đổi: Icon, Prefab, Type",
            MessageType.Info
        );
        
        // Show last import log
        if (!string.IsNullOrEmpty(lastImportLog))
        {
            GUILayout.Space(10);
            GUILayout.Label("Last Import Log:", EditorStyles.boldLabel);
            EditorGUILayout.TextArea(lastImportLog, GUILayout.Height(200));
        }
        
        EditorGUILayout.EndScrollView();
    }

    private void UpdateWeapons()
    {
        if (!File.Exists(jsonFilePath))
        {
            EditorUtility.DisplayDialog("Error", $"File không tồn tại:\n{jsonFilePath}", "OK");
            return;
        }

        try
        {
            // Read JSON
            string json = File.ReadAllText(jsonFilePath);
            WeaponDataWrapper wrapper = JsonUtility.FromJson<WeaponDataWrapper>(json);
            
            if (wrapper == null || wrapper.Weapons == null)
            {
                EditorUtility.DisplayDialog("Error", "Không thể parse JSON file hoặc không có dữ liệu Weapons!", "OK");
                return;
            }

            // Backup if enabled
            if (backupBeforeImport)
            {
                CreateBackup();
            }

            List<string> log = new List<string>();
            log.Add($"=== IMPORT STARTED: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss} ===");
            log.Add($"JSON File: {jsonFilePath}");
            log.Add($"Update Buffs: {updateBuffs}");
            log.Add($"Update Effects: {updateEffects}");
            log.Add("");

            int totalUpdated = 0;
            int totalNotFound = 0;

            // Update Thrust Weapons
            if (updateThrust)
            {
                var thrustList = wrapper.Weapons.Where(w => w.Type == "Thrust").ToList();
                log.Add("━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                log.Add("🗡️  THRUST WEAPONS");
                log.Add("━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                var result = UpdateWeaponType(thrustList, "Assets/ScriptableObject/Weapon/Thrust", log);
                totalUpdated += result.Item1;
                totalNotFound += result.Item2;
                log.Add("");
            }

            // Update Bow Weapons
            if (updateBow)
            {
                var bowList = wrapper.Weapons.Where(w => w.Type == "Bow").ToList();
                log.Add("━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                log.Add("🏹 BOW WEAPONS");
                log.Add("━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                var result = UpdateWeaponType(bowList, "Assets/ScriptableObject/Weapon/Bow", log);
                totalUpdated += result.Item1;
                totalNotFound += result.Item2;
                log.Add("");
            }

            // Update Sword Weapons
            if (updateSword)
            {
                var swordList = wrapper.Weapons.Where(w => w.Type == "Sword").ToList();
                log.Add("━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                log.Add("⚔️  SWORD WEAPONS");
                log.Add("━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                var result = UpdateWeaponType(swordList, "Assets/ScriptableObject/Weapon/Sword", log);
                totalUpdated += result.Item1;
                totalNotFound += result.Item2;
                log.Add("");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            log.Add("━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            log.Add("=== IMPORT COMPLETE ===");
            log.Add($"✅ Total Updated: {totalUpdated}");
            log.Add($"❌ Total Not Found: {totalNotFound}");
            log.Add("━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

            // Store log
            lastImportLog = string.Join("\n", log);

            // Show dialog
            string message = $"Import hoàn tất!\n\n";
            message += $"✅ Updated: {totalUpdated}\n";
            message += $"❌ Not Found: {totalNotFound}\n\n";
            message += "Xem chi tiết trong window!";

            EditorUtility.DisplayDialog("Import Complete", message, "OK");
            
            Debug.Log($"[WeaponUpdateTool] Import Complete\n{lastImportLog}");
        }
        catch (System.Exception e)
        {
            lastImportLog = $"ERROR: {e.Message}\n{e.StackTrace}";
            EditorUtility.DisplayDialog("Error", $"Import failed:\n{e.Message}", "OK");
            Debug.LogError($"[WeaponUpdateTool] Error: {e}");
        }
    }

    private System.Tuple<int, int> UpdateWeaponType(List<WeaponData> weaponList, string targetFolderPath, List<string> log)
    {
        int updatedCount = 0;
        int notFoundCount = 0;

        log.Add($"Target Folder: {targetFolderPath}");
        log.Add($"Weapons in JSON: {weaponList.Count}");
        log.Add("");

        // Load all WeaponStats in target folder
        string[] guids = AssetDatabase.FindAssets("t:WeaponStats", new[] { targetFolderPath });
        Dictionary<string, WeaponStats> weaponMap = new Dictionary<string, WeaponStats>();
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            WeaponStats weapon = AssetDatabase.LoadAssetAtPath<WeaponStats>(path);
            if (weapon != null && !string.IsNullOrEmpty(weapon.Name))
            {
                weaponMap[weapon.Name] = weapon;
            }
        }

        log.Add($"Found {weaponMap.Count} WeaponStats in folder");
        log.Add("");

        // Update each weapon
        foreach (var weaponData in weaponList)
        {
            if (string.IsNullOrEmpty(weaponData.Name))
            {
                log.Add("⚠️ SKIPPED: Weapon without name");
                continue;
            }

            if (weaponMap.ContainsKey(weaponData.Name))
            {
                WeaponStats weapon = weaponMap[weaponData.Name];
                
                Undo.RecordObject(weapon, "Update Weapon Stats");
                
                // Store old values for logging
                float oldDamage = weapon.Damage;
                float oldCooldown = weapon.Cooldown;
                float oldRange = weapon.Range;
                int oldCost = weapon.Cost;
                Rarity oldRarity = weapon.Rarity;
                
                // Update basic values
                weapon.Damage = weaponData.Damage;
                weapon.Cooldown = weaponData.Cooldown;
                weapon.Range = weaponData.Range;
                weapon.Cost = weaponData.Cost;
                
                if (System.Enum.TryParse<Rarity>(weaponData.Rarity, out Rarity rarity))
                {
                    weapon.Rarity = rarity;
                }

                // Update Buffs if enabled
                if (updateBuffs && weaponData.Buffs != null && weaponData.Buffs.Count > 0)
                {
                    weapon.Buffs = new List<StatBuff>();
                    foreach (var buffData in weaponData.Buffs)
                    {
                        if (System.Enum.TryParse<StatType>(buffData.StatType, out StatType statType) && System.Enum.TryParse<BuffType>(buffData.buffType, out BuffType buffType) )
                        {
                            weapon.Buffs.Add(new StatBuff
                            {
                                Type = statType,
                                Value = buffData.Value,
                                BuffType = buffType
                            });
                        }
                    }
                }

                // Update Effects if enabled
                if (updateEffects && weaponData.Effects != null && weaponData.Effects.Count > 0)
                {
                    weapon.effects = new List<EffectConfig>();
                    foreach (var effectData in weaponData.Effects)
                    {
                        if (System.Enum.TryParse<EffectType>(effectData.EffectType, out EffectType effectType))
                        {
                            weapon.effects.Add(new EffectConfig
                            {
                                type = effectType,
                                value = effectData.Value,
                                duration = effectData.Duration,
                                tickInterval = effectData.TickRate
                            });
                        }
                    }
                }

                EditorUtility.SetDirty(weapon);
                updatedCount++;
                
                // Detailed log
                log.Add($"✅ {weaponData.Name}");
                bool hasChanges = false;
                
                if (oldDamage != weaponData.Damage)
                {
                    log.Add($"   Damage: {oldDamage} → {weaponData.Damage}");
                    hasChanges = true;
                }
                if (oldCooldown != weaponData.Cooldown)
                {
                    log.Add($"   Cooldown: {oldCooldown:F2}s → {weaponData.Cooldown:F2}s");
                    hasChanges = true;
                }
                if (oldRange != weaponData.Range)
                {
                    log.Add($"   Range: {oldRange:F2} → {weaponData.Range:F2}");
                    hasChanges = true;
                }
                if (oldCost != weaponData.Cost)
                {
                    log.Add($"   Cost: {oldCost} → {weaponData.Cost}");
                    hasChanges = true;
                }
                if (oldRarity.ToString() != weaponData.Rarity)
                {
                    log.Add($"   Rarity: {oldRarity} → {weaponData.Rarity}");
                    hasChanges = true;
                }
                if (updateBuffs && weaponData.Buffs != null && weaponData.Buffs.Count > 0)
                {
                    log.Add($"   Buffs: {weaponData.Buffs.Count} buffs");
                    hasChanges = true;
                }
                if (updateEffects && weaponData.Effects != null && weaponData.Effects.Count > 0)
                {
                    log.Add($"   Effects: {weaponData.Effects.Count} effects");
                    hasChanges = true;
                }
                
                if (!hasChanges)
                {
                    log.Add($"   (No changes)");
                }
            }
            else
            {
                notFoundCount++;
                log.Add($"❌ NOT FOUND: {weaponData.Name}");
            }
        }

        return new System.Tuple<int, int>(updatedCount, notFoundCount);
    }

    private void CreateBackup()
    {
        string backupFolder = "Assets/ExportedData/Backups";
        if (!Directory.Exists(backupFolder))
        {
            Directory.CreateDirectory(backupFolder);
        }

        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string backupPath = Path.Combine(backupFolder, $"Backup_AllWeapons_{timestamp}");
        
        try
        {
            if (!Directory.Exists(backupPath))
            {
                Directory.CreateDirectory(backupPath);
            }
            
            int backedUpCount = 0;
            
            // Backup all weapon types
            string[] folders = new string[] {
                "Assets/ScriptableObject/Weapon/Thrust",
                "Assets/ScriptableObject/Weapon/Bow",
                "Assets/ScriptableObject/Weapon/Sword"
            };
            
            foreach (string folder in folders)
            {
                if (!AssetDatabase.IsValidFolder(folder)) continue;
                
                string[] guids = AssetDatabase.FindAssets("t:WeaponStats", new[] { folder });
                string folderName = Path.GetFileName(folder);
                string subBackupPath = Path.Combine(backupPath, folderName);
                
                if (!Directory.Exists(subBackupPath))
                {
                    Directory.CreateDirectory(subBackupPath);
                }
                
                foreach (string guid in guids)
                {
                    string sourcePath = AssetDatabase.GUIDToAssetPath(guid);
                    string fileName = Path.GetFileName(sourcePath);
                    string destPath = Path.Combine(subBackupPath, fileName);
                    
                    File.Copy(sourcePath, destPath, true);
                    backedUpCount++;
                }
            }
            
            AssetDatabase.Refresh();
            Debug.Log($"[WeaponUpdateTool] Backup created: {backupPath} ({backedUpCount} files)");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[WeaponUpdateTool] Backup failed: {e.Message}");
        }
    }

    // JSON Data structures - CHỈ SỬA ĐỂ PHÙ HỢP VỚI FORMAT MỚI
    [System.Serializable]
    private class WeaponDataWrapper
    {
        public List<WeaponData> Weapons;
    }

    [System.Serializable]
    private class WeaponData
    {
        public string Name;
        public float Damage;
        public float Cooldown;
        public float Range;
        public string Type;
        public string Rarity;
        public int Cost;
        public List<BuffData> Buffs;
        public List<EffectData> Effects;
    }

    [System.Serializable]
    private class BuffData
    {
        public string StatType;
        public float Value;
        public string buffType; // Giữ nguyên để code cũ vẫn chạy
    }

    [System.Serializable]
    private class EffectData
    {
        public string EffectType;
        public float Value;
        public float Duration;
        public float TickRate;
    }
}