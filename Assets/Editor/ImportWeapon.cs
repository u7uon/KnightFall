using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class WeaponImporterTool : EditorWindow
{
    private TextAsset jsonFile;
    private string outputPath = "Assets/ScriptableObject/Weapon/Bow";
    private Vector2 scrollPos;
    private bool showPreview = false;
    private WeaponDataCollection previewData;

    [MenuItem("Tools/Weapon Importer")]
    public static void ShowWindow()
    {
        GetWindow<WeaponImporterTool>("Weapon Importer");
    }

    void OnGUI()
    {
        GUILayout.Label("Weapon ScriptableObject Importer", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // JSON File Input
        EditorGUILayout.BeginVertical("box");
        jsonFile = (TextAsset)EditorGUILayout.ObjectField("JSON File", jsonFile, typeof(TextAsset), false);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // Output Path
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Output Settings", EditorStyles.boldLabel);
        outputPath = EditorGUILayout.TextField("Output Path", outputPath);
        
        if (GUILayout.Button("Select Output Folder"))
        {
            string selectedPath = EditorUtility.OpenFolderPanel("Select Output Folder", "Assets", "");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                if (selectedPath.StartsWith(Application.dataPath))
                {
                    outputPath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                }
                else
                {
                    EditorUtility.DisplayDialog("Invalid Path", "Please select a folder inside the Assets directory.", "OK");
                }
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // Preview Button
        GUI.enabled = jsonFile != null;
        if (GUILayout.Button("Preview Weapons", GUILayout.Height(30)))
        {
            PreviewWeapons();
        }
        GUI.enabled = true;

        // Preview Section
        if (showPreview && previewData != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField($"Preview: {previewData.Weapons.Count} Weapons Found", EditorStyles.boldLabel);
            
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(300));
            
            foreach (var weapon in previewData.Weapons)
            {
                EditorGUILayout.BeginVertical("helpbox");
                EditorGUILayout.LabelField($"Name: {weapon.Name}", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"Damage: {weapon.Damage} | Cooldown: {weapon.Cooldown}s | Range: {weapon.Range}");
                EditorGUILayout.LabelField($"Type: {weapon.Type} | Rarity: {weapon.Rarity} | Cost: {weapon.Cost}");
                
                if (weapon.Buffs != null && weapon.Buffs.Count > 0)
                {
                    EditorGUILayout.LabelField("Buffs:", EditorStyles.miniBoldLabel);
                    foreach (var buff in weapon.Buffs)
                    {
                        string valueStr = buff.BuffType == "Percentage" ? $"{buff.Value * 100}%" : buff.Value.ToString();
                        EditorGUILayout.LabelField($"  • {buff.StatType}: {valueStr} ({buff.BuffType})");
                    }
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);
            }
            
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space();

        // Import Button
        GUI.enabled = jsonFile != null;
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Import Weapons to ScriptableObjects", GUILayout.Height(40)))
        {
            ImportWeapons();
        }
        GUI.backgroundColor = Color.white;
        GUI.enabled = true;
    }

    private void PreviewWeapons()
    {
        if (jsonFile == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign a JSON file first.", "OK");
            return;
        }

        try
        {
            previewData = JsonUtility.FromJson<WeaponDataCollection>(jsonFile.text);
            showPreview = true;
            
            if (previewData == null || previewData.Weapons == null || previewData.Weapons.Count == 0)
            {
                EditorUtility.DisplayDialog("Error", "No weapons found in JSON file.", "OK");
                showPreview = false;
            }
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Parse Error", $"Failed to parse JSON:\n{e.Message}", "OK");
            showPreview = false;
        }
    }

    private void ImportWeapons()
    {
        if (jsonFile == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign a JSON file first.", "OK");
            return;
        }

        try
        {
            // Parse JSON
            WeaponDataCollection weaponData = JsonUtility.FromJson<WeaponDataCollection>(jsonFile.text);
            
            if (weaponData == null || weaponData.Weapons == null || weaponData.Weapons.Count == 0)
            {
                EditorUtility.DisplayDialog("Error", "No weapons found in JSON file.", "OK");
                return;
            }

            // Create output directory if it doesn't exist
            if (!AssetDatabase.IsValidFolder(outputPath))
            {
                string[] folders = outputPath.Split('/');
                string currentPath = folders[0];
                
                for (int i = 1; i < folders.Length; i++)
                {
                    string newPath = currentPath + "/" + folders[i];
                    if (!AssetDatabase.IsValidFolder(newPath))
                    {
                        AssetDatabase.CreateFolder(currentPath, folders[i]);
                    }
                    currentPath = newPath;
                }
            }

            int successCount = 0;
            List<string> createdAssets = new List<string>();

            // Create ScriptableObjects
            foreach (var weaponJson in weaponData.Weapons)
            {
                WeaponStats weapon = ScriptableObject.CreateInstance<WeaponStats>();
                
                // Basic Stats
                weapon.Name = weaponJson.Name;
                weapon.Damage = weaponJson.Damage;
                weapon.Cooldown = weaponJson.Cooldown;
                weapon.Range = weaponJson.Range;
                weapon.Cost = weaponJson.Cost;
                
                // Parse Enums
                if (System.Enum.TryParse(weaponJson.Type, out WeaponType weaponType))
                    weapon.Type = weaponType;
                
                if (System.Enum.TryParse(weaponJson.Rarity, out Rarity rarity))
                    weapon.Rarity = rarity;
                
                // Convert Buffs
                if (weaponJson.Buffs != null)
                {
                    weapon.Buffs = new List<StatBuff>() ;
                    foreach (var buffJson in weaponJson.Buffs)
                    {
                        StatBuff buff = new StatBuff();
                        
                        if (System.Enum.TryParse(buffJson.StatType, out StatType statType))
                            buff.Type = statType;
                        
                        buff.Value = buffJson.Value;
                        
                        if (System.Enum.TryParse(buffJson.BuffType, out BuffType buffType))
                            buff.BuffType = buffType;
                        
                        weapon.Buffs.Add(buff);
                    }
                }
                
                // Create asset file
                string sanitizedName = SanitizeFileName(weaponJson.Name);
                string assetPath = $"{outputPath}/{sanitizedName}.asset";
                
                // Check if asset already exists
                if (AssetDatabase.LoadAssetAtPath<WeaponStats>(assetPath) != null)
                {
                    if (!EditorUtility.DisplayDialog("Asset Exists", 
                        $"Asset '{sanitizedName}' already exists. Do you want to overwrite it?", 
                        "Yes", "Skip"))
                    {
                        continue;
                    }
                }
                
                AssetDatabase.CreateAsset(weapon, assetPath);
                createdAssets.Add(assetPath);
                successCount++;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Select created assets
            if (createdAssets.Count > 0)
            {
                Object[] assets = createdAssets.Select(path => AssetDatabase.LoadAssetAtPath<WeaponStats>(path)).ToArray();
                Selection.objects = assets;
                EditorGUIUtility.PingObject(assets[0]);
            }

            EditorUtility.DisplayDialog("Import Complete", 
                $"Successfully imported {successCount} weapon(s) to:\n{outputPath}", 
                "OK");
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Import Error", 
                $"Failed to import weapons:\n{e.Message}\n\nStack Trace:\n{e.StackTrace}", 
                "OK");
        }
    }

    private string SanitizeFileName(string fileName)
    {
        // Remove invalid characters from file name
        string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        foreach (char c in invalid)
        {
            fileName = fileName.Replace(c.ToString(), "");
        }
        return fileName;
    }
}

// Data classes for JSON deserialization
[System.Serializable]
public class WeaponDataCollection
{
    public List<WeaponJsonData> Weapons;
}

[System.Serializable]
public class WeaponJsonData
{
    public string Name;
    public float Damage;
    public float Cooldown;
    public float Range;
    public string Type;
    public string Rarity;
    public int Cost;
    public List<StatBuffJsonData> Buffs;
}

[System.Serializable]
public class StatBuffJsonData
{
    public string StatType;
    public float Value;
    public string BuffType;
}

// Note: These enums should match your actual game enums
// If they don't exist, you'll need to create them or adjust the parsing logic



