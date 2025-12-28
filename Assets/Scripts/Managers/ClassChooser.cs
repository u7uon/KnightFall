using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ClassChooser : MonoBehaviour
{

    [SerializeField] private List<PlayerStats> characterClasses;

    [SerializeField] private ClassChooseButton btn;

    [SerializeField] private GameObject startWeaponChooser ;

    [SerializeField] private Transform classButtonParent;
    
    [SerializeField] private Button confirmButton;

    [SerializeField] private TextMeshProUGUI classNameText;
    [SerializeField] private TextMeshProUGUI classDescriptionText;

    private ClassChooseButton choosedButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        confirmButton.onClick.AddListener(ConfirmChoice);
        foreach (var characterClass in characterClasses)
        {
            ClassChooseButton button = Instantiate(btn, classButtonParent);
            button.Setup(characterClass);

        }
        // Tự động chọn class đầu tiên
        ClassChooseButton firstButton = classButtonParent.GetComponentInChildren<ClassChooseButton>();
        if (firstButton != null)
        {
            firstButton.OnClick();
        }
    }

    public void onChosseClass(ClassChooseButton button)
    {
        choosedButton = button;
        classNameText.text = LocalizationManager.Instance.Get(button.GetStats().Name);
        classDescriptionText.text = LocalizationManager.Instance.Get(button.GetStats().Description) ;
    }


    public void ConfirmChoice()
    {
        if (choosedButton == null) return;


        FindAnyObjectByType<GameManager>().SetCharacter(choosedButton.GetStats());

        this.gameObject.SetActive(false);
        
        startWeaponChooser.SetActive(true);

    }

    void OnDestroy()
    {
        confirmButton.onClick.RemoveListener(ConfirmChoice);
    }



}
