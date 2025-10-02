using DialogueEditor;
using UnityEngine;

public class TriggerHandler : MonoBehaviour
{
    private NPCConversation conversation;
    private bool playerInTrigger = false;

    private void Awake()
    {
        conversation = GetComponent<NPCConversation>();
    }

    private void Update()
    {
        if (playerInTrigger && InputManager.Instance.InteractPressed)
        {
            Debug.Log("Detected input");
            if (ConversationManager.Instance.IsConversationActive)
            {
                // Handle active conversation
            }
            else
            {
                Debug.Log("Starting conversation with NPC: " + gameObject.name);
                if (conversation != null)
                {
                    ConversationManager.Instance.StartConversation(conversation);
                }
                else
                {
                    Debug.LogWarning("NPCConversation component is missing on the GameObject with TriggerHandler.");
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInTrigger = true;
            Debug.Log("Player entered trigger area of: " + gameObject.name);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInTrigger = false;
            Debug.Log("Player exited trigger area of: " + gameObject.name);
        }
    }
}