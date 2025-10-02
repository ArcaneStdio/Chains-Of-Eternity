using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SellUIManager : MonoBehaviour
{
    [Header("UI References")]
    public Image selectedItemImage;
    public TMP_Text recommendedPriceText;
    public TMP_Text itemnameText;
    public TMP_Text itemnameDescription;
    public TMP_InputField customPriceInput;

    [Header("Dump Target")]
    public ItemObject selectedItem;  // ✅ Assigned in inspector, acts as "dump storage"

    private InventorySlot currentSelectedSlot;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }

    void HandleClick()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            foreach (UserInterface ui in FindObjectsOfType<UserInterface>())
            {
                if (ui.slotsOnInterface.ContainsKey(result.gameObject))
                {
                    InventorySlot clickedSlot = ui.slotsOnInterface[result.gameObject];
                    if (clickedSlot.item.Id >= 0)
                    {
                        DumpItem(clickedSlot.ItemObject);
                    }
                    return;
                }
            }
        }
    }

    void DumpItem(ItemObject source)
    {
        if (source == null || selectedItem == null) return;

        // ✅ Copy basic info
        selectedItem.itemName = source.itemName;
        selectedItem.uiDisplay = source.uiDisplay;
        selectedItem.rarity = source.rarity;
        selectedItem.stackable = source.stackable;
        selectedItem.type = source.type;
        selectedItem.description = source.description;
        selectedItem.itemCost = source.itemCost;
        selectedItem.maxItemStock = source.maxItemStock;
        selectedItem.NFTID = source.NFTID;

        // ✅ Copy type-specific data (deep clone where possible)
        switch (source.type)
        {
            case ItemType.Weapon:
                selectedItem.weaponData = source.weaponData?.Clone();
                break;
            case ItemType.Armour:
                selectedItem.armourData = source.armourData?.Clone();
                break;
            case ItemType.Consumable:
                selectedItem.consumableData = source.consumableData?.Clone();
                break;
            case ItemType.Accessory:
                selectedItem.accessoryData = source.accessoryData?.Clone();
                break;
            case ItemType.Default:
                // leave as is, no extra data
                break;
        }

        // ✅ Update UI
        itemnameText.text = $"NAME: {selectedItem.itemName.ToUpper()}";
        itemnameDescription.text = $"Description: {selectedItem.description}";
        selectedItemImage.sprite = selectedItem.uiDisplay;
        selectedItemImage.color = Color.white;
        recommendedPriceText.text = $"${selectedItem.itemCost}";
        customPriceInput.text = "";
    }

    public void ConfirmPrice()
    {
        if (selectedItem == null) return;

        if (float.TryParse(customPriceInput.text, out float enteredPrice))
        {
            selectedItem.itemCost = enteredPrice; // ✅ overwrite dump object cost
            Debug.Log($"✅ Item {selectedItem.itemName} set to sell for {enteredPrice} gold!");
        }
        else
        {
            Debug.LogWarning("❌ Invalid price entered!");
        }
    }
}
