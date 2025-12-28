using UnityEngine;
using UnityEngine.UI;

public class WeaponIcon : MonoBehaviour
{
    [SerializeField] private Image iconImage; // Assign in inspector
    [SerializeField] private Button button; // Assign in inspector
    [SerializeField] private Image frameImage; // Assign in inspector
     private bool isSelected = false;
    private WeaponStats weaponData;

    void Awake()
    {
        // Get button if not assigned
        if (button == null)
        {
            button = GetComponent<Button>();
        }
        // Setup button listener
        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }
    }

    void OnDestroy()
    {
        // Cleanup listener
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    public void SetData(WeaponStats data, Sprite frame)
    {
        weaponData = data;
        frameImage.sprite = frame;
        frameImage.gameObject.SetActive(true);
        // Update icon sprite
        if (iconImage != null && data != null && data.Icon != null)
        {
            iconImage.sprite = data.Icon;
        }
    }

    public void ClearData()
    {
        weaponData = null;
        iconImage.sprite = null;
        frameImage.sprite = null;
        frameImage.gameObject.SetActive(false);
        Deselect();
    }

    private void OnClick()
    {
        // Nếu đang được chọn thì bỏ chọn và đóng dialog
        if (isSelected)
        {
            Deselect();
            if (WeaponDialog.Instance != null)
                WeaponDialog.Instance.Close();
            return;
        }

        // Chọn icon hiện tại
        Select();

        // Hiển thị dialog
        if (WeaponDialog.Instance == null)
        {
            var dialogObj = FindAnyObjectByType<WeaponDialog>();
            if (dialogObj != null)
            {
                dialogObj.gameObject.SetActive(true);
            }
            else
            {
                return;
            }
        }

        WeaponDialog.Instance.Open(weaponData);
    }

    private void Select()
    {
        isSelected = true;
    }

    private void Deselect()
    {
        isSelected = false;
    }
}