//using UnityEngine;
//using System;
//using System.Numerics;
//using Nethereum.Web3;

//public class EthereumReader : MonoBehaviour
//{
//    async void Start()
//    {
//        var rpcUrl = "https://mainnet.infura.io/v3/84842078b09946638c03157f83405213";
//        var address = "0x742d35Cc6634C0532925a3b844Bc454e4438f44e";

//        var web3 = new Web3(rpcUrl);

//        try
//        {
//            BigInteger balanceWei = await web3.Eth.GetBalance.SendRequestAsync(address);
//            decimal balanceEth = Web3.Convert.FromWei(balanceWei);
//            Debug.Log($"Balance: {balanceEth} ETH");
//        }
//        catch (Exception ex)
//        {
//            Debug.LogError($"RPC Error: {ex.Message}");
//        }
//    }
//}
