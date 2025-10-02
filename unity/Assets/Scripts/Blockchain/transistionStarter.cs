//using Thirdweb.Unity;
//using UnityEngine;

//public class transistionStarter : MonoBehaviour
//{
//    [SerializeField] private CanvasSceneTransition sceneTransition;
//    private async void Update()
//    {
//        if (ThirdwebManager.Instance != null && ThirdwebManager.Instance.ActiveWallet != null && (await ThirdwebManager.Instance.ActiveWallet.IsConnected()) == true)
//        {
//            if(Web3AuthManager.Instance != null && Web3AuthManager.Instance.GetWalletAddress() != null && Web3AuthManager.Instance.GetWalletAddress() != "")
//            {
//                Debug.Log("Wallet is connected: " + Web3AuthManager.Instance.GetWalletAddress());
//                sceneTransition.StartTransition();
//            }
//        }
//    }
//}
