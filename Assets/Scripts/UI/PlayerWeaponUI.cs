using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWeaponUI : MonoBehaviour
{
    [SerializeField] private Sprite commonFrame;
    [SerializeField] private Sprite uncommonFrame;
    [SerializeField] private Sprite rareFrame;
    [SerializeField] private Sprite epicFrame;
    [SerializeField] private Sprite legendFrame;

    [SerializeField] private List<Image> currentWeaponImages;


    void OnEnable()
    {
        var playerInventory = FindAnyObjectByType<PlayerInventory>();
        if (playerInventory != null)
        {
            PlayerInventory.OnWeaponChanged += UpdateCurrentWeapon;
            UpdateCurrentWeapon(playerInventory.GetWeapons());
        }
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
