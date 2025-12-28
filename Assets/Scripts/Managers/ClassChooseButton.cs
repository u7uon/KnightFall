using UnityEngine.UI;
using UnityEngine;

public class ClassChooseButton : MonoBehaviour
{
    private PlayerStats stats;

    [SerializeField] private Image iconImage;
    [SerializeField] private Image selectedImage;

    private Button button;


    public void Setup(PlayerStats stats)
    {
        button = GetComponent<Button>();

        iconImage.sprite = stats.Icon;
        this.stats = stats;
        button.onClick.AddListener(OnClick);
    }
    
    public PlayerStats GetStats() => stats;


    public void OnClick()
    {
        ClassChooseButton[] allButtons = transform.parent.GetComponentsInChildren<ClassChooseButton>();
        foreach(var btn in allButtons)
        {
           btn.Enable();
        }
        // Disable button hiện tại (đang được chọn)
        Disable();

        // Gọi hàm chọn class trong ClassChooser
        FindAnyObjectByType<ClassChooser>().onChosseClass(this);
        
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