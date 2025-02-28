using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    public Image shop_frame; // Reference to the UI Image component
    public Sprite[] itemSprites; // Array of item sprites

    private int currentItemIndex;

    void Start()
    {
        RerollItem(); // Initialize with a random item
    }

    public void RerollItem()
    {
        currentItemIndex = Random.Range(0, itemSprites.Length);
        shop_frame.sprite = itemSprites[currentItemIndex];
    }

    // Call this method when the item is bought
    public void OnItemBought()
    {
        RerollItem();
    }
}