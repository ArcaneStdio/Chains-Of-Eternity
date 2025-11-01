using UnityEngine;

public class marketplaceGuiHandler : MonoBehaviour
{

    [SerializeField] private Canvas tooltip;
    [SerializeField] private Canvas MarketplaceBuy;
    [SerializeField] private Canvas MarketplaceSell;
    private bool inRegion = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T) && inRegion)
        {
            MarketplaceBuy.enabled = !MarketplaceBuy.enabled;
        }

        if(Input.GetKeyDown(KeyCode.Y) && inRegion)
        {
            MarketplaceSell.enabled = !MarketplaceSell.enabled;
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
