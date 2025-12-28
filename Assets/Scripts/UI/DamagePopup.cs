using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public TextMeshProUGUI text;
    public CanvasGroup canvasGroup;

    private float speed = 50f; // Tăng speed vì đây là UI space
    private float fadeSpeed = 2f;
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Setup(float damage, Color color)
    {
        text.text = damage == 0 ? LocalizationManager.Instance.Get("DODGE") :  damage.ToString("F2"); // Format số nguyên
        text.fontSize =  20 ;
        text.color = color;

        canvasGroup.alpha = 1f;
        rectTransform.localScale = Vector3.one * 1f;
    }

    void Update()
    {
        // Di chuyển lên trên trong UI space
        rectTransform.anchoredPosition += Vector2.up * speed * Time.deltaTime;
        
        canvasGroup.alpha -= fadeSpeed * Time.deltaTime;

        if (canvasGroup.alpha <= 0)
            DamagePopupPool.Instance.ReturnToPool(this);
    }
}