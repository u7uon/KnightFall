using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class TranslationData
{
    public Dictionary<string, string> en;
    public Dictionary<string, string> vi;
}

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;
    
    private Dictionary<string, string> currentLanguageDict;
    private TranslationData allTranslations;
    private string currentLanguage;

    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadTranslations();
        }
        else
        {
            Destroy(gameObject);
        }

        currentLanguage = PlayerPrefs.GetString("Language", "en");
    }
    
    void LoadTranslations()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("translations");
        
        if (jsonFile != null)
        {
            // Parse JSON với Newtonsoft
            allTranslations = JsonConvert.DeserializeObject<TranslationData>(jsonFile.text);
            
            // Load ngôn ngữ đã lưu hoặc mặc định
            string savedLang = PlayerPrefs.GetString("Language", "en");
            SetLanguage(savedLang);
            
        }
        else
        {
        }
    }
    
    // Lấy text theo key - NHANH với Dictionary O(1)
    public string Get(string key)
    {
        if (currentLanguageDict == null)
        {
            return key;
        }
        
        // TryGetValue nhanh hơn ContainsKey + []
        if (currentLanguageDict.TryGetValue(key, out string value))
        {
            return value;
        }
        
        return key;
    }
    
    // Lấy text với fallback
    public string Get(string key, string fallback)
    {
        if (currentLanguageDict != null && currentLanguageDict.TryGetValue(key, out string value))
        {
            return value;
        }
        return fallback;
    }
    
    // Lấy text với format parameters
    public string GetFormat(string key, params object[] args)
    {
        string text = Get(key);
        return string.Format(text, args);
    }
    
    // Đổi ngôn ngữ
    public void SetLanguage(string lang)
    {
        currentLanguage = lang;
        
        // Switch dictionary pointer - CỰC NHANH!
        currentLanguageDict = lang == "vi" ? allTranslations.vi : allTranslations.en;
        
        // Lưu preference
        PlayerPrefs.SetString("Language", lang);
        PlayerPrefs.Save();
        
        // Notify UI update
        OnLanguageChanged?.Invoke();
        
    }
    
    public string GetCurrentLanguage()
    {
        return currentLanguage;
    }
    
    // Check key có tồn tại không
    public bool HasKey(string key)
    {
        return currentLanguageDict != null && currentLanguageDict.ContainsKey(key);
    }
    
    // Event để UI components subscribe
    public System.Action OnLanguageChanged;
}