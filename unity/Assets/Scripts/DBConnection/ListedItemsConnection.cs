using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// DTO to match your API response
[System.Serializable]
public class NFTResponse
{
    public ulong nft_id;
    public string name;
    public string description;
    public string kind;
    public string rarity;

    public WeaponData weapon;
    public ArmourData armour;
    public ConsumableData consumable;
    public AccessoryData accessory;
}

[System.Serializable]
public class NFTResponseList
{
    public List<NFTResponse> items;
}

public class ListedItemsConnection : MonoBehaviour
{
    [Header("Assign these in Inspector")]
    public ItemObject FirstListedObj;
    public ItemObject SecondListedObj;
    public ItemObject ThirdListedObj;
    public ItemObject FourthListedObj;

    private string url = "http://localhost:3000/latest-nfts";

    void Start()
    {
        StartCoroutine(LoadNFTs());
    }

    public IEnumerator Refresh()
    {   
        yield return new WaitForSeconds(2f);
        StartCoroutine(LoadNFTs());
    }

    IEnumerator LoadNFTs()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string rawJson = request.downloadHandler.text;

                // Wrap array for Unity's JsonUtility
                string wrapped = "{\"items\":" + rawJson + "}";
                NFTResponseList nftList = JsonUtility.FromJson<NFTResponseList>(wrapped);

                if (nftList != null && nftList.items != null)
                {
                    if (nftList.items.Count > 0) FillItemFromNFT(nftList.items[0], FirstListedObj);
                    if (nftList.items.Count > 1) FillItemFromNFT(nftList.items[1], SecondListedObj);
                    if (nftList.items.Count > 2) FillItemFromNFT(nftList.items[2], ThirdListedObj);
                    if (nftList.items.Count > 3) FillItemFromNFT(nftList.items[3], FourthListedObj);
                }
                else
                {
                    Debug.LogError("NFT parsing failed or empty list.");
                }
            }
            else
            {
                Debug.LogError("Failed to fetch NFTs: " + request.error);
            }
        }
    }

    private void FillItemFromNFT(NFTResponse nft, ItemObject item)
    {
        if (item == null) return; // nothing assigned in inspector

        // Basic Info
        item.NFTID = nft.nft_id;
        item.itemName = nft.name;
        item.description = nft.description;
        item.type = ParseItemType(nft.kind);
        item.rarity = ParseRarity(nft.rarity);
        item.stackable = false;
        item.itemCost = 10f;
        item.maxItemStock = 1;

        // Type-specific data
        switch (item.type)
        {
            case ItemType.Weapon:
                if (nft.weapon != null) item.weaponData = nft.weapon.Clone();
                break;
            case ItemType.Armour:
                if (nft.armour != null) item.armourData = nft.armour.Clone();
                break;
            case ItemType.Consumable:
                if (nft.consumable != null) item.consumableData = nft.consumable.Clone();
                break;
            case ItemType.Accessory:
                if (nft.accessory != null) item.accessoryData = nft.accessory.Clone();
                break;
            default:
                break;
        }

        Debug.Log($"Filled ItemObject: {item.itemName} ({item.type}, {item.rarity})");
    }

    public ItemType ParseItemType(string kind)
    {
        return kind switch
        {
            "Weapon" => ItemType.Weapon,
            "Armour" => ItemType.Armour,
            "Consumable" => ItemType.Consumable,
            "Accessory" => ItemType.Accessory,
            _ => ItemType.Default,
        };
    }

    private Rarity ParseRarity(string rarity)
    {
        return rarity switch
        {
            "Common" => Rarity.Common,
            "Uncommon" => Rarity.Uncommon,
            "Rare" => Rarity.Rare,
            "Epic" => Rarity.Epic,
            "Legendary" => Rarity.Legendary,
            _ => Rarity.Common
        };
    }
}
