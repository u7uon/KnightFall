using UnityEngine;

using UnityEngine.UI;

public class DragonHealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image fillImage;     

    private float currentFillAmount;
    private float targetFillAmount;

    void Start()
    {
        PlayerStatsManager.OnHealthChanged += UpdateHealthBar;
        
        // Khởi tạo full health
        currentFillAmount = 1f;
        targetFillAmount = 1f;
        fillImage.fillAmount = 1f;
    }

    void OnDestroy()
    {
        PlayerStatsManager.OnHealthChanged -= UpdateHealthBar;
    }


    private void UpdateHealthBar(float health, float maxHealth)
    {
        targetFillAmount = Mathf.Clamp01(health / maxHealth);
        currentFillAmount = targetFillAmount;
        fillImage.fillAmount = currentFillAmount;

    }


}

