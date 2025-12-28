using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponChooser : MonoBehaviour
{
    [SerializeField] private WeaponDatabase weaponDatabase;
    [SerializeField] private GameObject chooseClass ;
    [SerializeField] private WeaponChooseButton btn;
    [SerializeField] private Transform classButtonParent;
    [SerializeField] private Button backButton ; 
    [SerializeField] private Button confirmButton;
    private WeaponChooseButton choosedButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        confirmButton.onClick.AddListener(ConfirmChoice);
        backButton.onClick.AddListener(Back) ; 

        for (int i = 0; i <= 4; i++)
        {
            var button = Instantiate(btn, classButtonParent);
            button.Setup(GetUniqueRandomWeapon());
        }
        WeaponChooseButton firstButton = classButtonParent.GetComponentInChildren<WeaponChooseButton>();
        if (firstButton != null)
        {
            firstButton.OnClick();
        }

        usedWeapons.Clear();
    }

    private List<WeaponStats> usedWeapons = new List<WeaponStats>();

    WeaponStats GetUniqueRandomWeapon()
    {
        var weapon = weaponDatabase.GetRandomWeaponByRarity(Rarity.Common);
        if (usedWeapons.Contains(weapon))
        {
            return GetUniqueRandomWeapon();
        }
        usedWeapons.Add(weapon);
        return weapon;
    }
    



    public void onChosseClass(WeaponChooseButton button)
    {
        choosedButton = button;
    }


    public void ConfirmChoice()
    {
        if (choosedButton == null) return;
        FindAnyObjectByType<GameManager>().SetStartingWeapon(choosedButton.GetStats());
        this.gameObject.SetActive(false);
    }

    private void Back()
    {
        gameObject.SetActive(false);
        chooseClass.SetActive(true);
    }



}
