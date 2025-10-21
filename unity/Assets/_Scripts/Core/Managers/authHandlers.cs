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

    private void Start()
    {
        ConnectFlowWallet();
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
        Mint_NFT.StartCoroutine(Mint_NFT.MintNFT(Web3AuthManager.Instance.GetWalletAddress()));

        sceneTransition.StartTransition();
    }

    public void ListItem()
    {
        Web3AuthManager.Instance.ListItemOnMarketplace(1, cost);
        sceneTransition.StartTransition();
    }
}
