using UnityEngine;

public class enabler : MonoBehaviour
{
    [SerializeField] private GameObject menu;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            menu.SetActive(false);
        }

        if(Input.GetKeyDown(KeyCode.N))
        {
            menu.SetActive(true);
        }
    }
}
