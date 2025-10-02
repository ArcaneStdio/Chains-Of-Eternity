using DialogueEditor;
using UnityEngine;

public class startConversation : MonoBehaviour
{

    public NPCConversation conversation;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startConversationFunction()
    {
        if (conversation != null)
        {
            ConversationManager.Instance.StartConversation(conversation);
        }
        else
        {
            Debug.LogWarning("No conversation assigned to start.");
        }
    }
}
