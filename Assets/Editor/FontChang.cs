using UnityEditor;
using UnityEngine;
using TMPro;

public class TMPFontChanger : EditorWindow
{
    private TMP_FontAsset targetFont;
    private bool includeInactive = true;

    [MenuItem("Tools/TMP/Change Font In Scene")]
    public static void OpenWindow()
    {
        GetWindow<TMPFontChanger>("TMP Font Changer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Change TMP Font In Current Scene", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        targetFont = (TMP_FontAsset)EditorGUILayout.ObjectField(
            "Target Font",
            targetFont,
            typeof(TMP_FontAsset),
            false
        );

        includeInactive = EditorGUILayout.Toggle(
            "Include Inactive Objects",
            includeInactive
        );

        EditorGUILayout.Space();

        GUI.enabled = targetFont != null;
        if (GUILayout.Button("Change All TMP Fonts"))
        {
            ChangeAllTMPFonts();
        }
        GUI.enabled = true;
    }

    private void ChangeAllTMPFonts()
    {
        int count = 0;

        // UI Text
        var tmpUIs = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
        foreach (var tmp in tmpUIs)
        {
            if (!includeInactive && !tmp.gameObject.activeInHierarchy)
                continue;

            Undo.RecordObject(tmp, "Change TMP Font");
            tmp.font = targetFont;
            EditorUtility.SetDirty(tmp);
            count++;
        }

        // World Text
        var tmpWorlds = Resources.FindObjectsOfTypeAll<TextMeshPro>();
        foreach (var tmp in tmpWorlds)
        {
            if (!includeInactive && !tmp.gameObject.activeInHierarchy)
                continue;

            Undo.RecordObject(tmp, "Change TMP Font");
            tmp.font = targetFont;
            EditorUtility.SetDirty(tmp);
            count++;
        }

        Debug.Log($"TMP Font Changer: Updated {count} TMP components.");
    }
}
