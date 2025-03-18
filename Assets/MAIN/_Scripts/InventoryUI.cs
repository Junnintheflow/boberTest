using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Image[] itemSlots; 
    public Sprite emptySlotSprite; 
    public static InventoryUI Instance;

    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        ClearInventoryUI();
    }

    public void UpdateInventory(Sprite[] itemIcons)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (i < itemIcons.Length && itemIcons[i] != null)
            {
                itemSlots[i].sprite = itemIcons[i]; 
            }
            else
            {
                itemSlots[i].sprite = emptySlotSprite; 
            }
        }
    }

    private void ClearInventoryUI()
    {
        foreach (Image slot in itemSlots)
        {
            slot.sprite = emptySlotSprite; 
        }
    }

    public void UpdateInventoryUI(int selectedIndex)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].color = (i == selectedIndex) ? Color.gray : Color.white;
        }
    }
}