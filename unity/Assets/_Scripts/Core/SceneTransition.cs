using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using System.Threading;

public class CanvasSceneTransition : MonoBehaviour
{
    [Header("Transition Settings")]
    [SerializeField] private string sceneToLoad;          // Scene name (or leave empty to load next in build order)
    [SerializeField] private float vidDuration = 1f;     // Duration of fade
    [SerializeField] private GameObject transitionCanvas; // CanvasGroup for fading
    private float Timer = 0f;

    private bool isTransitioning = false;

    private void Start()
    {
        // // Ensure canvas starts hidden
        // if (transitionCanvas != null)
        // {
        //     transitionCanvas.alpha = 0f;
        //     transitionCanvas.blocksRaycasts = false; // Don't block gameplay when transparent
        // }
    }
    // private void Update()
    // {
    //     Timer += Time.deltaTime;
    //     if (Timer > 5)
    //     {   
    //         if (player.currentHealth <= 5 && !isTransitioning)
    //         {
    //             StartCoroutine(DoSceneTransition());
    //         }
    //     }
    // }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger entered");
        if (other.CompareTag("Player") && !isTransitioning)
        {
            StartCoroutine(DoSceneTransition());
        }
    }

    public void StartTransition()
    {
        if (!isTransitioning)
        {
            Debug.Log("Starting Transition");
            StartCoroutine(DoSceneTransition());
        }
    }


    private IEnumerator DoSceneTransition()
    {
        isTransitioning = true;
        //player.Enable_DisableInput(false); 
        // Fade in (to black)
        Debug.Log("Transition started");
        yield return StartCoroutine(Activatee());
        Debug.Log("Transition 1");

        // Load next scene
        string nextScene = string.IsNullOrEmpty(sceneToLoad)
            ? GetNextSceneName()
            : sceneToLoad;

        Debug.Log("Transition 2");
        yield return SceneManager.LoadSceneAsync(nextScene);

        Debug.Log("Transition 3");
        isTransitioning = false;
    }

    private IEnumerator Activatee()
    {
        Debug.Log("Transition 4");
        if (transitionCanvas == null)
        {
            Debug.Log("Transition 5");
            yield break;
        }
        transitionCanvas.SetActive(true);
        Debug.Log("Transition 6");

        //yield return new WaitForSeconds(vidDuration);
        Debug.Log("Transition 7");
    }

    private string GetNextSceneName()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = (currentIndex + 1) % SceneManager.sceneCountInBuildSettings;
        return SceneUtility.GetScenePathByBuildIndex(nextIndex)
            .Replace("Assets/Scenes/", "")
            .Replace(".unity", "");
    }
}
