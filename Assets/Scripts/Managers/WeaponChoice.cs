using UnityEngine.UI;
using UnityEngine;

public class WeaponChooseButton : MonoBehaviour
{
    private WeaponStats stats;

    [SerializeField] private Image iconImage;
    [SerializeField] private Image selectedImage;

    private Button button;
    

    public void Setup(WeaponStats stats)
    {
        button = GetComponent<Button>();
        iconImage.sprite = stats.Icon;
        this.stats = stats;
        button.onClick.AddListener(OnClick);
    }
    
    public WeaponStats GetStats() => stats;


    public void OnClick()
    {
        WeaponChooseButton[] allButtons = transform.parent.GetComponentsInChildren<WeaponChooseButton>();
        foreach(var btn in allButtons)
        {
           btn.Enable();
        }
        // Disable button hiện tại (đang được chọn)
        Disable();

        // Gọi hàm chọn class trong ClassChooser
        FindAnyObjectByType<WeaponChooser>().onChosseClass(this);
    }

    void Disable()
    {
        selectedImage.enabled = false;
        button.interactable = false;
    }

    void Enable()
    {
        selectedImage.enabled = true;
        button.interactable = true;
    }
}