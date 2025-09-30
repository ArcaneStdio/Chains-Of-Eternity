using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using System.Threading;

public class CanvasSceneTransition : MonoBehaviour
{
    [Header("Transition Settings")]
    [SerializeField] private string sceneToLoad;          // Scene name (or leave empty to load next in build order)
    [SerializeField] private float vidDuration = 2f;     // Duration of fade
    [SerializeField] private GameObject transitionCanvas; // CanvasGroup for fading
    [SerializeField] private bool callinSpellTest = false; // If true, transition is triggered externally
    [SerializeField] private GameObject resetSpell; // Reference to ResetSpell script
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
        Destroy(resetSpell);    
        if (!isTransitioning)
        {
            StartCoroutine(DoSceneTransition());
        }

    }


    private IEnumerator DoSceneTransition()
    {
        isTransitioning = true;
        //player.Enable_DisableInput(false); 
        // Fade in (to black)
        yield return StartCoroutine(Activatee());

        // Load next scene
        string nextScene = string.IsNullOrEmpty(sceneToLoad)
            ? GetNextSceneName()
            : sceneToLoad;

        yield return SceneManager.LoadSceneAsync(nextScene);

        isTransitioning = false;
    }

    private IEnumerator Activatee()
    {
        if (transitionCanvas == null)
            yield break;
        transitionCanvas.SetActive(true);

        yield return new WaitForSeconds(vidDuration);
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
