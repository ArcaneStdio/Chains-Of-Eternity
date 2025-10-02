using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("Scene Names")]
    public string loginSceneName = "LoginScene";
    public string characterSceneName = "checkingAuction";
    public string gameSceneName = "tutorial";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Subscribe to Web3Auth login event
        if (Web3AuthManager.Instance != null)
        {
            // We'll handle transition manually after login success
        }
    }

    public void OnLoginSuccess()
    {
        Debug.Log("Login successful, transitioning to character scene");
        LoadCharacterScene();
    }

    public void LoadCharacterScene()
    {
        Debug.Log("Loading character selection scene");
        SceneManager.LoadScene("checkingAuction");
    }

    public void OnCharacterSelected()
    {
        Debug.Log("Character selected, transitioning to game scene");
        LoadGameScene();
    }

    public void LoadGameScene()
    {
        Debug.Log("Loading game scene");
        SceneManager.LoadScene(gameSceneName);
    }

    public void ReturnToLogin()
    {
        Debug.Log("Returning to login scene");
        SceneManager.LoadScene(loginSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}