using DapperLabs.Flow.Sdk;
using DapperLabs.Flow.Sdk.Cadence;
using DapperLabs.Flow.Sdk.Crypto;
using DapperLabs.Flow.Sdk.DataObjects;
using DapperLabs.Flow.Sdk.WalletConnect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class Web3AuthManager : MonoBehaviour
{

    private UInt64 executionEffort = 1000;
    private Byte priority = 1;
    private UInt64 tokenID = 53;
    private Decimal price = 100;
    private Decimal delaySeconds = 600;
    private UInt64 ListingID = 53;
    private Decimal paymentAmount = 100;
    private bool heroExists = false;


    public TMPro.TextMeshProUGUI statusText;

    private string _currentAddress;
    private bool _isConnected;
    
    public bool HeroExists() => heroExists;
    public bool IsConnected() => _isConnected;
    public string GetWalletAddress() => _currentAddress;

    public Mint_NFT mint_NFT;

    private FlowUnityBridgeHero heronft = new FlowUnityBridgeHero();

    public FlowUnityBridgeUpdateHero updateHero;

    public FlowUnityBridgeListed listmarketplace;

    #region Transaction Strings
    private string initTransaction = " import FungibleToken from 0x9a0766d93b6608b7\r\n    import Arcane from 0x0095f13a82f1a835\r\n    import HeroNFT from 0x0095f13a82f1a835\r\n    import AuctionCallbackHandler from 0x0095f13a82f1a835\r\n    import FlowTransactionScheduler from 0x8c5303eaa26202d6\r\n    import ItemManager from 0x0095f13a82f1a835\r\n\r\n    transaction() {\r\n\r\n        prepare(\r\n            signer: auth(\r\n                BorrowValue,\r\n                IssueStorageCapabilityController,\r\n                PublishCapability,\r\n                SaveValue,\r\n                UnpublishCapability,\r\n                Storage,\r\n                Capabilities\r\n            ) &Account\r\n        ) {\r\n\r\n            //\r\n            // 1. Initialize Arcane Vault\r\n            //\r\n            if signer.storage.borrow<&Arcane.Vault>(from: Arcane.VaultStoragePath) == nil {\r\n                let vault <- Arcane.createEmptyVault(vaultType: Type<@Arcane.Vault>())\r\n                signer.storage.save(<-vault, to: Arcane.VaultStoragePath)\r\n\r\n                let vaultCap = signer.capabilities.storage.issue<&Arcane.Vault>(\r\n                    Arcane.VaultStoragePath\r\n                )\r\n                signer.capabilities.publish(vaultCap, at: Arcane.VaultPublicPath)\r\n            }\r\n\r\n            //\r\n            // 2. Initialize HeroNFT Collection\r\n            //\r\n            if signer.storage.borrow<&HeroNFT.Collection>(from: HeroNFT.CollectionStoragePath) == nil {\r\n                let collection <- HeroNFT.createEmptyCollection(nftType: Type<@HeroNFT.NFT>())\r\n                signer.storage.save(<-collection, to: HeroNFT.CollectionStoragePath)\r\n\r\n                let collectionCap = signer.capabilities.storage.issue<&HeroNFT.Collection>(\r\n                    HeroNFT.CollectionStoragePath\r\n                )\r\n                signer.capabilities.publish(collectionCap, at: HeroNFT.CollectionPublicPath)\r\n            }\r\n\r\n            //\r\n            // 3. Initialize AuctionCallbackHandler\r\n            //\r\n            if signer.storage.borrow<&AnyResource>(from: /storage/AuctionCallbackHandler) == nil {\r\n                let handler <- AuctionCallbackHandler.createHandler()\r\n                signer.storage.save(<-handler, to: /storage/AuctionCallbackHandler)\r\n            }\r\n\r\n            let _ = signer.capabilities.storage.issue<\r\n                auth(FlowTransactionScheduler.Execute) &{FlowTransactionScheduler.TransactionHandler}\r\n            >(/storage/AuctionCallbackHandler)\r\n\r\n            //\r\n            // 4. Initialize ItemManager Collection\r\n            //\r\n            if signer.storage.borrow<&ItemManager.Collection>(from: ItemManager.CollectionStoragePath) == nil {\r\n                let collection <- ItemManager.createEmptyCollection(nftType: Type<@ItemManager.NFT>())\r\n                signer.storage.save(<-collection, to: ItemManager.CollectionStoragePath)\r\n\r\n                let collectionCap = signer.capabilities.storage.issue<&ItemManager.Collection>(ItemManager.CollectionStoragePath)\r\n                signer.capabilities.publish(collectionCap, at: ItemManager.CollectionPublicPath)\r\n            }\r\n        }\r\n    }";
    private string listAuctiontx = "import FlowTransactionScheduler from 0x8c5303eaa26202d6\r\n    import FlowToken from 0x7e60df042a9c0868\r\n    import FungibleToken from 0x9a0766d93b6608b7\r\n    import AuctionHouse from 0x0095f13a82f1a835\r\n    import AuctionCallbackHandler from 0x0095f13a82f1a835\r\n    import NonFungibleToken from 0x631e88ae7f1d7c20\r\n    import ItemManager from 0x0095f13a82f1a835\r\n\r\n    /// Schedule an increment of the Counter with a relative delay in seconds\r\n    transaction(\r\n        delaySeconds: UFix64,\r\n        priority: UInt8,\r\n        executionEffort: UInt64,\r\n        tokenID: UInt64,\r\n        price: UFix64\r\n    ) {\r\n        let withdrawRef: auth(NonFungibleToken.Withdraw) &{NonFungibleToken.Collection}\r\n        prepare(signer: auth(Storage, Capabilities) &Account) {\r\n            self.withdrawRef = signer.storage.borrow<auth(NonFungibleToken.Withdraw) &{NonFungibleToken.Collection}>(from: ItemManager.CollectionStoragePath)\r\n                ?? panic(\"Missing ItemManager collection\")\r\n\r\n            // Withdraw NFT from seller's collection (this requires signer's withdraw auth)\r\n            let nft <- self.withdrawRef.withdraw(withdrawID: tokenID)\r\n\r\n            // Pass the NFT resource and signer.address into contract\r\n            let future = getCurrentBlock().timestamp + delaySeconds\r\n            let listId: UInt64 = AuctionHouse.listItem(nft: <- nft, basePrice: price, seller: signer.address, endTime: future)\r\n            let transactionData = AuctionCallbackHandler.loradata(listingId: listId)\r\n\r\n            let pr = priority == 0\r\n                ? FlowTransactionScheduler.Priority.High\r\n                : priority == 1\r\n                    ? FlowTransactionScheduler.Priority.Medium\r\n                    : FlowTransactionScheduler.Priority.Low\r\n\r\n            let est = FlowTransactionScheduler.estimate(\r\n                data: transactionData,\r\n                timestamp: future,\r\n                priority: pr,\r\n                executionEffort: executionEffort\r\n            )\r\n\r\n            assert(\r\n                est.timestamp != nil || pr == FlowTransactionScheduler.Priority.Low,\r\n                message: est.error ?? \"estimation failed\"\r\n            )\r\n\r\n            let vaultRef = signer.storage\r\n                .borrow<auth(FungibleToken.Withdraw) &FlowToken.Vault>(from: /storage/flowTokenVault)\r\n                ?? panic(\"missing FlowToken vault\")\r\n            let fees <- vaultRef.withdraw(amount: est.flowFee ?? 0.0) as! @FlowToken.Vault\r\n\r\n            let handlerCap = signer.capabilities.storage\r\n                .issue<auth(FlowTransactionScheduler.Execute) &{FlowTransactionScheduler.TransactionHandler}>(/storage/AuctionCallbackHandler)\r\n\r\n            let receipt <- FlowTransactionScheduler.schedule(\r\n                handlerCap: handlerCap,\r\n                data: transactionData,\r\n                timestamp: future,\r\n                priority: pr,\r\n                executionEffort: executionEffort,\r\n                fees: <-fees\r\n            )\r\n\r\n            log(\"Scheduled transaction id: \".concat(receipt.id.toString()).concat(\" at \").concat(receipt.timestamp.toString()))\r\n            \r\n            destroy receipt\r\n        }\r\n    }";
    private string bidOnAuction = "import FungibleToken from 0x9a0766d93b6608b7\r\n    import FlowToken from 0x7e60df042a9c0868\r\n    import NonFungibleToken from 0x631e88ae7f1d7c20\r\n    import ItemManager from 0x0095f13a82f1a835   // replace if different\r\n    import AuctionHouse from 0x0095f13a82f1a835   // replace with marketplace address\r\n    transaction(listingID: UInt64, paymentAmount: UFix64) {\r\n        let vaultRef: auth(FungibleToken.Withdraw) &{FungibleToken.Vault}\r\n        let collectionRef: &ItemManager.Collection\r\n        prepare(buyer: auth(Storage, BorrowValue) &Account) {\r\n            self.vaultRef = buyer.storage.borrow<auth(FungibleToken.Withdraw) &{FungibleToken.Vault}>(from: /storage/flowTokenVault)\r\n            ?? panic(\"Missing FlowToken vault in buyer account. Please create & link one.\")\r\n            // 3) Withdraw the paymentAmount (should be >= listing price; contract will refund any extra)\r\n            let payment <- self.vaultRef.withdraw(amount: paymentAmount)\r\n            self.collectionRef = buyer.storage.borrow<&ItemManager.Collection>(\r\n                from: ItemManager.CollectionStoragePath // Assuming this exists; if not, replace with the actual StoragePath, e.g., /storage/ItemManagerCollection\r\n            ) ?? panic(\"Missing ItemManager collection in buyer account. Please create & link one.\")\r\n            // 4) Call marketplace purchase. Buyer address passed so contract can route refunds, deposits, etc.\r\n            AuctionHouse.placeBid(\r\n                listingID: listingID,\r\n                bidder: buyer.address,\r\n                payment: <-payment\r\n            )\r\n        }\r\n        execute {\r\n        log(\"Purchase transaction executed — check marketplace events for details.\")\r\n        }\r\n    }";
    private string buyItemTx = "import FungibleToken from 0x9a0766d93b6608b7\r\n    import FlowToken from 0x7e60df042a9c0868\r\n    import NonFungibleToken from 0x631e88ae7f1d7c20 \r\n    import ItemManager from 0x0095f13a82f1a835   // replace if different\r\n    import MarketPlace2 from 0x0095f13a82f1a835   // replace with marketplace address\r\n    transaction(listingID: UInt64, paymentAmount: UFix64) {\r\n    let vaultRef: auth(FungibleToken.Withdraw) &{FungibleToken.Vault}\r\n    let collectionRef: &ItemManager.Collection\r\n    prepare(buyer: auth(Storage, BorrowValue) &Account) {\r\n        self.vaultRef = buyer.storage.borrow<auth(FungibleToken.Withdraw) &{FungibleToken.Vault}>(from: /storage/flowTokenVault)\r\n        ?? panic(\"Missing FlowToken vault in buyer account. Please create & link one.\")\r\n      \r\n        let payment <- self.vaultRef.withdraw(amount: paymentAmount)\r\n        self.collectionRef = buyer.storage.borrow<&ItemManager.Collection>(\r\n            from: ItemManager.CollectionStoragePath \r\n        ) ?? panic(\"Missing ItemManager collection in buyer account. Please create & link one.\")\r\n        \r\n        MarketPlace2.purchase(\r\n            listingID: listingID,\r\n            buyer: buyer.address,\r\n            buyerCollection: self.collectionRef,\r\n            payment: <-payment\r\n        )\r\n    }\r\n    execute {\r\n    log(\"Purchase transaction executed — check marketplace events for details.\")\r\n    }\r\n  }";
    private string listItemMarketplaceTx = " import ItemManager from 0x0095f13a82f1a835\r\n    import MarketPlace2 from 0x0095f13a82f1a835\r\n    import NonFungibleToken from 0x631e88ae7f1d7c20\r\n\r\n    transaction(tokenID: UInt64, price: UFix64) {\r\n        let withdrawRef: auth(NonFungibleToken.Withdraw) &{NonFungibleToken.Collection}\r\n        prepare(signer: auth(Storage , BorrowValue) &Account) {\r\n            self.withdrawRef = signer.storage.borrow<auth(NonFungibleToken.Withdraw) &{NonFungibleToken.Collection}>(from: ItemManager.CollectionStoragePath)\r\n                ?? panic(\"Missing ItemManager collection\")\r\n\r\n            // Withdraw NFT from seller's collection\r\n            let nft <- self.withdrawRef.withdraw(withdrawID: tokenID)\r\n\r\n            // Pass the NFT resource and seller address to the marketplace\r\n            MarketPlace2.listItem(nft: <- nft, price: price, seller: signer.address)\r\n        }\r\n    }";
    #endregion

    #region Scripts Strings
    private string checkHeroScript = "import HeroNFT from 0x0095f13a82f1a835\r\nimport NonFungibleToken from 0x631e88ae7f1d7c20\r\n\r\n/// Script to check if a Hero NFT exists for a given wallet address\r\n/// Returns true if the address has a Hero NFT, false otherwise\r\naccess(all) fun main(address: Address): Bool {\r\n    // Get the public capability for the collection\r\n    let collectionRef = getAccount(address)\r\n        .capabilities.get<&HeroNFT.Collection>(HeroNFT.CollectionPublicPath)\r\n        .borrow()\r\n    \r\n    // If the collection doesn't exist, return false\r\n    if collectionRef == nil {\r\n        return false\r\n    }\r\n    \r\n    // Check if the collection has any NFTs\r\n    let ids = collectionRef!.getIDs()\r\n    \r\n    // Return true if there's at least one NFT\r\n    return ids.length > 0\r\n}";
    #endregion
    void InitializeSDK()
    {
        FlowSDK.Init(new FlowConfig
        {
            NetworkUrl = FlowConfig.TESTNETURL, // or Mainnet / Emulator
            Protocol = FlowConfig.NetworkProtocol.HTTP
        });
        Debug.Log("Flow SDK Initialized");
    }
    void InitializeWalletProvider()
    {
        IWallet walletProvider = new WalletConnectProvider();
        walletProvider.Init(new WalletConnectConfig
        {
            ProjectId = "bbf47dd7e875d04adc5ea2ea211b2418", // the Project ID from the previous step
            ProjectDescription = "An example project to showcase Wallet Connect", // a description for your project
            ProjectIconUrl = "https://walletconnect.com/meta/favicon.ico", // URL for an icon for your project
            ProjectName = "Dapper Unity Example", // the name of your project
            ProjectUrl = "https://dapperlabs.com" // URL for your project
        });
        FlowSDK.RegisterWalletProvider(walletProvider);
        Debug.Log("Wallet Provider Initialized");
    }

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
        InitializeSDK();
        InitializeWalletProvider();
    }

    public void ConnectFlowWallet()
    {
        //#if UNITY_WEBGL && !UNITY_EDITOR
        //        FlowBridge_ConnectFlow();
        //#else
        //        Debug.LogWarning("FlowBridge not available in Editor.");
        //#endif

        // ========= After initializing the wallet provider, this function successfully connects to the wallet ========

        FlowSDK.GetWalletProvider().Authenticate("", (string flowAddress) =>
        {
            Debug.Log($"Authenticated - Flow account address is {flowAddress}");
            _currentAddress = flowAddress;
            _isConnected = true;
            SceneTransitionManager.Instance.OnLoginSuccess();
        }, () =>
        {
            Debug.Log("Authentication failed.");
        });
    }

    public void InitializePlayerAccount()
    {
        //#if UNITY_WEBGL && !UNITY_EDITOR
        //        FlowBridge_InitializePlayerAccount();
        //#else
        //        Debug.LogWarning("FlowBridge not available in Editor.");
        //#endif
        if (FlowSDK.GetWalletProvider() != null && FlowSDK.GetWalletProvider().IsAuthenticated())
        {
            StartCoroutine(InitPlayerAcc());
        }
        else
        {
            Debug.Log("Wallet not authenticated or initialized");
        }
    }

    public void DisconnectFlowWallet()
    {
        //#if UNITY_WEBGL && !UNITY_EDITOR
        //        FlowBridge_DisconnectFlow();
        //#else
        //        Debug.LogWarning("FlowBridge not available in Editor.");
        //#endif
        if (FlowSDK.GetWalletProvider() != null && FlowSDK.GetWalletProvider().IsAuthenticated())
        {
            FlowSDK.GetWalletProvider().Unauthenticate();
        }
        else
        {
            Debug.Log("Wallet not authenticated or initialized");
        }
    }

    //    public void CreateHeroCollection()
    //    {
    //#if UNITY_WEBGL && !UNITY_EDITOR
    //        FlowBridge_CreateHeroCollection();
    //#else
    //        Debug.LogWarning("FlowBridge not available in Editor.");
    //#endif
    //    }

    //    public void CreateNFTCollection()
    //    {
    //#if UNITY_WEBGL && !UNITY_EDITOR
    //        FlowBridge_CreateNFTCollection();
    //#else
    //        Debug.LogWarning("FlowBridge not available in Editor.");
    //#endif
    //    }

    public void BidOnItem(ulong listingID, ulong paymentAmount)
    {
        //#if UNITY_WEBGL && !UNITY_EDITOR
        //        FlowBridge_BidOnItem(listingID.ToString(), paymentAmount.ToString("F2"));
        //#else
        //        Debug.LogWarning("FlowBridge not available in Editor.");
        //#endif
        ListingID = listingID;
        this.paymentAmount = paymentAmount;
        if (FlowSDK.GetWalletProvider() != null && FlowSDK.GetWalletProvider().IsAuthenticated())
        {
            StartCoroutine(BidOnItemEnum());
        }
        else
        {
            Debug.Log("Wallet not authenticated or initialized");
        }
    }

    public void BuyItem(ulong listingID, ulong paymentAmount)
    {
//#if UNITY_WEBGL && !UNITY_EDITOR
//        FlowBridge_BuyItem(listingID.ToString(), paymentAmount.ToString("F2"));
//#else
//        Debug.LogWarning("FlowBridge not available in Editor.");
//#endif

        ListingID = listingID;
        this.paymentAmount = paymentAmount;
        if (FlowSDK.GetWalletProvider() != null && FlowSDK.GetWalletProvider().IsAuthenticated())
        {
            StartCoroutine(BidOnItemEnum());
        }
        else
        {
            Debug.Log("Wallet not authenticated or initialized");
        }
    }

    //    public void SetupArcaneTokenAccount()
    //    {
    //#if UNITY_WEBGL && !UNITY_EDITOR
    //        FlowBridge_SetupArcaneTokenAccount();
    //#else
    //        Debug.LogWarning("FlowBridge not available in Editor.");
    //#endif
    //    }

    //    public void InitializeAuctionScheduler()
    //    {
    //#if UNITY_WEBGL && !UNITY_EDITOR
    //        FlowBridge_InitializeAuctionScheduler();
    //#else
    //        Debug.LogWarning("FlowBridge not available in Editor.");
    //#endif
    //    }

    //    public void GetFlowUser()
    //    {
    //#if UNITY_WEBGL && !UNITY_EDITOR
    //        FlowBridge_GetFlowUser();
    //#else
    //        Debug.LogWarning("FlowBridge not available in Editor.");
    //#endif
    //    }

    public void ListOnAuction(Decimal delaySeconds, Byte priority, ulong executionEffort, ulong tokenID, ulong price)
    {
        //#if UNITY_WEBGL && !UNITY_EDITOR
        //        FlowBridge_ListOnAuction(
        //            delaySeconds.ToString("F2"),
        //            priority.ToString(),
        //            executionEffort.ToString(),
        //            tokenID.ToString(),
        //            price.ToString("F2")
        //        );
        //#else
        //  Debug.LogWarning("FlowBridge not available in Editor.");
        //#endif
        this.delaySeconds = delaySeconds;
        this.priority = priority;
        this.executionEffort = executionEffort;
        this.tokenID = tokenID;
        this.price = price;
        if (FlowSDK.GetWalletProvider() != null && FlowSDK.GetWalletProvider().IsAuthenticated())
        {
            StartCoroutine(ListAuctionEnum());
        }
        else
        {
            Debug.Log("Wallet not authenticated or initialized");
        }
    }



    public void ListItemOnMarketplace(ulong tokenID, ulong price)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        FlowBridge_ListItemOnMarketplace(tokenID.ToString(), price.ToString("F2"));
