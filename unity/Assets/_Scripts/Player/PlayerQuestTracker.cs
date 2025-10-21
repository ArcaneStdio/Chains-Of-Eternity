using UnityEngine;
using System.Collections.Generic;

public class PlayerQuestTracker : MonoBehaviour
{
    private const int MaxQuestSlots = 5;

    [Header("Player Quest Slots (Max 5)")]
    public Quest[] questSlots = new Quest[MaxQuestSlots];

    private QuestManager questManager;

    private void Start()
    {
        questManager = QuestManager.Instance;
        if (questManager == null)
        {
            Debug.LogError("❌ QuestManager instance not found in the scene!");
        }
    }

    /// <summary>
    /// Assigns a new quest to the first available slot.
    /// </summary>
    public bool AssignQuest(Quest quest)
    {
        if (quest == null)
        {
            Debug.LogWarning("⚠️ Attempted to assign a null quest!");
            return false;
        }

        // Prevent duplicate active quest
        foreach (var q in questSlots)
        {
            if (q == quest && q.state == QuestState.Active)
            {
                Debug.LogWarning($"⚠️ Quest '{quest.questName}' already active!");
                return false;
            }
        }

        // Find first free slot
        for (int i = 0; i < MaxQuestSlots; i++)
        {
            if (questSlots[i] == null || questSlots[i].state == QuestState.NotAssigned || questSlots[i].state == QuestState.Failed)
            {
                quest.Assign();
                questSlots[i] = quest;
                questManager?.AddQuest(quest);

                Debug.Log($"🧾 Quest Assigned: {quest.questName} → Slot {i + 1}");
                return true;
            }
        }

        Debug.LogWarning("⚠️ No free quest slots available!");
        return false;
    }

    /// <summary>
    /// Abandons an active quest and frees up the slot.
    /// </summary>
    public void AbandonQuest(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MaxQuestSlots)
        {
            Debug.LogWarning("⚠️ Invalid slot index!");
            return;
        }

        Quest quest = questSlots[slotIndex];
        if (quest != null && quest.state == QuestState.Active)
        {
            quest.MarkFailed();
            questManager?.RemoveQuest(quest);
            questSlots[slotIndex] = null;

            Debug.Log($"🚫 Quest Abandoned: {quest.questName}");
        }
        else
        {
            Debug.LogWarning("⚠️ No active quest in this slot to abandon!");
        }
    }

    /// <summary>
    /// Called when the player kills an enemy.
    /// </summary>
    public void OnEnemyKilled(EnemyType type)
    {
        bool anyUpdated = false;

        foreach (var quest in questSlots)
        {
            if (quest != null && quest.state == QuestState.Active)
            {
                quest.RegisterKill(type);
                anyUpdated = true;
            }
        }

        if (anyUpdated)
            questManager?.RegisterEnemyKill(type);
    }

    /// <summary>
    /// Checks if a given quest is currently assigned and active.
    /// </summary>
    public bool HasQuest(Quest quest)
    {
        foreach (var q in questSlots)
        {
            if (q == quest && q.state == QuestState.Active)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Returns all quests that should be shown in the GUI (active or completed).
    /// </summary>
    public Quest[] GetVisibleQuests()
    {
        List<Quest> visible = new();
        foreach (var quest in questSlots)
        {
            if (quest != null && quest.ShouldShowInGUI())
                visible.Add(quest);
        }
        return visible.ToArray();
    }

    /// <summary>
    /// Clears all quest slots (for testing/reset).
    /// </summary>
    public void ClearAllQuests()
    {
        for (int i = 0; i < MaxQuestSlots; i++)
        {
            if (questSlots[i] != null)
            {
                questSlots[i].ResetProgress();
                questManager?.RemoveQuest(questSlots[i]);
                questSlots[i] = null;
            }
        }
        Debug.Log("🧹 Cleared all quest slots.");
    }

    /// <summary>
    /// Returns the number of currently active quests.
    /// </summary>
    public int ActiveQuestCount()
    {
        int count = 0;
        foreach (var quest in questSlots)
        {
            if (quest != null && quest.state == QuestState.Active)
                count++;
        }
        return count;
    }
}
