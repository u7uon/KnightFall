using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;

public class ItemUpdateTool : EditorWindow
{
    private string jsonFilePath = "Assets/ExportedData/Items_Accessory.json";
    private string targetFolderPath = "Assets/ScriptableObject/ShopItem";
    private bool backupBeforeImport = true;
    
    private Vector2 scrollPosition;
    private string lastImportLog = "";
    
    [MenuItem("Tools/Update Items from JSON")]
    public static void ShowWindow()
    {
        GetWindow<ItemUpdateTool>("Item Update Tool");
    }

    void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        GUILayout.Label("Update Items from JSON", EditorStyles.boldLabel);
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
        
        // Folder selection
        EditorGUILayout.BeginHorizontal();
        targetFolderPath = EditorGUILayout.TextField("Target Folder:", targetFolderPath);
        if (GUILayout.Button("Browse", GUILayout.Width(70)))
        {
            string path = EditorUtility.OpenFolderPanel("Select Target Folder", "Assets", "");
            if (!string.IsNullOrEmpty(path))
            {
                targetFolderPath = "Assets" + path.Replace(Application.dataPath, "");
            }
        }
        EditorGUILayout.EndHorizontal();
        
        GUILayout.Space(10);
        
        // Options
        backupBeforeImport = EditorGUILayout.Toggle("Backup Before Import:", backupBeforeImport);
        
        GUILayout.Space(10);

        // Import button
        if (GUILayout.Button("Update Items Now", GUILayout.Height(40)))
        {
            UpdateItems();
        }
        
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox(
            "Tool sẽ tìm ItemData theo Name field (localization key)\n" +
            "Ví dụ: JSON có Name=\"ITEM_HEALTHPOTION_NAME\" sẽ match với ScriptableObject có Name=\"ITEM_HEALTHPOTION_NAME\"\n\n" +
            "Sẽ update: Price, Rarity, StatModifiers\n" +
            "Không đổi: Icon, Description, Category",
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

    private void UpdateItems()
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
            ItemCollection collection = JsonUtility.FromJson<ItemCollection>(json);
            
            if (collection == null || collection.Items == null || collection.Items.Count == 0)
            {
                EditorUtility.DisplayDialog("Error", "JSON file rỗng hoặc không đúng format!", "OK");
                return;
            }

            // Backup if enabled
            if (backupBeforeImport)
            {
                CreateBackup();
            }

            int updatedCount = 0;
            int notFoundCount = 0;
            List<string> log = new List<string>();
            log.Add($"=== IMPORT STARTED: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss} ===");
            log.Add($"JSON File: {jsonFilePath}");
            log.Add($"Target Folder: {targetFolderPath}");
            log.Add($"Total Items in JSON: {collection.Items.Count}");
            log.Add("");

            // Load all ItemData in target folder
            string[] guids = AssetDatabase.FindAssets("t:ItemData", new[] { targetFolderPath });
            Dictionary<string, ItemData> itemMap = new Dictionary<string, ItemData>();
            
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ItemData item = AssetDatabase.LoadAssetAtPath<ItemData>(path);
                if (item != null && !string.IsNullOrEmpty(item.Name))
                {
                    itemMap[item.Name] = item;
                }
            }

            log.Add($"Found {itemMap.Count} ItemData in target folder");
            log.Add("");

