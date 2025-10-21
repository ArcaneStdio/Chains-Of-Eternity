using UnityEngine;

public class DesertSceneTransition : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {   
        Debug.Log("Desert scene transition triggered by: " + other.gameObject.name);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the desert scene transition area.");
            // Load the desert scene here
            UnityEngine.SceneManagement.SceneManager.LoadScene("Desert");
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
