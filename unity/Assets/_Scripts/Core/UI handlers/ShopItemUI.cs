using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image itemSprite;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemCostText;
    public TextMeshProUGUI stockText;
    public Button buyButton;

    private ItemObject item;
    private int itemIndex;
    private ShopManager shopManager;

    public void SetupItem(ItemObject shopItem, int stock, int index, ShopManager manager)
    {
        item = shopItem;
        itemIndex = index;
        shopManager = manager;

        // Set UI elements
        itemSprite.sprite = shopItem.uiDisplay;
        itemNameText.text = shopItem.name;
        itemCostText.text = $"${shopItem.itemCost}";
        stockText.text = $"Stock: {stock}";

        // Setup buy button
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => shopManager.BuyItem(itemIndex));

        // Update button state
        UpdateButtonState(stock);
    }

    public void UpdateStock(int stock)
    {
        stockText.text = $"Stock: {stock}";
        UpdateButtonState(stock);
    }

    private void UpdateButtonState(int stock)
    {
        buyButton.interactable = stock > 0;

        // Optional: Change button text based on stock
        TextMeshProUGUI buttonText = buyButton.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = stock > 0 ? "Buy" : "Sold Out";
        }
    }
}