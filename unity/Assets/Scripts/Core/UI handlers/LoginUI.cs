using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;

public class LoginUI : MonoBehaviour
{
//    [Header("UI Elements")]
//    public Button metamaskButton;
//    public Button flowWalletButton;
//    public TextMeshProUGUI statusText;
//    public GameObject loadingIndicator;

//    private bool isLoggingIn = false;
    public string connectedAccount = "";
    //    private string walletType; // "metamask" or "flow"

    //#if UNITY_WEBGL && !UNITY_EDITOR
    //    [DllImport("__Internal")]
    //    private static extern void ConnectMetaMask();

    //    [DllImport("__Internal")]
    //    private static extern void ConnectFlowWallet();

    //    [DllImport("__Internal")]
    //    private static extern void createHeroCollection();
    //#endif

    //    private void Start()
    //    {
    //        InitializeUI();
    //    }

    //    private void InitializeUI()
    //    {
    //        isLoggingIn = false;
    //        metamaskButton.onClick.AddListener(OnMetaMaskLoginClicked);
    //        flowWalletButton.onClick.AddListener(OnFlowLoginClicked);
    //        statusText.text = "Welcome! Please connect a wallet to continue.";

    //        if (loadingIndicator != null)
    //            loadingIndicator.SetActive(false);   
    //    }

    //    private void OnMetaMaskLoginClicked()
    //    {
    //        if (isLoggingIn) return;

    //        walletType = "metamask";
    //        StartLogin("MetaMask");
    //#if UNITY_WEBGL && !UNITY_EDITOR
    //        ConnectMetaMask();
    //#else
    //        Debug.LogWarning("MetaMask connection only works in WebGL builds.");
    //#endif
    //    }

    //    private void OnFlowLoginClicked()
    //    {
    //        if (isLoggingIn) return;

    //        walletType = "flow";
    //        StartLogin("Flow Wallet");
    //#if UNITY_WEBGL && !UNITY_EDITOR
    //        ConnectFlowWallet();
    //#else
    //        Debug.LogWarning("Flow Wallet connection only works in WebGL builds.");
    //#endif
    //    }

    //    private void StartLogin(string providerName)
    //    {
    //        isLoggingIn = true;
    //        metamaskButton.interactable = false;
    //        flowWalletButton.interactable = false;
    //        statusText.text = $"Connecting to {providerName}...";

    //        if (loadingIndicator != null)
    //            loadingIndicator.SetActive(true);
    //    }

    //    // Called from JS when wallet connects
    //    public void OnWalletConnected(string account)
    //    {
    //        connectedAccount = account;
    //        isLoggingIn = false;
    //        statusText.text = $"Connected ({walletType}): {account}";
    //        if (loadingIndicator != null)
    //            loadingIndicator.SetActive(false);

    //        Invoke(nameof(InitializeUI), 1f);
    //        //Invoke(nameof(TransitionToCharacterScene), 1f);
    //    }
    //    public void OnFlowWalletConnected(string account)
    //    {
    //        connectedAccount = account;
    //        isLoggingIn = false;
    //        statusText.text = $"Connected ({walletType}): {account}";
    //        if (loadingIndicator != null)
    //            loadingIndicator.SetActive(false);

    //        Invoke(nameof(InitializeUI), 1f);
    //#if UNITY_WEBGL && !UNITY_EDITOR
    //        createHeroCollection();
    //#else
    //        Debug.LogWarning("Flow Wallet connection only works in WebGL builds.");
    //#endif
    //        //Invoke(nameof(TransitionToCharacterScene), 1f);
    //    }


    //    // Called from JS on error
    //    public void OnWalletError(string error)
    //    {
    //        isLoggingIn = false;
    //        metamaskButton.interactable = true;
    //        flowWalletButton.interactable = true;
    //        statusText.text = $"Error ({walletType}): " + error;
    //        if (loadingIndicator != null)
    //            loadingIndicator.SetActive(false);
    //    }

    //    private void TransitionToCharacterScene()
    //    {
    //        SceneTransitionManager.Instance.OnLoginSuccess();
    //    }
}
