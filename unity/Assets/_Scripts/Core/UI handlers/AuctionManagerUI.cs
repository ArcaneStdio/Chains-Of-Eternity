using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class AuctionUIManager : MonoBehaviour
{
    [Header("UI References")]
    public Image selectedItemImage;
    public TMP_Text itemNameText;
    public TMP_Text itemDescriptionText;
    public TMP_InputField basePriceInput;
    public TMP_InputField durationInput;
    public TMP_Text feedbackText; // optional for user feedback

    [Header("Dump Target")]
    public ItemObject selectedItem;  // ✅ Temporary container for currently selected item

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

        // ✅ Copy core item data
        selectedItem.itemName = source.itemName;
        selectedItem.uiDisplay = source.uiDisplay;
        selectedItem.rarity = source.rarity;
        selectedItem.stackable = source.stackable;
        selectedItem.type = source.type;
        selectedItem.description = source.description;
        selectedItem.itemCost = source.itemCost;
        selectedItem.maxItemStock = source.maxItemStock;
        selectedItem.NFTID = source.NFTID;

        // ✅ Deep clone type-specific data
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
        }

        // ✅ Update UI
        itemNameText.text = $"{selectedItem.itemName.ToUpper()}";
        itemDescriptionText.text = $"{selectedItem.description}";
        selectedItemImage.sprite = selectedItem.uiDisplay;
        selectedItemImage.color = Color.white;
        basePriceInput.text = selectedItem.itemCost.ToString();
        durationInput.text = "";
    }

    public void ConfirmAuction()
    {
        if (selectedItem == null) return;

        bool validPrice = float.TryParse(basePriceInput.text, out float basePrice);
        bool validDuration = int.TryParse(durationInput.text, out int duration);

        if (!validPrice || basePrice <= 0)
        {
            Debug.LogWarning("❌ Invalid base price entered!");
            if (feedbackText) feedbackText.text = "Invalid Base Price!";
            return;
        }

        if (!validDuration || duration <= 0)
        {
            Debug.LogWarning("❌ Invalid duration entered!");
            if (feedbackText) feedbackText.text = "Invalid Duration!";
            return;
        }

        // ✅ Save to selected item (extend ItemObject with auction fields if needed)
        selectedItem.itemCost = basePrice;

        // Example: you may want to add custom auction fields in ItemObject
        // selectedItem.auctionDuration = duration;

        Debug.Log($"✅ Auction created for {selectedItem.itemName} | Price: {basePrice} | Duration: {duration}");
        if (feedbackText) feedbackText.text = $"Auction started!";
    }
}
