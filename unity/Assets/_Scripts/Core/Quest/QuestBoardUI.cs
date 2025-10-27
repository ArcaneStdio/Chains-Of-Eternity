using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestBoardUI : MonoBehaviour
{
    [Header("References")]
    public List<QuestSlotUI> questSlots = new(); // 10 slots, manually assigned
    public ExpandedQuestView expandedQuestView; // Central expanded panel
    public PlayerQuestTracker playerQuestTracker;
    public Button refreshButton;

    private void Start()
    {
        if (playerQuestTracker == null)
            playerQuestTracker = FindObjectOfType<PlayerQuestTracker>();

        if (refreshButton != null)
            refreshButton.onClick.AddListener(FetchNewQuests);

        if (expandedQuestView != null)
            expandedQuestView.Hide();

        RefreshBoardUI();
    }

    /// <summary>
    /// Refreshes visible quest slots.
    /// </summary>
    public void RefreshBoardUI()
    {
        List<Quest> quests = QuestBoard.Instance.GetCurrentQuests();

        for (int i = 0; i < questSlots.Count; i++)
        {
            if (i < quests.Count && quests[i] != null)
                questSlots[i].SetupSlot(quests[i], this);
            else
                questSlots[i].ClearSlot();
        }
    }

    /// <summary>
    /// When a quest slot is clicked, expand its details.
    /// </summary>
    public void OnQuestClicked(Quest quest, GameObject slot)
    {
        if (expandedQuestView != null && quest != null)
        {
            expandedQuestView.DisplayQuest(quest, slot);
            expandedQuestView.Show();
        }
    }

    /// <summary>
    /// Accept quest through expanded view.
    /// </summary>
    public void AcceptQuest(Quest quest)
    {
        if (playerQuestTracker.AssignQuest(quest))
        {
            Debug.Log($"✅ Accepted quest: {quest.questName}");
        }
        else
        {
            Debug.Log("⚠️ Could not accept quest (maybe full slots?)");
        }
    }

    /// <summary>
    /// Simulates backend fetch; updates QuestBoard and UI.
    /// </summary>
    public void FetchNewQuests()
    {
        Debug.Log("🌐 Fetching new quests from backend...");

        List<QuestDataDTO> fakeData = new();
        for (int i = 0; i < 10; i++)
        {
            fakeData.Add(
                new QuestDataDTO
                {
                    id = $"Q{i + 1}",
                    name = $"Quest #{i + 1}",
                    description = $"Defeat {Random.Range(2, 6)} enemies for Quest #{i + 1}.",
                    recommendedLevel = $"{Random.Range(1, 10)}",
                    rarity = Random.Range(0, 4),
                    expReward = Random.Range(100, 400),
                    tokenReward = Random.Range(10, 50),
                    objectives = new List<QuestObjective>
                    {
                        new QuestObjective
                        {
                            enemyType = EnemyType.Slime,
                            requiredKills = Random.Range(1, 4),
                        },
                        new QuestObjective
                        {
                            enemyType = EnemyType.Fire_Worm,
                            requiredKills = Random.Range(1, 3),
                        },
                    },
                }
            );
        }

        QuestBoard.Instance.UpdateQuestsFromBackend(fakeData);
        RefreshBoardUI();
    }
}
