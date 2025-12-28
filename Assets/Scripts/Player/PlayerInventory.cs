using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{

    private WeaponManager _playerWeapon;
    public static event Action<int> OnGoldChanged;
    public static event Action<List<WeaponStats>> OnWeaponChanged;
    public static event Action<ItemData> OnItemChanged;

    private List<ItemData> items = new();
    private List<WeaponStats> weapons = new();

    void Awake()
    {
        _playerWeapon = GetComponent<WeaponManager>();
    }
    
    private float Coins = 200;
    public List<WeaponStats> GetWeapons() => weapons;
    public List<ItemData> GetItems() => items; 
    public void AddCoin(int count)
    {
        Coins += count;
        OnGoldChanged?.Invoke((int)Coins); 
    }

    public void  PickUpCoins(int amount)
    {

        Coins += amount * PlayerStatsManager.Instance.GetCoinsMutil() ;
        OnGoldChanged?.Invoke((int)Coins); 
    }

    public void Spend(int count)
    {
        if (count > Coins) return;
        Coins -= count;
        OnGoldChanged?.Invoke((int)Coins);
    }

    public bool Sell(WeaponStats wp)
{
    if (weapons.Contains(wp))
    {
        weapons.Remove(wp);

        // Tìm instance thực tế trong WeaponManager
        var weaponInstance = _playerWeapon.GetWeaponInstanceByStats(wp);
        if (weaponInstance != null)
        {
            _playerWeapon.RemoveWeapon(weaponInstance);
        }

        PlayerStatsManager.Instance.DeBuffStat(wp.Buffs);

        var s = wp.Cost * 0.7f;
        AddCoin(Mathf.FloorToInt(s));

        OnWeaponChanged?.Invoke(weapons);
        return true;
    }
    return false;
}

    public bool AddWeapon(WeaponStats wp)
    {
        if(_playerWeapon == null)
        {
            return false;
        }

        if (_playerWeapon.AddWeapon(wp.weaponPrefab))
        {
            weapons.Add(wp);
            PlayerStatsManager.Instance.BuffStat(wp.Buffs);
            OnWeaponChanged?.Invoke(weapons);
            return true;
        }
        else
        {
            return false; 
        }
       
    }
    public void AddItem(ItemData item)
    {
        items.Add(item);
        PlayerStatsManager.Instance.BuffStat(item.StatModifiers);
        OnItemChanged?.Invoke(item);

    }
    
    public int GetBalance() => (int)Coins; 

}
