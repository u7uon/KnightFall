using UnityEngine;
using UnityEngine.UI;
using TMPro; // Nếu dùng TextMeshPro

public class LocalizedText : MonoBehaviour
{
    [SerializeField] private string translationKey; // Key từ JSON
    
    private Text uiText; // UI Text
    private TextMeshProUGUI tmpText; // TextMeshPro

    
    
    void Start()
    {
        // Lấy component
        uiText = GetComponent<Text>();
        tmpText = GetComponent<TextMeshProUGUI>();
        
        // Subscribe event đổi ngôn ngữ
        LocalizationManager.Instance.OnLanguageChanged += UpdateText;
        
        // Update lần đầu
        UpdateText();
    }
    
    void UpdateText()
    {
        string translatedText = LocalizationManager.Instance.Get(translationKey);
        
        if (uiText != null)
            uiText.text = translatedText;
        
        if (tmpText != null)
            tmpText.text = translatedText;
    }
    
    void OnDestroy()
    {
        // Unsubscribe khi destroy
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged -= UpdateText;
    }
    
    // Đổi key runtime nếu cần
    public void SetKey(string key)
    {
        translationKey = key;
        UpdateText();
    }
}
