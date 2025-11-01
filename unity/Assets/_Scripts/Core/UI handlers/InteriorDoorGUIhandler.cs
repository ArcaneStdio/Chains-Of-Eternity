using UnityEngine;
using UnityEngine.SceneManagement;

public class InteriorDoorGUIhandler : MonoBehaviour
{
    [SerializeField] private Canvas tooltip;
    [SerializeField] private Vector3 outdoorSpawnPosition; // set manually in Inspector

    private bool inRegion = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && inRegion)
        {
            // Remember the spawn position for the next scene
            SpawnPoint.NextSpawnPosition = outdoorSpawnPosition;

            // Load the outdoor scene
            SceneManager.LoadScene("village_scene");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            tooltip.enabled = true;
            inRegion = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            tooltip.enabled = false;
            inRegion = false;
        }
    }
}
