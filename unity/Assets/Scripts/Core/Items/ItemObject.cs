using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum ItemType
{
    Weapon,
    Armour,
    Consumable,
    Accessory,
    Default
}

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

public enum ArmourSlot
{
    Helmet,
    Chest,
    Leggings,
    Boots
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Items/Item")]
public class ItemObject : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;
    public Sprite uiDisplay;
    public Rarity rarity;
    public bool stackable;
    public ItemType type;
    [TextArea(3, 10)]
    public string description;

    public ulong NFTID;
    public Item data = new Item();
    public float itemCost;
    public int maxItemStock;

    [Header("Type-specific Data")]
    public WeaponData weaponData;
    public ArmourData armourData;
    public ConsumableData consumableData;
    public AccessoryData accessoryData;

    private static int nextId = 0;
    public int id = -1;

    private void OnEnable()
    {
        if (id == -1)
            id = nextId++;
    }

    public Item CreateItem()
    {
        return new Item(this);
    }
}

[System.Serializable]
public class Item
{
    public string Name;
    public int Id = -1;
    public ItemType type;
    public Rarity rarity;
    public string description;

    public WeaponData weaponData;
    public ArmourData armourData;
    public ConsumableData consumableData;
    public AccessoryData accessoryData;
    public DefaultData defaultData;

    public Item() { Id = -1; }

    public Item(ItemObject item)
    {
        Name = item.itemName;
        Id = item.id;
        type = item.type;
        rarity = item.rarity;
        description = item.description;

        switch (type)
        {
            case ItemType.Weapon:
                weaponData = item.weaponData.Clone();
                break;
            case ItemType.Armour:
                armourData = item.armourData.Clone();
                break;
            case ItemType.Consumable:
                consumableData = item.consumableData.Clone();
                break;
            case ItemType.Accessory:
                accessoryData = item.accessoryData.Clone();
                break;
            case ItemType.Default:
                defaultData = new DefaultData
                {
                    name = item.itemName,
                    notes = item.description
                };
                break;
        }
    }
}
