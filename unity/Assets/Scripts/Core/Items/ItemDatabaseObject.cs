using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory System/Items/Database")]
public class ItemDatabaseObject : ScriptableObject, ISerializationCallbackReceiver
{
    public ItemObject[] ItemObjects;

    [ContextMenu("Update ID's")]
    public void UpdateID()
    {
        for (int i = 0; i < ItemObjects.Length; i++)
        {
            if (ItemObjects[i] != null)
            {
                ItemObjects[i].data.Id = i;
            }
        }
    }

    public void OnAfterDeserialize()
    {
        UpdateID(); // ensures IDs are correct after loading
    }

    public void OnBeforeSerialize()
    {
        UpdateID(); // ensures IDs are fresh before saving
    }

    public ItemObject GetItemObjectById(int id)
    {
        if (id < 0 || id >= ItemObjects.Length)
            return null;
        return ItemObjects[id];
    }
}
