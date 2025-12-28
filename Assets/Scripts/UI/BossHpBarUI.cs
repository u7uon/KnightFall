using UnityEngine.UI ; 
using UnityEngine;
using System;

public class BossHpBarUI : MonoBehaviour
{
    [SerializeField] private Image hpImage ; 
    [Header("Animation Settings")]
    [SerializeField] private float smoothSpeed = 5f; // Tốc độ smooth khi exp thay đổi
    [SerializeField] private bool useSmooth = true; // Bật/tắt hiệu ứng smooth

    private float targetFillAmount ; 

    private float maxHp ; 
    private float currentHp ;


    void Awake()
    {
        Boss.OnBossSpawn += OnBossSpawn ; 
        Boss.OnHpChange += OnBossHPChange; 
        Boss.OnBossDie += OnBossDie ; 

        gameObject.SetActive(false);
    }

    private void OnBossDie()
    {
        this.gameObject.SetActive(false);
    }

    private void OnBossSpawn( float Hp )
    {
        maxHp = Hp ;
        currentHp = Hp ; 

        targetFillAmount = 1f;
    if (hpImage != null)
        hpImage.fillAmount = 1f;
        gameObject.SetActive(true); 
    }

    private void OnBossHPChange(float currentHp )
    {
        targetFillAmount = currentHp / maxHp; 
    }

    void OnDestroy()
    {
        Boss.OnBossSpawn -= OnBossSpawn;
        Boss.OnHpChange -= OnBossHPChange;
        Boss.OnBossDie -= OnBossDie;
    }


    // Update is called once per frame
    void Update()
    {
         // Smooth fill animation
        if (useSmooth && hpImage != null)
        {
            hpImage.fillAmount = Mathf.Lerp(hpImage.fillAmount, targetFillAmount, Time.deltaTime * smoothSpeed);
        }
    }
}
