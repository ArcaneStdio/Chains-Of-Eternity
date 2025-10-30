using DapperLabs.Flow.Sdk;
using DapperLabs.Flow.Sdk.Cadence;
using DapperLabs.Flow.Sdk.Crypto;
using DapperLabs.Flow.Sdk.DataObjects;
using DapperLabs.Flow.Sdk.WalletConnect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class FlowConnectSamples : MonoBehaviour
{
    string mintTransaction = "import FungibleToken from 0x9a0766d93b6608b7\r\n    import FlowToken from 0x7e60df042a9c0868\r\n    import NonFungibleToken from 0x631e88ae7f1d7c20 \r\n    import ItemManager from 0x0095f13a82f1a835   // replace if different\r\n    import MarketPlace2 from 0x0095f13a82f1a835   // replace with marketplace address\r\n    transaction(listingID: UInt64, paymentAmount: UFix64) {\r\n    let vaultRef: auth(FungibleToken.Withdraw) &{FungibleToken.Vault}\r\n    let collectionRef: &ItemManager.Collection\r\n    prepare(buyer: auth(Storage, BorrowValue) &Account) {\r\n        self.vaultRef = buyer.storage.borrow<auth(FungibleToken.Withdraw) &{FungibleToken.Vault}>(from: /storage/flowTokenVault)\r\n        ?? panic(\"Missing FlowToken vault in buyer account. Please create & link one.\")\r\n      \r\n        let payment <- self.vaultRef.withdraw(amount: paymentAmount)\r\n        self.collectionRef = buyer.storage.borrow<&ItemManager.Collection>(\r\n            from: ItemManager.CollectionStoragePath \r\n        ) ?? panic(\"Missing ItemManager collection in buyer account. Please create & link one.\")\r\n        \r\n        MarketPlace2.purchase(\r\n            listingID: listingID,\r\n            buyer: buyer.address,\r\n            buyerCollection: self.collectionRef,\r\n            payment: <-payment\r\n        )\r\n    }\r\n    execute {\r\n    log(\"Purchase transaction executed — check marketplace events for details.\")\r\n    }\r\n  }";
    [SerializeField]
    private TextMeshProUGUI statusText;

    private UInt64 executionEffort = 1000;
    private Byte priority = 1;
    public UInt64 ListingID = 53;
    public Decimal paymentAmount = 100;
    public Decimal delaySeconds = 600;

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

    public void initAccount()
    {
        Web3AuthManager.Instance.InitializePlayerAccount();
    }

    public void AuthenticateWallet()
    {

        // ========= After initializing the wallet provider, this function successfully connects to the wallet ========

        FlowSDK.GetWalletProvider().Authenticate("", (string flowAddress) =>
        {
            Debug.Log($"Authenticated - Flow account address is {flowAddress}");
        }, () =>
        {
            Debug.Log("Authentication failed.");
        });
    }

    public void TestTransaction()
    {
        if (FlowSDK.GetWalletProvider() != null && FlowSDK.GetWalletProvider().IsAuthenticated())
        {
            StartCoroutine(SampleTransaction());
        }
        else
        {
            Debug.Log("Wallet not authenticated or initialized");
        }
    }
    public IEnumerator SampleTransaction()
    {
        List<CadenceBase> args = new List<CadenceBase>
        {
            DapperLabs.Flow.Sdk.Cadence.Convert.ToCadence(ListingID, "UInt64"),
            DapperLabs.Flow.Sdk.Cadence.Convert.ToCadence(paymentAmount, "UFix64")
        };
        Task<FlowTransactionResponse> txResponse = Transactions.Submit(mintTransaction, args);

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

    private void Awake()
    {
        InitializeSDK();
        InitializeWalletProvider();
    }
}