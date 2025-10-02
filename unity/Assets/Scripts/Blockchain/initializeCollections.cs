using UnityEngine;

public class initializeCollections : MonoBehaviour
{
    private void Start()
    {
        Web3AuthManager.Instance.InitializePlayerAccount();
    }
}
