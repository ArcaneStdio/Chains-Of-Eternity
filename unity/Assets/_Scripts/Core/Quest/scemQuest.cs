 using UnityEngine;

public class scemQuest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void joinq()
    {
        Web3AuthManager.Instance.JoinQuest(1);
    }
}
