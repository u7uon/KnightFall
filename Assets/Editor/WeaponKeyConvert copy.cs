using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class ItemLocalizationConvertera
{
    [MenuItem("Tools/Localization/Convertt")]
    public static void ConvertAllItems()
    {
        string folderPath = "Assets/ScriptableObject/Weapon/Bow";
        string exportPath = "Assets/Bowweapon_localization_export.json";

        string[] guids = AssetDatabase.FindAssets("t:WeaponStats", new[] { folderPath });
        if (guids.Length == 0)
        {
            Debug.LogWarning("⚠️ No ItemData found in " + folderPath);
            return;
        }

        // JSON dictionary: key -> value (English source text)
        Dictionary<string, string> enDict = new Dictionary<string, string>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            WeaponStats item = AssetDatabase.LoadAssetAtPath<WeaponStats>(path);
            if (item == null) continue;

            // ✅ Generate base key (remove spaces)
            string baseName = item.Name.Replace(" ", "").ToUpper();

            string nameKey = $"ITEM_{baseName}_NAME";
            string descKey = $"ITEM_{baseName}_DESC";

            // Add to export dictionary
            enDict[nameKey] = item.Name;

            // ✅ Update ItemData
            item.Name = nameKey;

            EditorUtility.SetDirty(item);
            Debug.Log($"✅ Converted {item.Name} → {nameKey}");
        }

        // Save & refresh
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Export JSON
        WriteJson(exportPath, enDict);
        Debug.Log($"🎉 Exported {enDict.Count} entries to {exportPath}");
    }

    private static void WriteJson(string path, Dictionary<string, string> dict)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("{");
        int i = 0;
        foreach (var kvp in dict)
        {
            string line = $"  \"{kvp.Key}\": \"{kvp.Value.Replace("\"", "\\\"")}\"";
            if (i < dict.Count - 1) line += ",";
            sb.AppendLine(line);
            i++;
        }
        sb.AppendLine("}");

        File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        AssetDatabase.Refresh();
    }
}
