using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AuctionTesting : MonoBehaviour
{
    string walletAddress;
    [SerializeField]
    private TMP_InputField tokenId;
    [SerializeField]
    private TMP_InputField basePrice;
    [SerializeField]
    private TMP_InputField Duration;
    [SerializeField]
    private TMP_InputField bidPrice;
    [SerializeField]
    private TMP_InputField listId;
    [SerializeField]
    private TMP_InputField marketPlaceListPrice;
    

    [SerializeField]
    private TextMeshProUGUI statusText;

    private float basePriceVal;
    private UInt64 tokenIdVal;
    private float durationVal;
    private float bidPriceVal;
    private UInt64 listIdVal;
    private float marketPlaceListPriceVal;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public async void Start()
    {
               // Ensure Web3AuthManager instance exists
        if (Web3AuthManager.Instance == null)
        {
            Debug.LogError("Web3AuthManager instance not found in the scene.");
            return;
        }
        await System.Threading.Tasks.Task.Delay(5000); // Simulate wait time
        walletAddress = Web3AuthManager.Instance.GetWalletAddress();
    }

    public void OnTokenIdChanged(string value)
    {
        if (UInt64.TryParse(value, out UInt64 result))
        {
            tokenIdVal = result;
            Debug.Log("Token ID updated: " + tokenIdVal);
        }
    }
    public void OnMarketPlaceListPriceChanged(string value)
    {
        if (float.TryParse(value, out float result))
        {
            marketPlaceListPriceVal = result;
            Debug.Log("Market Place List Price updated: " + marketPlaceListPriceVal);
        }
    }
    public void OnBasePriceChanged(string value)
    {
        if (float.TryParse(value, out float result))
        {
            basePriceVal = result;
            Debug.Log("Base Price updated: " + basePriceVal);
        }
    }

    public void OnDurationChanged(string value)
    {
        if (float.TryParse(value, out float result))
        {
            durationVal = result;
            Debug.Log("Duration updated: " + durationVal);
        }
    }

    public void OnBidPriceChanged(string value)
    {
        if (float.TryParse(value, out float result))
            bidPriceVal = result;
    }

    public void OnListIdChanged(string value)
    {
        if (UInt64.TryParse(value, out UInt64 result))
            listIdVal = result;
    }


    public void CreateEmptyCollection()
    {
        Web3AuthManager.Instance.CreateNFTCollection();
    }

    public void ListOnAuction()
    {
        Debug.Log(tokenIdVal);
        Debug.Log(basePriceVal);
        Web3AuthManager.Instance.ListOnAuction(
            durationVal,
            1,
            1000,
            tokenIdVal,
            basePriceVal
        );

        statusText.text = Web3AuthManager.Instance.transactionId;
    }

    public void InitializeAuctionScheduler()
    {
        Web3AuthManager.Instance.InitializeAuctionScheduler();
    }

    public void PlaceBid()
    {
        Web3AuthManager.Instance.BidOnItem(listIdVal, bidPriceVal);
    }

    public void BuyItem()
    {
        Web3AuthManager.Instance.BuyItem(17, 10f);
    }

    public void ListItemOnMarketPlace()
    {
        Web3AuthManager.Instance.ListItemOnMarketplace(tokenIdVal, basePriceVal);
    }

}
