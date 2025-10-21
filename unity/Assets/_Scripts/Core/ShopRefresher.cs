//using Nethereum.ABI.Model;
//using Nethereum.Contracts;
//using Nethereum.Web3;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Numerics;
//using System.Threading.Tasks;
//using UnityEngine;

//public class ShopRefreshReader : MonoBehaviour
//{
//    [Header("Contract Settings")]
//    private string contractAddress = "0x7d1FA1DE536130778eC59D232041cF863BdDb715";
//    private string rpcUrl = "https://api.avax-test.network/ext/bc/C/rpc"; // Fuji testnet

//    private Web3 web3;
//    private Contract contract;
//    private Function refreshCounterFunction;
//    private BigInteger prevValue = 0;

//    [SerializeField] ShopManager shopManager;

//    private string abi = @"[
//        {
//            ""inputs"": [],
//            ""name"": ""shopRefreshCounter"",
//            ""outputs"": [
//                {
//                    ""internalType"": ""uint256"",
//                    ""name"": """",
//                    ""type"": ""uint256""
//                }
//            ],
//            ""stateMutability"": ""view"",
//            ""type"": ""function""
//        }
//    ]";

//    private string abiJSON;

//    private async void Start()
//    {
//        TextAsset abiAsset = Resources.Load<TextAsset>("ABI/refreshABI");
//        if (abiAsset != null)
//        {
//            abiJSON = abiAsset.text;
//            ExtractABIFromContractJson(abiJSON);
//        }
//        else
//        {
//            Debug.LogError("ABI file not found in Resources/ABI/refreshABI");
//        }

//        ExtractABIFromContractJson(abiJSON);
//        web3 = new Web3(rpcUrl);
//        contract = web3.Eth.GetContract(abi, contractAddress);
//        refreshCounterFunction = contract.GetFunction("shopRefreshCounter");
//        if(refreshCounterFunction == null)
//        {
//            Debug.Log("Function not found in abi");
//        }
//        // Start checking every 10 minutes
//        await CheckCounterPeriodically();
//    }

//    private async Task CheckCounterPeriodically()
//    {
//        while (true)
//        {
//            await GetAndLogShopRefreshCounter();
//            await Task.Delay(TimeSpan.FromMinutes(1));
//        }
//    }

//    private async Task GetAndLogShopRefreshCounter()
//    {
//        try
//        {
//            Debug.Log("Fetching shop counter value");
//            BigInteger counter = await refreshCounterFunction.CallAsync<BigInteger>();
//            Debug.Log($"[ShopRefresh] Counter: {counter}");
//            // Optionally: use this value in-game
//            if(counter != prevValue)
//            {
//                prevValue = counter;
//                shopManager.refreshShop();
//            }
//        }
//        catch (Exception e)
//        {
//            Debug.LogError($"[ShopRefresh] Failed to fetch counter: {e.Message}");
//        }
//    }

//    private void ExtractABIFromContractJson(string contractJson)
//    {
//        try
//        {
//            JObject contractData = JObject.Parse(contractJson);

//            // Check if it's a full Forge contract JSON (has both abi and bytecode)
//            if (contractData["abi"] != null)
//            {
//                // Extract just the ABI part and convert back to JSON string
//                abi = contractData["abi"].ToString();
//                Debug.Log("ABI extracted from full Forge contract JSON");
//            }
//            else if (contractData["abi"] == null && contractData.First != null)
//            {
//                // It's likely a direct ABI array
//                abi = contractJson;
//                Debug.Log("Using JSON as direct ABI");
//            }
//            else
//            {
//                Debug.LogError("Could not find ABI in the provided JSON");
//            }
//        }
//        catch (Exception e)
//        {
//            Debug.LogError($"Failed to parse contract JSON: {e.Message}");
//        }
//    }
//}
