using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class authHandlers : MonoBehaviour
{
    [SerializeField] private CanvasSceneTransition sceneTransition;
    [SerializeField] private Mint_NFT Mint_NFT;
    private float cost;
    public void onValueChangeFunc(float s)
    {
        cost = s;
    }

   
    public void ConnectFlowWallet()
    {
        Web3AuthManager.Instance.ConnectFlowWallet();
    }
    private IEnumerator delay() { yield return new WaitForSeconds(3); }

    public void MintHero()
    {
        if (Web3AuthManager.Instance == null)
        {
            Debug.Log("Web3AuthManager instance is null!==================");
        }
        else
        {
            Debug.Log("Minting Hero for address: " + Web3AuthManager.Instance.GetWalletAddress());
        }
        Web3AuthManager.Instance.HeroNFT_Request();

        //sceneTransition.StartTransition();
    }

    public void ListItem()
    {
        //Web3AuthManager.Instance.ListItemOnMarketplace(1, cost);
        sceneTransition.StartTransition();
    }
}
