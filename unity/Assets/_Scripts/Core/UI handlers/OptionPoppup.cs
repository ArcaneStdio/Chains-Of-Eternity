using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionPoppup : MonoBehaviour
{
    [SerializeField] private Canvas tooltip;
    [SerializeField] private Canvas marketplace;
    [SerializeField] private bool isTownHall = false;

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
            if(isTownHall)
            {
                SceneManager.LoadScene("Interior");
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
