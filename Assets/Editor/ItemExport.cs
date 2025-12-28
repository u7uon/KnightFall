using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ItemExportTool : EditorWindow
{
    private string folderPath = "Assets/ScriptableObject/ShopItem";
    private string outputFileName = "ShopItems.json";
    private ItemData.ItemCategory filterCategory = ItemData.ItemCategory.Armor;
    private bool useFilter = false;
    
    [MenuItem("Tools/Export Shop Items to JSON")]
    public static void ShowWindow()
    {
        GetWindow<ItemExportTool>("Item Export Tool");
    }

    void OnGUI()
    {
        GUILayout.Label("Export Shop Items", EditorStyles.boldLabel);
        GUILayout.Space(10);

        folderPath = EditorGUILayout.TextField("Folder Path:", folderPath);
        outputFileName = EditorGUILayout.TextField("Output File Name:", outputFileName);
        
        GUILayout.Space(10);
        
        useFilter = EditorGUILayout.Toggle("Filter by Category:", useFilter);
        if (useFilter)
        {
            filterCategory = (ItemData.ItemCategory)EditorGUILayout.EnumPopup("Category:", filterCategory);
        }
        
        GUILayout.Space(10);

        if (GUILayout.Button("Export to JSON", GUILayout.Height(40)))
        {
            ExportItemsToJSON();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("Export All Categories (Separate Files)", GUILayout.Height(30)))
        {
            ExportAllCategoriesSeparately();
        }
        
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox(
            "Xuất tất cả ItemData trong thư mục ShopItem ra file JSON.\n" +
            "File sẽ được lưu tại: Assets/ExportedData/", 
            MessageType.Info
        );
    }

    private void ExportItemsToJSON()
    {
        // Tìm tất cả ItemData trong folder
        string[] guids = AssetDatabase.FindAssets("t:ItemData", new[] { folderPath });
        
        if (guids.Length == 0)
        {
            EditorUtility.DisplayDialog("Warning", 
                $"Không tìm thấy ItemData nào trong thư mục:\n{folderPath}", "OK");
            return;
        }

        List<ItemDataExport> itemDataList = new List<ItemDataExport>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ItemData item = AssetDatabase.LoadAssetAtPath<ItemData>(path);
            
            if (item != null)
            {
                // Nếu có filter và không khớp category thì skip
                if (useFilter && item.Category != filterCategory)
                    continue;

                ItemDataExport data = new ItemDataExport
                {
                    Name = item.Name,
                    Price = item.Price,
                    Rarity = item.Rarity.ToString(),
                    Category = item.Category.ToString(),
                    Description = item.Description,
                    IconPath = item.Icon != null ? AssetDatabase.GetAssetPath(item.Icon) : "null",
                    StatModifiers = new List<StatBuffExport>(),
                    SpecialEffect = null
                };

                // Export Stat Modifiers
                if (item.StatModifiers != null && item.StatModifiers.Count > 0)
                {
                    foreach (var stat in item.StatModifiers)
                    {
                        data.StatModifiers.Add(new StatBuffExport
                        {
                            StatType = stat.Type.ToString(),
                            Value = stat.Value,
                        });
                    }
                }

                // Export Special Effect
                if (item.SpecialEffect != null)
                {
                    data.SpecialEffect = new SpecialEffectExport
                    {
                        EffectName = item.SpecialEffect.EffectName,
                        Description = item.SpecialEffect.Description
                    };
                }

                itemDataList.Add(data);
            }
        }

        if (itemDataList.Count == 0)
        {
            string message = useFilter 
                ? $"Không tìm thấy item nào với category: {filterCategory}!" 
                : "Không tìm thấy item nào!";
            EditorUtility.DisplayDialog("Warning", message, "OK");
            return;
        }

        // Tạo wrapper object
        ItemCollection collection = new ItemCollection
        {
            TotalCount = itemDataList.Count,
            ExportDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            FilteredCategory = useFilter ? filterCategory.ToString() : "All",
            Items = itemDataList
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
        string outputPath = Path.Combine(outputFolder, outputFileName);
        File.WriteAllText(outputPath, json);

        AssetDatabase.Refresh();

        string successMessage = useFilter
            ? $"Đã export {itemDataList.Count} items (Category: {filterCategory})!\n\nFile: {outputPath}"
            : $"Đã export {itemDataList.Count} items!\n\nFile: {outputPath}";

        EditorUtility.DisplayDialog("Success", successMessage, "OK");

        Debug.Log($"[ItemExportTool] Exported {itemDataList.Count} items to: {outputPath}");
    }

    private void ExportAllCategoriesSeparately()
    {
        int totalExported = 0;
        List<string> exportedFiles = new List<string>();

        // Lấy tất cả categories
        var categories = System.Enum.GetValues(typeof(ItemData.ItemCategory));

        foreach (ItemData.ItemCategory category in categories)
        {
            // Tìm items theo category
            string[] guids = AssetDatabase.FindAssets("t:ItemData", new[] { folderPath });
            List<ItemDataExport> itemDataList = new List<ItemDataExport>();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ItemData item = AssetDatabase.LoadAssetAtPath<ItemData>(path);
                
                if (item != null && item.Category == category)
                {
                    ItemDataExport data = new ItemDataExport
                    {
                        Name = item.Name,
                        Price = item.Price,
                        Rarity = item.Rarity.ToString(),
                        Category = item.Category.ToString(),
                        Description = item.Description,
                        IconPath = item.Icon != null ? AssetDatabase.GetAssetPath(item.Icon) : "null",
                        StatModifiers = new List<StatBuffExport>(),
                        SpecialEffect = null
                    };

                    if (item.StatModifiers != null)
                    {
                        foreach (var stat in item.StatModifiers)
                        {
                            data.StatModifiers.Add(new StatBuffExport
                            {
                                StatType = stat.Type.ToString(),
                                Value = stat.Value,
                                BuffType = stat.BuffType.ToString()
                            });
                        }
                    }

                    if (item.SpecialEffect != null)
                    {
                        data.SpecialEffect = new SpecialEffectExport
                        {
                            EffectName = item.SpecialEffect.EffectName,
                            Description = item.SpecialEffect.Description
                        };
                    }

                    itemDataList.Add(data);
                }
            }

            // Chỉ export nếu có items
            if (itemDataList.Count > 0)
            {
                ItemCollection collection = new ItemCollection
                {
                    TotalCount = itemDataList.Count,
                    ExportDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    FilteredCategory = category.ToString(),
                    Items = itemDataList
                };

                string json = JsonUtility.ToJson(collection, true);
                string outputFolder = "Assets/ExportedData";
                
                if (!Directory.Exists(outputFolder))
                {
                    Directory.CreateDirectory(outputFolder);
                }

                string fileName = $"Items_{category}.json";
                string outputPath = Path.Combine(outputFolder, fileName);
                File.WriteAllText(outputPath, json);
                
                exportedFiles.Add($"{category}: {itemDataList.Count} items → {fileName}");
                totalExported += itemDataList.Count;
            }
        }

        AssetDatabase.Refresh();

        if (totalExported > 0)
        {
            string message = $"Đã export {totalExported} items thành {exportedFiles.Count} files:\n\n";
            message += string.Join("\n", exportedFiles);
            EditorUtility.DisplayDialog("Success", message, "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Warning", "Không tìm thấy item nào để export!", "OK");
        }
    }
}

// ===== DATA CLASSES =====

[System.Serializable]
public class ItemCollection
{
    public int TotalCount;
    public string ExportDate;
    public string FilteredCategory;
    public List<ItemDataExport> Items;
}

[System.Serializable]
public class ItemDataExport
{
    public string Name;
    public int Price;
    public string Rarity;
    public string Category;
    public string Description;
    public string IconPath;
    public List<StatBuffExport> StatModifiers;
    public SpecialEffectExport SpecialEffect;
}

[System.Serializable]
public class StatBuffExport
{
    public string StatType;
    public float Value;
    public string BuffType;
}

[System.Serializable]
public class SpecialEffectExport
{
    public string EffectName;
    public string Description;
}