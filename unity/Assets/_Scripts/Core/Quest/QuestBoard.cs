using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class QuestBoard : MonoBehaviour
{
    public static QuestBoard Instance { get; private set; }

    [Header("Backend Quest Pool (10 fixed)")]
    public List<Quest> availableQuests = new(); // 10 ScriptableObjects in Inspector

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Returns all current available quests for the UI.
    /// </summary>
    public List<Quest> GetCurrentQuests()
    {
        return availableQuests;
    }

    /// <summary>
    /// Called by QuestBoardUI when new quests are fetched from backend.
    /// It dumps quest data into existing ScriptableObjects.
    /// </summary>
    public void UpdateQuestsFromBackend(List<QuestDataDTO> backendQuests)
    {
        int count = Mathf.Min(availableQuests.Count, backendQuests.Count);

        for (int i = 0; i < count; i++)
        {
            Quest q = availableQuests[i];
            QuestDataDTO data = backendQuests[i];

            q.questID = data.id;
            q.questName = data.name;
            q.questDescription = data.description;
            q.experienceReward = data.expReward;
            q.recommendedLevel = data.recommendedLevel;
            q.rarity = GetRarity(data.rarity);
            q.tokenReward = data.tokenReward;
            q.objectives = data.objectives;
            q.state = QuestState.NotAssigned;
        }

        Debug.Log($"🌐 QuestBoard updated with {count} backend quests.");
    }
    public QuestRarity GetRarity(int rarityValue)
    {
        return rarityValue switch
        {
            0 => QuestRarity.Common,
            1 => QuestRarity.Rare,
            2 => QuestRarity.Epic,
            3 => QuestRarity.Legendary,
            _ => QuestRarity.Common,
        };
    }

}
