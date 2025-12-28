using UnityEngine;
using UnityEngine.UI;

public class LanguageManager : MonoBehaviour
{

    [Header("UI References")]
    public Toggle englishToggle;
    public Toggle vietnameseToggle;

    [Header("Language Settings")]
    public string language = "en"; // "en" = English, "vi" = Vietnamese


    void Start()
    {
        // Load ngôn ngữ đã lưu
        language = PlayerPrefs.GetString("Language", "en");
        PlayerPrefs.SetString("Language", language);
        
        // Set toggle state theo ngôn ngữ đã lưu
        if (language == "en")
        {
            englishToggle.isOn = true;
            vietnameseToggle.isOn = false;
        }
        else
        {
            englishToggle.isOn = false;
            vietnameseToggle.isOn = true;
        }
        
        // Đăng ký sự kiện
        englishToggle.onValueChanged.AddListener(OnEnglishToggleChanged);
        vietnameseToggle.onValueChanged.AddListener(OnVietnameseToggleChanged);
        
        // Áp dụng ngôn ngữ

    }
    
    public void OnEnglishToggleChanged(bool isOn)
    {
        if (isOn)
        {
            language = "en";
            LocalizationManager.Instance.SetLanguage("en");
            vietnameseToggle.isOn = false; // Tắt toggle kia (radio button behavior)
            PlayerPrefs.SetString("Language", "en");
            PlayerPrefs.Save();

        }
    }
    
    public void OnVietnameseToggleChanged(bool isOn)
    {
        if (isOn)
        {
            language = "vi";
            englishToggle.isOn = false; // Tắt toggle kia (radio button behavior)
            LocalizationManager.Instance.SetLanguage("vi");
            PlayerPrefs.SetString("Language", "vi");
            PlayerPrefs.Save();


        }
    }
    

}