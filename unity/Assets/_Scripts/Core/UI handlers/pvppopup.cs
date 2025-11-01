using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class pvppopup : MonoBehaviour
{
    [SerializeField] private Canvas tooltip;
    [SerializeField] private Canvas marketplace;
    [SerializeField] private bool isPvP = false;

    private bool inRegion = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && inRegion)
        {
            if(isPvP)
            {
                SceneManager.LoadScene("LobbyStart Safe_ryuga");
            }else
            {
                marketplace.enabled = !marketplace.enabled;
            }
            //SceneTransitionManager.Instance.
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            tooltip.enabled = true;
            inRegion = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            tooltip.enabled = false;
            inRegion = false;
        }
    }

    
}
