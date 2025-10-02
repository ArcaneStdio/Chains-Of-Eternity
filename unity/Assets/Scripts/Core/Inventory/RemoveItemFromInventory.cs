using UnityEngine;

public class RemoveItemFromInventory : MonoBehaviour
{
    public InventoryObject inventory;
    public ItemObject itemToRemove;
    public int amountToRemove = 1;

    // Call this method to remove the item
    public void RemoveItem()
    {
        //if (itemToRemove != null && inventory != null)
        //{
        //    inventory.RemoveItemByName(itemToRemove.itemName, amountToRemove);
        //    Debug.Log($"Removed {amountToRemove} of {itemToRemove.itemName} from inventory.");
        //}
        //else
        //{
        //    Debug.LogWarning("Item to remove or inventory is not assigned.");
        //}
    }
}
