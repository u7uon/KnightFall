using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class ShopUi : MonoBehaviour
{
    private PlayerInventory _playerInventory;
    [SerializeField] private TextMeshProUGUI commonRatio;
    [SerializeField] private TextMeshProUGUI unCommonRatio;
    [SerializeField] private TextMeshProUGUI rareRatio;
    [SerializeField] private TextMeshProUGUI epicRatio;
    [SerializeField] private TextMeshProUGUI legendRatio;
    [SerializeField] private TextMeshProUGUI coinBalanceText;
    [SerializeField] private List<Image> currentWeaponImages;

    [SerializeField] private Transform itemsContainer ;
    [SerializeField] private GameObject itemImagePrefab;

    [SerializeField] private Sprite commonFrame;
    [SerializeField] private Sprite uncommonFrame;
    [SerializeField] private Sprite rareFrame;
    [SerializeField] private Sprite epicFrame;
    [SerializeField] private Sprite legendFrame;



    void Awake()
    {
        _playerInventory = FindAnyObjectByType<PlayerInventory>();

        // Subscribe to events in Awake (these persist)
        PlayerInventory.OnWeaponChanged += UpdateCurrentWeapon;
        PlayerInventory.OnItemChanged += UpdateCurrentItem; 
        PlayerInventory.OnGoldChanged += UpdateCoinBalance;
        ShopManager.onRatioChange += UpdateRatito;
    }

    void OnDestroy()
    {
        // Unsubscribe on destroy
        PlayerInventory.OnWeaponChanged -= UpdateCurrentWeapon;
        PlayerInventory.OnItemChanged -= UpdateCurrentItem; 
        PlayerInventory.OnGoldChanged -= UpdateCoinBalance;
        ShopManager.onRatioChange -= UpdateRatito;
    }

    void OnEnable()
    {
        // Manually update UI when shop opens
        UpdateCoinBalance(_playerInventory.GetBalance());

        FindAnyObjectByType<PlayerStatPanel>()?.Refresh() ; 

        if (_playerInventory != null)
        {
            UpdateCurrentWeapon(_playerInventory.GetWeapons());
        }

    }

    private void UpdateCoinBalance(int coin)
    {
        if (coinBalanceText != null)
        {
            coinBalanceText.text = coin.ToString();
        }
        else
        {
        }
    }

    private void UpdateRatito(float com, float unco, float rare, float epic, float legend)
    {
        if (commonRatio != null) commonRatio.text = com.ToString("0.#") + "%";
        if (unCommonRatio != null) unCommonRatio.text = unco.ToString("0.#") + "%";
        if (rareRatio != null) rareRatio.text = rare.ToString("0.#") + "%";
        if (epicRatio != null) epicRatio.text = epic.ToString("0.#") + "%";
        if (legendRatio != null) legendRatio.text = legend.ToString("0.#") + "%";
    }

    private void UpdateCurrentWeapon(List<WeaponStats> weapons)
    {
        if (weapons == null )
        {
            return;
        }


        for (int i = 0; i < weapons.Count; i++)
        {
            if (i < currentWeaponImages.Count && currentWeaponImages[i] != null)
            {
                if (weapons[i] != null && weapons[i].Icon != null)
                {
                    currentWeaponImages[i].gameObject.SetActive(true);
                    currentWeaponImages[i].GetComponent<WeaponIcon>().SetData(weapons[i],GetFrameByRarity(weapons[i].Rarity));
                }
                else
                {
                }
            }
            else
            {
                break;
            }
        }

        // Hide unused weapon slots
        for (int i = weapons.Count; i < currentWeaponImages.Count; i++)
        {
            if (currentWeaponImages[i] != null)
            {
                currentWeaponImages[i].GetComponent<WeaponIcon>().ClearData();
            }
        }
    }


    private void UpdateCurrentItem(ItemData item)
    {
        if (item == null)
        {
            return;
        }
        var obj = Instantiate(itemImagePrefab, itemsContainer);
        obj.GetComponent<ItemsIcon>().SetData(item,GetFrameByRarity(item.Rarity));



    }
    
    private Sprite GetFrameByRarity(Rarity rarity)
    {
        return rarity switch
        {
            Rarity.Common => commonFrame,
            Rarity.Uncommon => uncommonFrame,
            Rarity.Rare => rareFrame,
            Rarity.Epic => epicFrame,
            Rarity.Legendary => legendFrame,
            _ => null,
        };
    }
    


}