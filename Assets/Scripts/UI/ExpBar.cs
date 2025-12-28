using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI lvText; 
    
    [Header("Animation Settings")]
    [SerializeField] private float smoothSpeed = 5f; // Tốc độ smooth khi exp thay đổi
    [SerializeField] private bool useSmooth = true; // Bật/tắt hiệu ứng smooth

    private float currentExp;
    private float expToNextLv;
    private float targetFillAmount;

    void Start()
    {
        // Đăng ký sự kiện
        PlayerLevel.OnExpChange += UpdateExpBar;
        PlayerLevel.OnlevelUp += ResetExp;

        if (lvText != null)
            lvText.text = "lv. 0"; 
        
    }

    void OnDestroy()
    {
        // Hủy đăng ký sự kiện
        PlayerLevel.OnExpChange -= UpdateExpBar;
        PlayerLevel.OnlevelUp -= ResetExp; 
    }

    void Update()
    {
        // Smooth fill animation
        if (useSmooth && fillImage != null)
        {
            fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, targetFillAmount, Time.deltaTime * smoothSpeed);
        }
    }

    private void UpdateExpBar(float currentExp, float expToNext)
    {
        if (fillImage == null) return;

        this.currentExp = currentExp;
        this.expToNextLv = expToNext;

        // Tính toán phần trăm exp
        float fillAmount = expToNext > 0 ? currentExp / expToNext : 0f;
        fillAmount = Mathf.Clamp01(fillAmount); // Giới hạn từ 0 đến 1

        if (useSmooth)
        {
            targetFillAmount = fillAmount;
        }
        else
        {
            fillImage.fillAmount = fillAmount;
        }
    }

    // Reset về 0 (khi level up)
    public void ResetExp(int lv)
    {
        if(lvText != null)
        {
            lvText.text = "lv. " + lv; 
        }

        if (fillImage != null)
        {
            fillImage.fillAmount = 0f;
            targetFillAmount = 0f;
        }
    }
}