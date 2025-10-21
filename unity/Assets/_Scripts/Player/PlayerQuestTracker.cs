using UnityEngine;
using System.Collections.Generic;

public class PlayerQuestTracker : MonoBehaviour
{
    private const int MaxQuestSlots = 5;

    [Header("Player Quest Slots (Max 5)")]
    [Tooltip("Pre-assigned Quest ScriptableObjects (5)")]
    public Quest[] questSlots = new Quest[MaxQuestSlots];

    private QuestManager questManager;

    private void Start()
    {
        questManager = QuestManager.Instance;
        if (questManager == null)
            Debug.LogError("❌ QuestManager instance not found in the scene!");
    }

    /// <summary>
    /// Assigns quest data into the first available quest slot (only if a free slot exists).
    /// </summary>
    public bool AssignQuest(Quest sourceQuest)
    {
        if (sourceQuest == null)
        {
            Debug.LogWarning("⚠️ Attempted to assign a null quest!");
            return false;
        }

        // Prevent duplicates
        foreach (var q in questSlots)
        {
            if (q != null && q.questID == sourceQuest.questID && q.state == QuestState.Active)
            {
                Debug.LogWarning($"⚠️ Quest '{sourceQuest.questName}' is already active!");
                return false;
            }
        }

        // Find a NotAssigned slot
        int freeSlotIndex = -1;
        for (int i = 0; i < MaxQuestSlots; i++)
        {
            if (questSlots[i] == null)
            {
                Debug.LogError($"⚠️ Quest slot {i + 1} is missing a Quest ScriptableObject reference!");
                return false;
            }

            if (questSlots[i].state == QuestState.NotAssigned)
            {
                freeSlotIndex = i;
                break;
            }
        }

        if (freeSlotIndex == -1)
        {
            Debug.LogWarning("⚠️ Cannot accept new quest — all 5 quest slots are currently filled!");
            return false;
        }

        Quest targetQuest = questSlots[freeSlotIndex];

        // Copy data first
        CopyQuestData(sourceQuest, targetQuest);

        // Then explicitly activate it
        targetQuest.Assign();
        targetQuest.state = QuestState.Active;

        questManager?.AddQuest(targetQuest);
        Debug.Log($"🧾 Quest Assigned: {sourceQuest.questName} → Slot {freeSlotIndex + 1}");
        return true;
    }


    /// <summary>
    /// Copies all relevant data from source quest to target quest (in-place overwrite).
    /// </summary>
    private void CopyQuestData(Quest source, Quest target)
    {
        target.questID = source.questID;
        target.questName = source.questName;
        target.questDescription = source.questDescription;
        target.experienceReward = source.experienceReward;
        target.tokenReward = source.tokenReward;
        target.rarity = source.rarity;
        target.recommendedLevel = source.recommendedLevel;
        // Deep copy objectives
        target.objectives.Clear();
        foreach (var obj in source.objectives)
        {
            QuestObjective newObj = new QuestObjective
            {
                enemyType = obj.enemyType,
                requiredKills = obj.requiredKills,
                currentKills = 0
            };
            target.objectives.Add(newObj);
        }

    }

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
            quest.state = QuestState.NotAssigned;
            quest.ResetProgress();

            Debug.Log($"🚫 Quest Abandoned: {quest.questName}");
        }
        else
        {
            Debug.LogWarning("⚠️ No active quest in this slot to abandon!");
        }
    }

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

    public bool HasQuest(Quest quest)
    {
        foreach (var q in questSlots)
        {
            if (q != null && q.questID == quest.questID && q.state == QuestState.Active)
                return true;
        }
        return false;
    }

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

    public void ClearAllQuests()
    {
        for (int i = 0; i < MaxQuestSlots; i++)
        {
            if (questSlots[i] != null)
            {
                questSlots[i].ResetProgress();
                questSlots[i].state = QuestState.NotAssigned;
                questManager?.RemoveQuest(questSlots[i]);
            }
        }
        Debug.Log("🧹 Cleared all quest slots.");
    }

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
