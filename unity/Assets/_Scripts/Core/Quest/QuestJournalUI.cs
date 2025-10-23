using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class QuestJournalUI : MonoBehaviour
{
    [Header("References")]
    public GameObject questEntryPrefab;
    public Transform questListParent;
    public Button closeButton;

    private QuestManager questManager;
    private PlayerQuestTracker playerTracker;

    private void Start()
    {
        questManager = QuestManager.Instance;
        playerTracker = FindObjectOfType<PlayerQuestTracker>();

        if (closeButton != null)
            closeButton.onClick.AddListener(() => gameObject.SetActive(false));
    }

    private void OnEnable()
    {
        RefreshQuestList();
    }

    /// <summary>
    /// Refreshes the quest journal UI with all currently active quests.
    /// </summary>
    public void RefreshQuestList()
    {
        if (questManager == null || playerTracker == null)
        {
            Debug.LogError("❌ Missing QuestManager or PlayerQuestTracker reference.");
            return;
        }

        // Clear old UI
        foreach (Transform child in questListParent)
            Destroy(child.gameObject);

        List<Quest> activeQuests = questManager.activeQuests;

        foreach (Quest quest in activeQuests)
        {
            GameObject entryGO = Instantiate(questEntryPrefab, questListParent);
            QuestEntryUI entry = entryGO.GetComponent<QuestEntryUI>();
            //entry.Setup(quest, () => OnAbandonQuest(quest));
        }
    }

    /// <summary>
    /// Abandon a quest (handled both in tracker and manager).
    /// </summary>
    private void OnAbandonQuest(Quest quest)
    {
        // Find the quest in tracker slots
        for (int i = 0; i < 5; i++)
        {
            Quest slotQuest = playerTracker.questSlots[i];
            if (slotQuest != null && slotQuest.questID == quest.questID && slotQuest.state == QuestState.Active)
            {
                playerTracker.AbandonQuest(i);
                break;
            }
        }

        RefreshQuestList();
    }
}
