using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private List<Transform> slotPos; // 6 vị trí quanh player

    private List<WeaponBase> weapons = new();
    private Dictionary<WeaponBase, int> weaponSlotIndex = new(); // lưu vị trí slot của từng vũ khí

    private const int MAX_WEAPON_COUNT = 6;

    private void Update()
    {
        foreach (var weapon in weapons)
        {
            weapon.Tick(Time.deltaTime);
        }
    }

    public bool AddWeapon(WeaponBase weaponPrefab)
    {
        // Giới hạn số lượng
        if (weapons.Count >= MAX_WEAPON_COUNT)
        {
            return false;
        }

        // Tìm slot trống đầu tiên
        int freeSlot = GetFirstEmptySlot();
        if (freeSlot == -1)
        {
            return false;
        }

        Transform slot = slotPos[freeSlot];
        WeaponBase weapon = Instantiate(weaponPrefab, slot.position, slot.rotation, slot);
        weapon.Init(transform, enemyLayer);

        weapons.Add(weapon);
        weaponSlotIndex[weapon] = freeSlot;

        return true;
    }

    public void RemoveWeapon(WeaponBase weapon)
    {
        if (weapon == null)
        {
            return;
        }

        if (weapons.Contains(weapon))
        {
            int slot = weaponSlotIndex.ContainsKey(weapon) ? weaponSlotIndex[weapon] : -1;
            weapons.Remove(weapon);
            weaponSlotIndex.Remove(weapon);

            if (weapon != null)
                Destroy(weapon.gameObject);

        }
        else
        {
        }
    }

    public WeaponBase GetWeaponInstanceByStats(WeaponStats stats)
    {
        return weapons.FirstOrDefault(w => w.weaponStats == stats);
    }

    public void RemoveAllWeapons()
    {
        foreach (var w in weapons)
        {
            Destroy(w.gameObject);
        }
        weapons.Clear();
        weaponSlotIndex.Clear();
    }

    public List<WeaponStats> getCurrentWeapons() => weapons.Select(x => x.weaponStats).ToList();



    private int GetFirstEmptySlot()
    {
        var usedSlots = weaponSlotIndex.Values.ToHashSet();
        for (int i = 0; i < slotPos.Count; i++)
        {
            if (!usedSlots.Contains(i))
                return i;
        }
        return -1;
    }
}
