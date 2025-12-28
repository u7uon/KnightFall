using Unity.VisualScripting;
using UnityEngine;

public class PlayerItemsUI : MonoBehaviour
{
    [SerializeField] private Transform itemsContainer ;
    [SerializeField] private GameObject itemImagePrefab;
    [SerializeField] private Sprite commonFrame;
    [SerializeField] private Sprite uncommonFrame;
    [SerializeField] private Sprite rareFrame;
    [SerializeField] private Sprite epicFrame;
    [SerializeField] private Sprite legendFrame;

    void OnEnable()
    {
        var playerInventory = FindAnyObjectByType<PlayerInventory>();
        if (playerInventory != null)
        {
            foreach(var item in playerInventory.GetItems())
            {
                UpdateCurrentItem(item);
            }
        }
    }

    void OnDisable()
    {
        foreach (Transform child in itemsContainer)
        {
            Destroy(child.gameObject);
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
