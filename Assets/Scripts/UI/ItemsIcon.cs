using UnityEngine.UI;
using UnityEngine;

public class ItemsIcon : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image frameImage; // Assign in inspector
    private ItemData itemData;

    public void SetData(ItemData data, Sprite frame)
    {
        itemData = data;
        frameImage.sprite = frame;
        // Update icon sprite
        if (iconImage != null && data != null && data.Icon != null)
        {
            iconImage.sprite = data.Icon;
            iconImage.preserveAspect = true;
        }
    }


}