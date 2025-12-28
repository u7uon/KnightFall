using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoinUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private PlayerInventory _playerInventory;


    void Awake()
    {
        _playerInventory = FindAnyObjectByType<PlayerInventory>();
        // Đăng ký event
        PlayerInventory.OnGoldChanged += UpdateGold;

    }

    void OnEnable()
    {
        UpdateGold(_playerInventory.GetBalance());  
    }

    void OnDestroy()
    {
        // Hủy đăng ký event
        PlayerInventory.OnGoldChanged -= UpdateGold;
    }

    private void UpdateGold(int gold)
    {
        if (goldText == null)
        {
            return;
        }
        else
        {
            goldText.text = gold.ToString();
        }
    }
}