#else
        Debug.LogWarning("FlowBridge not available in Editor.");
#endif
        this.tokenID = tokenID;
        this.price = price;
        if (FlowSDK.GetWalletProvider() != null && FlowSDK.GetWalletProvider().IsAuthenticated())
        {
            StartCoroutine(ListMarketplaceEnum());
        }
        else
        {
            Debug.Log("Wallet not authenticated or initialized");
        }
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
        StartCoroutine(MintHero(_currentAddress));
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


    #region Transactions Enumerators

    public IEnumerator InitPlayerAcc()
    {

        Task<FlowTransactionResponse> txResponse = Transactions.Submit(initTransaction);

        while (!txResponse.IsCompleted)
        {
            yield return null;
        }

        if (txResponse.Result.Error != null)
        {
            statusText.text = "Error, see log";
            Debug.LogError(txResponse.Result.Error.Message);
            yield break;
        }

        FlowTransactionResult txResult = null;
        while (true)
        {
            var task = Transactions.GetResult(txResponse.Result.Id);
            yield return new WaitUntil(() => task.IsCompleted);

            txResult = task.Result;

            if (txResult.Error != null)
            {
                statusText.text = "Error, see log";
                Debug.LogError(txResult.Error.Message);
                yield break;
            }

            statusText.text = "Transaction Status: " + txResult.Status.ToString();
            Debug.Log("Transaction Status: " + txResult.Status);

            if (txResult.Status == FlowTransactionStatus.SEALED)
            {
                statusText.text = " Transaction Sealed! ID: " + txResponse.Result.Id;
                Debug.Log("Transaction sealed with ID: " + txResponse.Result.Id);
                yield break;
            }

            // Wait a couple seconds before polling again
            yield return new WaitForSeconds(1f);
        }
    }

    public IEnumerator BidOnItemEnum()
    {
        List<CadenceBase> args = new List<CadenceBase>
        {
            DapperLabs.Flow.Sdk.Cadence.Convert.ToCadence(ListingID, "UInt64"),
            DapperLabs.Flow.Sdk.Cadence.Convert.ToCadence(paymentAmount, "UFix64")
        };
        Task<FlowTransactionResponse> txResponse = Transactions.Submit(bidOnAuction, args);

        while (!txResponse.IsCompleted)
        {
            yield return null;
        }

        if (txResponse.Result.Error != null)
        {
            statusText.text = "Error, see log";
            Debug.LogError(txResponse.Result.Error.Message);
            yield break;
        }

        FlowTransactionResult txResult = null;
        while (true)
        {
            var task = Transactions.GetResult(txResponse.Result.Id);
            yield return new WaitUntil(() => task.IsCompleted);

            txResult = task.Result;

            if (txResult.Error != null)
            {
                statusText.text = "Error, see log";
                Debug.LogError(txResult.Error.Message);
                yield break;
            }

            statusText.text = "Transaction Status: " + txResult.Status.ToString();
            Debug.Log("Transaction Status: " + txResult.Status);

            if (txResult.Status == FlowTransactionStatus.SEALED)
            {
                statusText.text = " Transaction Sealed! ID: " + txResponse.Result.Id;
                Debug.Log("Transaction sealed with ID: " + txResponse.Result.Id);
                yield break;
            }

            // Wait a couple seconds before polling again
            yield return new WaitForSeconds(1f);
        }
    }

    public IEnumerator BuyItemEnum()
    {
        List<CadenceBase> args = new List<CadenceBase>
        {
            DapperLabs.Flow.Sdk.Cadence.Convert.ToCadence(ListingID, "UInt64"),
            DapperLabs.Flow.Sdk.Cadence.Convert.ToCadence(paymentAmount, "UFix64")
        };
        Task<FlowTransactionResponse> txResponse = Transactions.Submit(buyItemTx, args);

        while (!txResponse.IsCompleted)
        {
            yield return null;
        }

        if (txResponse.Result.Error != null)
        {
            statusText.text = "Error, see log";
            Debug.LogError(txResponse.Result.Error.Message);
            yield break;
        }

        FlowTransactionResult txResult = null;
        while (true)
        {
            var task = Transactions.GetResult(txResponse.Result.Id);
            yield return new WaitUntil(() => task.IsCompleted);

            txResult = task.Result;

            if (txResult.Error != null)
            {
                statusText.text = "Error, see log";
                Debug.LogError(txResult.Error.Message);
                yield break;
            }

            statusText.text = "Transaction Status: " + txResult.Status.ToString();
            Debug.Log("Transaction Status: " + txResult.Status);

            if (txResult.Status == FlowTransactionStatus.SEALED)
            {
                statusText.text = " Transaction Sealed! ID: " + txResponse.Result.Id;
                Debug.Log("Transaction sealed with ID: " + txResponse.Result.Id);
                yield break;
            }

            // Wait a couple seconds before polling again
            yield return new WaitForSeconds(1f);
        }
    }



    public IEnumerator ListAuctionEnum()
    {
        List<CadenceBase> args = new List<CadenceBase>
        {
            DapperLabs.Flow.Sdk.Cadence.Convert.ToCadence(delaySeconds, "UFix64"),
            DapperLabs.Flow.Sdk.Cadence.Convert.ToCadence(priority, "UInt8"),
            DapperLabs.Flow.Sdk.Cadence.Convert.ToCadence(executionEffort, "UInt64"),
            DapperLabs.Flow.Sdk.Cadence.Convert.ToCadence(tokenID, "UInt64"),
            DapperLabs.Flow.Sdk.Cadence.Convert.ToCadence(price, "UFix64")
        };
        Task<FlowTransactionResponse> txResponse = Transactions.Submit(listAuctiontx, args);

        while (!txResponse.IsCompleted)
        {
            yield return null;
        }

        if (txResponse.Result.Error != null)
        {
            statusText.text = "Error, see log";
            Debug.LogError(txResponse.Result.Error.Message);
            yield break;
        }

        FlowTransactionResult txResult = null;
        while (true)
        {
            var task = Transactions.GetResult(txResponse.Result.Id);
            yield return new WaitUntil(() => task.IsCompleted);

            txResult = task.Result;

            if (txResult.Error != null)
            {
                statusText.text = "Error, see log";
                Debug.LogError(txResult.Error.Message);
                yield break;
            }

            statusText.text = "Transaction Status: " + txResult.Status.ToString();
            Debug.Log("Transaction Status: " + txResult.Status);

            if (txResult.Status == FlowTransactionStatus.SEALED)
            {
                statusText.text = " Transaction Sealed! ID: " + txResponse.Result.Id;
                Debug.Log("Transaction sealed with ID: " + txResponse.Result.Id);
                yield break;
            }

            // Wait a couple seconds before polling again
            yield return new WaitForSeconds(1f);
        }
    }

    public IEnumerator ListMarketplaceEnum()
    {
        List<CadenceBase> args = new List<CadenceBase>
        {
            DapperLabs.Flow.Sdk.Cadence.Convert.ToCadence(tokenID, "UInt64"),
            DapperLabs.Flow.Sdk.Cadence.Convert.ToCadence(price, "UFix64")
        };
        Task<FlowTransactionResponse> txResponse = Transactions.Submit(listItemMarketplaceTx, args);

        while (!txResponse.IsCompleted)
        {
            yield return null;
        }

        if (txResponse.Result.Error != null)
        {
            statusText.text = "Error, see log";
            Debug.LogError(txResponse.Result.Error.Message);
            yield break;
        }

        FlowTransactionResult txResult = null;
        while (true)
        {
            var task = Transactions.GetResult(txResponse.Result.Id);
            yield return new WaitUntil(() => task.IsCompleted);

            txResult = task.Result;

            if (txResult.Error != null)
            {
                statusText.text = "Error, see log";
                Debug.LogError(txResult.Error.Message);
                yield break;
            }

            statusText.text = "Transaction Status: " + txResult.Status.ToString();
            Debug.Log("Transaction Status: " + txResult.Status);

            if (txResult.Status == FlowTransactionStatus.SEALED)
            {
                statusText.text = " Transaction Sealed! ID: " + txResponse.Result.Id;
                Debug.Log("Transaction sealed with ID: " + txResponse.Result.Id);
                yield break;
            }

            // Wait a couple seconds before polling again
            yield return new WaitForSeconds(1f);
        }
    }

    public IEnumerator CheckHeroExistence()
    {
        //Create the script request.  We use the text in the GetNFTsOnAccount.cdc file and pass the address of the
        //authenticated account as the address of the account we want to query.
        Debug.Log("Checking Hero NFT existence for address: " + FlowSDK.GetWalletProvider().GetAuthenticatedAccount().Address);
        FlowScriptRequest scriptRequest = new FlowScriptRequest
        {
            Script = checkHeroScript,
            Arguments = new List<CadenceBase>
            {
                new CadenceAddress(FlowSDK.GetWalletProvider().GetAuthenticatedAccount().Address)
            }
        };

        //Execute the script and wait until it is completed.
        Task<FlowScriptResponse> scriptResponse = Scripts.ExecuteAtLatestBlock(scriptRequest);
        yield return new WaitForSeconds(3);

        

        //Iterate over the returned dictionary
        bool result = DapperLabs.Flow.Sdk.Cadence.Convert.FromCadence<bool>(scriptResponse.Result.Value);
        Debug.Log("Hero NFT existence: " + result);
        this.heroExists = result;
        if (result)
        {
            SceneTransitionManager.Instance.LoadGameScene();
        }
        else
        {
            SceneTransitionManager.Instance.LoadCharacterScene();
        }
    }

    public IEnumerator MintHero(string recipientAddress)
    {
        Debug.Log("Reached here -------- 0");
        string apiBase = "http://localhost:3000";
        MintHeroRequest reqData = new MintHeroRequest { recipientAddr = recipientAddress };
        string json = JsonUtility.ToJson(reqData);
        Debug.Log("Reached here -------- 1");
        UnityWebRequest request = new UnityWebRequest($"{apiBase}/mint-hero", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        Debug.Log("Reached here -------- 2");

        yield return request.SendWebRequest();

        Debug.Log("Reached here -------- 3");
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("MintHero Success: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("MintHero Failed: " + request.error);
        }
    }

    #endregion

}
