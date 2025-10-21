using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Active Quests")]
    public List<Quest> activeQuests = new();

    private void Awake()
    {
        // Enforce singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterEnemyKill(EnemyType type)
    {
        foreach (var quest in activeQuests)
        {
            if (!quest.IsCompleted)
            {
                quest.RegisterKill(type);

                if (quest.IsCompleted)
                {
                    Debug.Log($"✅ Quest Completed: {quest.questName}");
                    // TODO: Emit on-chain event or notify backend here
                }
            }
        }
    }

    public float GetQuestProgress(Quest quest)
    {
        return quest.GetProgress();
    }

    public void AddQuest(Quest quest)
    {
        if (!activeQuests.Contains(quest))
        {
            quest.ResetProgress();
            activeQuests.Add(quest);
            Debug.Log($"📜 Quest Added: {quest.questName}");
        }
    }

    public void RemoveQuest(Quest quest)
    {
        if (activeQuests.Contains(quest))
        {
            activeQuests.Remove(quest);
            Debug.Log($"🗑️ Quest Removed: {quest.questName}");
        }
    }
}
