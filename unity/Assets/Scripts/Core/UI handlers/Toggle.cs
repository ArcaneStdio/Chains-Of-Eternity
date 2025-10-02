using UnityEngine;

public class ToggleGameObjects : MonoBehaviour
{
    [Header("Assign your GameObjects here")]
    public GameObject objY;
    public GameObject objU;
    public GameObject objI;
    public GameObject objO;
    public GameObject objP;
    public GameObject objR;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            ToggleObject(objY);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            ToggleObject(objU);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleObject(objI);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            ToggleObject(objO);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            ToggleObject(objP);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ToggleObject(objR);
        }
    }

    void ToggleObject(GameObject obj)
    {
        if (obj != null)
        {
            obj.SetActive(!obj.activeSelf);
        }
    }
}
