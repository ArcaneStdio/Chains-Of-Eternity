using UnityEngine;

public class PlayerInventoryHandler : MonoBehaviour
{
    [Header("Inventory")]
    public InventoryObject inventory;
    public InventoryObject equipment;
    public InventoryObject potions;
    public HeroData heroData;

    private void Awake()
    {
        if (equipment != null )
        {
            // Debug.Log(equipment.GetSlots.Length);
            foreach (var slot in equipment.GetSlots)
            {
                // Debug.Log(slot.AllowedItems);
                slot.OnBeforeUpdate += OnBeforeSlotUpdate;
                slot.OnAfterUpdate += OnAfterSlotUpdate;
            }
        }
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     inventory.Save();
        //     equipment.Save();
        // }

        // if (Input.GetKeyDown(KeyCode.KeypadEnter))
        // {
        //     inventory.Load();
        //     equipment.Load();
        // }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("GroundItem"))
        {

            GroundItem groundItem = other.GetComponent<GroundItem>();
            if (groundItem != null && groundItem.item != null)
            {

                if (inventory.AddItem(new Item(groundItem.item), 1))
                {

                    Destroy(other.gameObject);
                }
            }
            else
            {
                Debug.LogWarning($"GroundItem {other.name} does not have a valid Item component.");
            }
        }
    }
    public void OnBeforeSlotUpdate(InventorySlot _slot)
    {
        if (_slot.ItemObject == null)
            return;

        if (_slot.parent.inventory.type == InterfaceType.Equipment)
        {
            Debug.Log($"Removed {_slot.ItemObject.name} from {_slot.parent.inventory.type}");
            ApplyItemToHero(_slot.item, false);
        }
    }

    public void OnAfterSlotUpdate(InventorySlot _slot)
    {
        if (_slot.ItemObject == null)
            return;

        if (_slot.parent.inventory.type == InterfaceType.Equipment)
        {
            Debug.Log($"Equipped {_slot.ItemObject.name} to {_slot.parent.inventory.type}");
            ApplyItemToHero(_slot.item, true);
        }
    }

    private void ApplyItemToHero(Item item, bool isEquipping)
    {
        int multiplier = isEquipping ? 1 : -1;

        switch (item.type)
        {
            case ItemType.Weapon:
                if (item.weaponData != null)
                {
                    heroData.offensiveStats.damage += multiplier * item.weaponData.damage;
                    heroData.offensiveStats.attackSpeed += Mathf.RoundToInt(multiplier * item.weaponData.attackSpeed);
                    heroData.offensiveStats.criticalRate += Mathf.RoundToInt(multiplier * item.weaponData.criticalRate);
                    heroData.offensiveStats.criticalDamage += Mathf.RoundToInt(multiplier * item.weaponData.criticalDamage);
                }
                break;

            case ItemType.Armour:
                if (item.armourData != null)
                {
                    heroData.defensiveStats.maxHealth += multiplier * item.armourData.maxHealth;
                    heroData.defensiveStats.defense += multiplier * item.armourData.defense;
                    heroData.defensiveStats.healthRegeneration += Mathf.RoundToInt(multiplier * item.armourData.healthRegeneration);

                    for (int i = 0; i < item.armourData.resistances.Count; i++)
                    {
                        if (i < heroData.defensiveStats.resistances.Count)
                            heroData.defensiveStats.resistances[i] += multiplier * item.armourData.resistances[i];
                    }
                }
                break;

            case ItemType.Accessory:
                if (item.accessoryData != null)
                {
                    heroData.specialStats.maxEnergy += multiplier * item.accessoryData.bonusEnergy;
                    heroData.specialStats.maxMana += multiplier * item.accessoryData.bonusMana;
                    heroData.specialStats.energyRegeneration += Mathf.RoundToInt(multiplier * item.accessoryData.bonusEnergyRegen);
                    heroData.specialStats.manaRegeneration += Mathf.RoundToInt(multiplier * item.accessoryData.bonusManaRegen);
                }
                break;

            case ItemType.Default:
            case ItemType.Consumable:
                break;
        }
    }

    private void OnApplicationQuit()
    {
        inventory.Clear();
        equipment.Clear();
    }

}
