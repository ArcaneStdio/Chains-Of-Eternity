using TMPro;
using UnityEngine;

public class potionSystem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject slot1;
    [SerializeField] private GameObject slot2;
    [SerializeField] private GameObject slot3;

    [SerializeField] private InventoryObject potionInventory;
    [SerializeField] private Item empty;
    [SerializeField] private PlayerStats playerstats; // Reference to the Player script
    private Item[] potionItems; // Array to hold potion items
    private InventorySlot slot;
    private Item potion;
    private void Awake()
    {
        if (potionInventory != null)
        {
            // Debug.Log(equipment.GetSlots.Length);
            foreach (var slot in potionInventory.GetSlots)
            {
                // Debug.Log(slot.AllowedItems);
                slot.OnBeforeUpdate += OnBeforeSlotUpdate;
                slot.OnAfterUpdate += OnAfterSlotUpdate;
            }
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.Instance.Potion1Pressed)
        {
            UsePotion(1); // Use potion from slot 1
        }
        else if (InputManager.Instance.Potion2Pressed)
        {
            UsePotion(2); // Use potion from slot 2
        }
        else if (InputManager.Instance.Potion3Pressed)
        {
            UsePotion(3); // Use potion from slot 3
        }
    }

    private TextMeshProUGUI potioncount;
    private int potionCountValue = 0; // Initialize potion count value
    public void UsePotion(int slotNumber)
    {
        switch (slotNumber)
        {
            case 1:
                // Logic for using potion in slot 1
                Debug.Log("Using potion from slot 1");
                useItem(0); // Assuming slot 1 corresponds to index 0 in the potion inventory
                //updatePotionText();
                break;
            case 2:
                // Logic for using potion in slot 2
                Debug.Log("Using potion from slot 2");
                useItem(1); // Assuming slot 2 corresponds to index 1 in the potion inventory
                //updatePotionText();
                break;
            case 3:
                // Logic for using potion in slot 3
                Debug.Log("Using potion from slot 3");
                useItem(2); // Assuming slot 3 corresponds to index 2 in the potion inventory
                //updatePotionText();
                break;
            default:
                Debug.LogWarning("Invalid slot number: " + slotNumber);
                break;
        }
    }

    public void OnBeforeSlotUpdate(InventorySlot _slot)
    {
        if (_slot.ItemObject == null)
            return;

        if (_slot.parent.inventory.type == InterfaceType.Equipment)
        {
            //Debug.Log($"Removed {_slot.ItemObject.name} from {_slot.parent.inventory.type}");
        }
    }

    public void OnAfterSlotUpdate(InventorySlot _slot)
    {
        if (_slot.ItemObject == null)
            return;
        if(_slot.amount <= 0)
        {
            _slot.UpdateSlot(empty, 0);
        }
        if (_slot.parent.inventory.type == InterfaceType.Equipment)
        {
            //Debug.Log($"Equipped {_slot.ItemObject.name} to {_slot.parent.inventory.type}");
        }
    }
    private void updatePotionText(int slotnum)
    {
        GameObject usedSlot = slot1;
        switch (slotnum)
        {
            case 0:
                usedSlot = slot1;
                break;
            case 1:
                usedSlot = slot2;
                break;
            case 2:
                usedSlot = slot3;
                break;
        }
        potioncount = usedSlot.GetComponentInChildren<TextMeshProUGUI>();
        //Debug.Log("Inside update potion text function " + potioncount.text.ToString());
        if(potioncount.text.ToString() == "")
        {
            potionCountValue = 1;
        }else
        {
            //Debug.Log("Might be failing here");
            potionCountValue = int.Parse(potioncount.text.ToString().Substring(0)); // Assuming the text is a valid integer
            //Debug.Log("This is potioncountval " + potionCountValue);
        }
        potionCountValue--; // Decrease potion count
        potioncount.text = potionCountValue.ToString(); // Update the text to reflect the new count
        //Debug.Log("Exiting the function " + potionCountValue);
    }

    private void useItem(int slotNum)
    {
        slot = potionInventory.GetSlots[slotNum];
        //Debug.Log("This is the slot for deleting " +  slotNum);
        potion = slot.item;

        if (potion != null && potion.Id >= 0)
        {
            //Debug.Log("In here");
            // Logic to use the potion
            //Debug.Log(potion.consumableData.healthAffected);
            //Debug.Log(potion.consumableData.manaAffected);
            //Debug.Log(potion.consumableData.energyAffected);
            //Debug.Log("From ItemObject: " + slot.ItemObject.consumableData.manaAffected);
            playerstats.ChangeHealth(playerstats.currentHealth.value + slot.ItemObject.consumableData.healthAffected); // Example of applying potion effect
            playerstats.currentMana.value += slot.ItemObject.consumableData.manaAffected; // Example of applying potion effect
            playerstats.currentEnergy.value += slot.ItemObject.consumableData.energyAffected; // Example of applying potion effect


            // Here you can add the logic to apply the potion's effects

            // Decrease the potion count in the UI
            updatePotionText(slotNum);
            //int curr = slot.amount;
            //Debug.Log("This is slot.amount before doing gay shit " + slot.amount);
            slot.AddAmount(-1);
            //Debug.Log("This is slot.amount before doing gay shit " + slot.amount);
            slot.UpdateSlot(slot.item, slot.amount);
        }
        else
        {
            Debug.Log("No valid potion found in the selected slot.");
        }

    }
}
