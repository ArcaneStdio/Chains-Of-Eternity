using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ShopUIManager : MonoBehaviour
{
    [Header("Item Slots (Right Side)")]
    public ItemSlotUI[] itemSlots = new ItemSlotUI[4];

    [Header("Item Details (Left Side)")]
    public Image detailImage;
    public TMP_Text detailName;
    public TMP_Text detailRarity;
    public TMP_Text detailDescription;
    public TMP_Text detailCost;

    [Header("Default Images (Type Based)")]
    public Sprite[] weaponImages;
    public Sprite[] armourImages;
    public Sprite[] consumableImages;
    public Sprite[] accessoryImages;
    public Sprite defaultImage;

    [Header("Spawning Settings")]
    public Transform spawnPoint;             // where to spawn items
    public GameObject defaultPrefab;         // fallback prefab
    private ItemObject currentSelectedItem;  // currently selected item

    private void Start()
    {
        foreach (var slot in itemSlots)
        {  slot.assignedItem.uiDisplay = GetSpriteForItem(slot.assignedItem);
            if (slot != null)
                slot.Setup(this);
        }
    }

    public void Refresh()
    {
        foreach (var slot in itemSlots)
        {
            if (slot != null)
                slot.Setup(this);
        }
    }

    public Sprite GetSpriteForItem(ItemObject item)
    {
        switch (item.type)
        {
            case ItemType.Weapon: return PickRandom(weaponImages);
            case ItemType.Armour: return PickRandom(armourImages);
            case ItemType.Consumable: return PickRandom(consumableImages);
            case ItemType.Accessory: return PickRandom(accessoryImages);
            default: return defaultImage;
        }
    }

    private Sprite PickRandom(Sprite[] pool)
    {
        if (pool != null && pool.Length > 0)
            return pool[Random.Range(0, pool.Length)];
        return defaultImage;
    }

    public void ShowItemDetails(ItemObject item)
    {
        if (item == null) return;

        currentSelectedItem = item; // save currently selected item

        detailImage.sprite = item.uiDisplay;
        detailName.text = item.itemName;
        detailRarity.text = item.rarity.ToString();
        detailDescription.text = item.description;
        detailCost.text = "Cost: " + item.itemCost.ToString();
    }

    public void BuyItem()
    {
        if (currentSelectedItem == null) return;

        defaultPrefab.GetComponent<GroundItem>().item = currentSelectedItem;
        defaultPrefab.GetComponentInChildren<SpriteRenderer>().sprite = currentSelectedItem.uiDisplay;
        // Load prefab from the ItemObject, or fallback to default
        GameObject prefabToSpawn =  defaultPrefab; // Extend this if ItemObject has its own prefab reference


        //Acutally buying items
        Web3AuthManager.Instance.BuyItem(currentSelectedItem.NFTID, currentSelectedItem.itemCost);
        if (prefabToSpawn != null && spawnPoint != null)
        {
            Instantiate(prefabToSpawn, spawnPoint.position + new Vector3(2, 2, 0), Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Missing prefab or spawnPoint for item: " + currentSelectedItem.itemName);
        }
    }
}

[System.Serializable]
public class ItemSlotUI
{
    [Header("Slot References")]
    public Button button;
    public Image itemImage;

    [Header("Assigned Item")]
    public ItemObject assignedItem;

    private ShopUIManager manager;

    public void Setup(ShopUIManager mgr)
    {
        manager = mgr;

        if (assignedItem != null && itemImage != null)
        {
            itemImage.sprite = assignedItem.uiDisplay;
        }

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                manager.ShowItemDetails(assignedItem);
            });
        }
    }
}
