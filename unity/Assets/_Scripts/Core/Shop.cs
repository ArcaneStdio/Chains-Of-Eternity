using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] Canvas dialogueCanvas;
    [SerializeField] Transform spawnPoint;

    public Mint_NFT mint_NFT;
    private string _currentAddress => Web3AuthManager.Instance.GetWalletAddress();

    [Header("Shop Items")]
    public GameObject shopItems;

    [Header("Stock Settings")]
    public int maxStock = 10;
    private int currentStock;

    [Header("UI References")]
    public Transform itemContainer; // Parent object for shop item UI elements
    public GameObject shopItemPrefab; // Prefab for individual shop items

    private void Start()
    {
        InitializeShop();
    }

    private void InitializeShop()
    {
        // Initialize stock array
        currentStock = 5;
        // Create UI elements for each shop item
        CreateShopUI();
    }

    private void CreateShopUI()
    {
        for (int i = 0; i < 1; i++)
        {
            GameObject itemUI = Instantiate(shopItemPrefab, itemContainer);
            ShopItemUI shopItemUI = itemUI.GetComponent<ShopItemUI>();

            // Setup the item UI with data
            shopItemUI.SetupItem(shopItems.GetComponent<GroundItem>().item, currentStock, i, this);
        }
    }
    public void MintNFT_Request()
    {
        if (mint_NFT != null)
        {
            mint_NFT.MintNFT(_currentAddress);
        }
    }
    public void BuyItem(int itemIndex)
    {
        // Check if item is available
        // if (itemIndex < 0 || itemIndex >= shopItems)
        // {
        //     Debug.LogError("Invalid item index!");
        //     return;
        // }

        if (currentStock <= 0)
        {
            Debug.Log("Item out of stock!");
            return;
        }


        // TODO: Check if player has enough currency
        // if (PlayerCurrency.Instance.GetCurrency() < item.cost)
        // {
        //     Debug.Log("Not enough currency!");
        //     return;
        // }

        // Deduct currency and add to inventory
        // deduct hero currency here
        // PlayerCurrency.Instance.SpendCurrency(item.cost);

        // add to player inventory here
        // PlayerInventory.Instance.AddItem(item);
        Vector3 spawnPosition = spawnPoint.position + new Vector3(2, 2, 0);
        Debug.Log($"Spawning item at position: {spawnPosition}");
        Instantiate(shopItems,spawnPosition, Quaternion.identity);
        // Update stock
        currentStock--;

        // Update UI
        UpdateShopUI();

        //Debug.Log($"Bought {item.name} for currency!");
    }

    private void UpdateShopUI()
    {
        ShopItemUI[] itemUIs = itemContainer.GetComponentsInChildren<ShopItemUI>();

        for (int i = 0; i < itemUIs.Length; i++)
        {
            if (i < currentStock)
            {
                itemUIs[i].UpdateStock(currentStock);
            }
        }
    }

    public int GetCurrentStock(int itemIndex)
    {
        if (itemIndex >= 0)
            return currentStock;
        return 0;
    }

    public void CloseShop()
    {
        canvas.enabled = false;
        dialogueCanvas.enabled = true;
    }

    public void OpenShop()
    {
        canvas.enabled = true;
        dialogueCanvas.enabled = false;
    }

    public void refreshShop()
    {

        // Reset each item's current stock to its max stock
        // for (int i = 0; i < shopItems.Length && i < currentStock.Length; i++)
        // {
        //     // Get the max stock from the individual ShopItemUI component
        //     int maxStockForItem = shopItems[i].maxItemStock; // You'll need this method in ShopItemUI

        //     // Set current stock to max stock
        //     currentStock[i] = maxStockForItem;
        // }
        int maxStockForItem = 5; // You'll need this method in ShopItemUI

        // Set current stock to max stock
        currentStock = maxStockForItem;
        // Update the UI to reflect the new stock values
        UpdateShopUI();

        Debug.Log("Shop refreshed! All items restocked to maximum.");
    }
}
