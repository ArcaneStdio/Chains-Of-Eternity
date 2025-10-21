using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [Header("Next Scene Settings")]
    public string nextSceneName;

    [Header("Optional Delay (seconds)")]
    public float transitionDelay = 0.5f;

    private bool isTransitioning = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTransitioning) return;

        if (other.CompareTag("Player"))
        {
            isTransitioning = true;
            Invoke(nameof(LoadNextScene), transitionDelay);
        }
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
