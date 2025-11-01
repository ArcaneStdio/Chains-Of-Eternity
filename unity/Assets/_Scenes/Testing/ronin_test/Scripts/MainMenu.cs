using UnityEngine;
using UnityEngine.SceneManagement; 

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        if (Web3AuthManager.Instance.HeroExists())
        {
            SceneTransitionManager.Instance.LoadGameScene();
        }
        else
        {
            SceneTransitionManager.Instance.LoadCharacterScene();
        }
    }

    public void OpenSettings()
    {
        SceneManager.LoadScene("SettingsScreen");
    }

    public void QuitGame()
    {
        Debug.Log("QUIT GAME!");

        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
