using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SurvivalStatUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI classNameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI timeText;

    void OnEnable()
    {
        UpdateInfo();
    }

    private void UpdateInfo()
    {
        UpdateClassName();
        UpdateLevel();
        UpdateTimeSurvived();
    }
    private void UpdateClassName()
    {
        var player = PlayerStatsManager.Instance ; 
        if( player == null ) return ;
        classNameText.text = LocalizationManager.Instance.Get( player.GetPlayerClassName());
    }
    private void UpdateLevel()
    {
        var level = FindAnyObjectByType<PlayerLevel>();
        if (level == null) return;
        
        levelText.text =  level.GetCurrentLevel().ToString();
    }
    private void UpdateTimeSurvived()
    {
        
        var stageManager = FindAnyObjectByType<StageManager>();
        if (stageManager == null) return;

 
        var timeSurvived = stageManager.GetTimeSurvived();
        int minutes = Mathf.FloorToInt(timeSurvived / 60f);
        int seconds = Mathf.FloorToInt(timeSurvived % 60f);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
