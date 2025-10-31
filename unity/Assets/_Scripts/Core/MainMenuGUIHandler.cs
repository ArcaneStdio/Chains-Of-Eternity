using UnityEngine;
using UnityEngine.UI;

public class MainMenuGUIHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private Button PlayButton;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Web3AuthManager.Instance.IsConnected() && Web3AuthManager.Instance.CheckedHeroExistance())
        {
            PlayButton.interactable = true;
        }
        else
        {
            PlayButton.interactable = false;
        }
    }
}
