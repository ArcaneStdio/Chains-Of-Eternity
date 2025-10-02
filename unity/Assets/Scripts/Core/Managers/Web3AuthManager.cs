using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class Web3AuthManager : MonoBehaviour
{
    private string _currentAddress;
    public string GetWalletAddress() => _currentAddress;

    public Mint_NFT mint_NFT;

    public FlowUnityBridgeHero heronft;

    public FlowUnityBridgeUpdateHero updateHero;

    public FlowUnityBridgeListed listmarketplace;


    //public class UpdateHeroData
    //{
    //    public ulong nftID;
    //
    //    // Offensive
    //    public uint damage;
    //    public uint attackSpeed;
    //    public uint criticalRate;
    //    public uint criticalDamage;
    //
    //    // Defensive
    //    public uint maxHealth;
    //    public uint defense;
    //    public uint healthRegeneration;
    //    public uint[] resistances;
    //
    //    // Special
    //    public uint maxEnergy;
    //    public uint energyRegeneration;
    //    public uint maxMana;
    //    public uint manaRegeneration;
    //
    //    // Stat Points
    //    public uint constitution;
    //    public uint strength;
    //    public uint dexterity;
    //    public uint intelligence;
    //    public uint stamina;
    //    public uint agility;
    //    public uint remainingPoints;
    //}

    //public UpdateHeroData data;
    //where do we get input from
    public string transactionId { get; private set; }
    public static Web3AuthManager Instance { get; private set; }

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void FlowBridge_SetUnityInstance(IntPtr instance);

    [DllImport("__Internal")]
    private static extern void FlowBridge_ConnectFlow();

    [DllImport("__Internal")]
    private static extern void FlowBridge_DisconnectFlow();

    [DllImport("__Internal")]
    private static extern void FlowBridge_CreateHeroCollection();

    [DllImport("__Internal")]
    private static extern void FlowBridge_CreateNFTCollection();

    [DllImport("__Internal")]
    private static extern void FlowBridge_BidOnItem(string listingID, string paymentAmount);

    [DllImport("__Internal")]
    private static extern void FlowBridge_BuyItem(string listingID, string paymentAmount);

    [DllImport("__Internal")]
    private static extern void FlowBridge_SetupArcaneTokenAccount();

    [DllImport("__Internal")]
    private static extern void FlowBridge_GetFlowUser();

    [DllImport("__Internal")]
    private static extern void FlowBridge_ListOnAuction(
        string delaySeconds,
        string priority,
        string executionEffort,
        string tokenID,
        string price
    );

    [DllImport("__Internal")]
    private static extern void FlowBridge_InitializeAuctionScheduler();

    [DllImport("__Internal")]
    private static extern void FlowBridge_ListItemOnMarketplace(string tokenID, string price);

    [DllImport("__Internal")]
    private static extern void FlowBridge_InitializePlayerAccount();

#endif

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
}

    public void ConnectFlowWallet()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        FlowBridge_ConnectFlow();
#else
        Debug.LogWarning("FlowBridge not available in Editor.");
#endif
    }

    public void InitializePlayerAccount()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        FlowBridge_InitializePlayerAccount();
#else
        Debug.LogWarning("FlowBridge not available in Editor.");
#endif
    }    

    public void DisconnectFlowWallet()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        FlowBridge_DisconnectFlow();
#else
        Debug.LogWarning("FlowBridge not available in Editor.");
#endif
    }

    public void CreateHeroCollection()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        FlowBridge_CreateHeroCollection();
#else
        Debug.LogWarning("FlowBridge not available in Editor.");
#endif
    }

    public void CreateNFTCollection()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        FlowBridge_CreateNFTCollection();
#else
        Debug.LogWarning("FlowBridge not available in Editor.");
#endif
    }

    public void BidOnItem(ulong listingID, float paymentAmount)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        FlowBridge_BidOnItem(listingID.ToString(), paymentAmount.ToString("F2"));
#else
        Debug.LogWarning("FlowBridge not available in Editor.");
#endif
    }

    public void BuyItem(ulong listingID, float paymentAmount)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        FlowBridge_BuyItem(listingID.ToString(), paymentAmount.ToString("F2"));
#else
        Debug.LogWarning("FlowBridge not available in Editor.");
#endif
    }

    public void SetupArcaneTokenAccount()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        FlowBridge_SetupArcaneTokenAccount();
#else
        Debug.LogWarning("FlowBridge not available in Editor.");
#endif
    }

    public void InitializeAuctionScheduler()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        FlowBridge_InitializeAuctionScheduler();
#else
        Debug.LogWarning("FlowBridge not available in Editor.");
#endif
    }

    public void GetFlowUser()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        FlowBridge_GetFlowUser();
#else
        Debug.LogWarning("FlowBridge not available in Editor.");
#endif
    }

    public void ListOnAuction(float delaySeconds, int priority, ulong executionEffort, ulong tokenID, float price)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        FlowBridge_ListOnAuction(
            delaySeconds.ToString("F2"),
            priority.ToString(),
            executionEffort.ToString(),
            tokenID.ToString(),
            price.ToString("F2")
        );
#else
        Debug.LogWarning("FlowBridge not available in Editor.");
#endif
    }

    public void ListItemOnMarketplace(ulong tokenID, float price)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        FlowBridge_ListItemOnMarketplace(tokenID.ToString(), price.ToString("F2"));
#else
        Debug.LogWarning("FlowBridge not available in Editor.");
#endif

    }

    public void MintNFT_Request()
    {
        if (mint_NFT != null)
        {
            Debug.Log("Minting NFT for address: " + _currentAddress);
            mint_NFT.MintNFT(_currentAddress);
        }
    }

    public void HeroNFT_Request()
    {
        if (heronft != null)
        {
            heronft.MintHero(_currentAddress);
        }
    }

    //public void UpdateHero_Request()
    //{
    //    
    //    if (updateHero != null)
    //    {
    //        updateHero.UpdateHero(data);
    //    }
    //}





    // ---- Callbacks from FlowBridge ----

    public void OnFlowWalletConnected(string address)
    {
        Debug.Log("Flow wallet connected: " + address);
        _currentAddress = address;
    }

    public void OnFlowTxSubmitted(string txId)
    {
        Debug.Log("Transaction submitted: " + txId);

    }
    public void OnFlowTxSealed(string txId)
    {
        Debug.Log("Transaction sealed: " + txId);
    }

    public void OnFlowError(string error)
    {
        Debug.LogError("Flow error: " + error);
    }

    
}