            // Update each item
            foreach (var itemData in collection.Items)
            {
                if (string.IsNullOrEmpty(itemData.Name))
                {
                    log.Add("⚠️ SKIPPED: Item without name");
                    continue;
                }

                if (itemMap.ContainsKey(itemData.Name))
                {
                    ItemData item = itemMap[itemData.Name];
                    
                    Undo.RecordObject(item, "Update Item Stats");
                    
                    // Store old values for logging
                    int oldPrice = item.Price;
                    Rarity oldRarity = item.Rarity;
                    int oldStatCount = item.StatModifiers != null ? item.StatModifiers.Count : 0;
                    
                    // Update values
                    item.Price = itemData.Price;
                    
                    if (System.Enum.TryParse<Rarity>(itemData.Rarity, out Rarity rarity))
                    {
                        item.Rarity = rarity;
                    }

                    // Update StatModifiers
                    if (itemData.StatModifiers != null && itemData.StatModifiers.Count > 0)
                    {
                        item.StatModifiers = new List<StatBuff>();
                        foreach (var statData in itemData.StatModifiers)
                        {
                            if (Enum.TryParse<StatType>(statData.StatType, out StatType statType) && System.Enum.TryParse<BuffType>(statData.BuffType, out BuffType buffType) )
                            {
                                item.StatModifiers.Add(new StatBuff
                                {
                                    Type = statType,
                                    Value = statData.Value,
                                    BuffType = buffType
                                });
                            }
                        }
                    }

                    EditorUtility.SetDirty(item);
                    updatedCount++;
                    
                    // Detailed log
                    log.Add($"✅ UPDATED: {itemData.Name}");
                    if (oldPrice != itemData.Price)
                        log.Add($"   - Price: {oldPrice} → {itemData.Price}");
                    if (oldRarity.ToString() != itemData.Rarity)
                        log.Add($"   - Rarity: {oldRarity} → {itemData.Rarity}");
                    if (itemData.StatModifiers != null)
                        log.Add($"   - Stats: {oldStatCount} → {itemData.StatModifiers.Count} modifiers");
                }
                else
                {
                    notFoundCount++;
                    log.Add($"❌ NOT FOUND: {itemData.Name}");
                    log.Add($"   (No ItemData with Name=\"{itemData.Name}\" found in {targetFolderPath})");
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            log.Add("");
            log.Add("=== IMPORT COMPLETE ===");
            log.Add($"✅ Updated: {updatedCount}");
            log.Add($"❌ Not Found: {notFoundCount}");
            log.Add($"Total Processed: {collection.Items.Count}");

            // Store log
            lastImportLog = string.Join("\n", log);

            // Show dialog
            string message = $"Import hoàn tất!\n\n";
            message += $"✅ Updated: {updatedCount}\n";
            message += $"❌ Not Found: {notFoundCount}\n";
            message += $"Total: {collection.Items.Count}\n\n";
            message += "Xem chi tiết trong window!";

            EditorUtility.DisplayDialog("Import Complete", message, "OK");
            
            Debug.Log($"[ItemUpdateTool] Import Complete\n{lastImportLog}");
        }
        catch (System.Exception e)
        {
            lastImportLog = $"ERROR: {e.Message}\n{e.StackTrace}";
            EditorUtility.DisplayDialog("Error", $"Import failed:\n{e.Message}", "OK");
            Debug.LogError($"[ItemUpdateTool] Error: {e}");
        }
    }

    private void CreateBackup()
    {
        string backupFolder = "Assets/ExportedData/Backups";
        if (!Directory.Exists(backupFolder))
        {
            Directory.CreateDirectory(backupFolder);
        }

        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string backupPath = Path.Combine(backupFolder, $"Backup_Items_{timestamp}");
        
        try
        {
            if (!Directory.Exists(backupPath))
            {
                Directory.CreateDirectory(backupPath);
            }
            
            string[] guids = AssetDatabase.FindAssets("t:ItemData", new[] { targetFolderPath });
            
            int backedUpCount = 0;
            foreach (string guid in guids)
            {
                string sourcePath = AssetDatabase.GUIDToAssetPath(guid);
                string fileName = Path.GetFileName(sourcePath);
                string destPath = Path.Combine(backupPath, fileName);
                
                File.Copy(sourcePath, destPath, true);
                backedUpCount++;
            }
            
            AssetDatabase.Refresh();
            Debug.Log($"[ItemUpdateTool] Backup created: {backupPath} ({backedUpCount} files)");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[ItemUpdateTool] Backup failed: {e.Message}");
        }
    }

    // JSON Data structures
    [System.Serializable]
    private class ItemCollection
    {
        public int TotalCount;
        public List<ItemDataJson> Items;
    }

    [System.Serializable]
    private class ItemDataJson
    {
        public string Name;
        public int Price;
        public string Rarity;
        public string Category;
        public List<StatModifierJson> StatModifiers;
    }

    [System.Serializable]
    private class StatModifierJson
    {
        public string StatType;
        public float Value;
        public string BuffType;
    }
}