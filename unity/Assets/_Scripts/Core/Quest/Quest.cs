using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quests/Kill Quest")]
public class Quest : ScriptableObject
{
    [Header("Quest Info")]
    public string questID;
    public string questName;
    public QuestRarity rarity;

    [TextArea]
    public string questDescription;

    [Header("Objectives")]
    public List<QuestObjective> objectives = new();

    [Header("Rewards")]
    public int experienceReward;
    public int tokenReward;
    public string recommendedLevel;

    [Header("Quest State")]
    public QuestState state = QuestState.NotAssigned;

    [Header("Tracking Info")]
    public DateTime assignedTime;
    public DateTime? expiryTime; // optional (for on-chain timing or expiry)

    public bool IsCompleted
    {
        get
        {
            foreach (var obj in objectives)
            {
                if (!obj.IsComplete)
                    return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Percentage of quest progress [0, 1].
    /// </summary>
    public float GetProgress()
    {
        if (objectives.Count == 0)
            return 0f;

        float totalRequired = 0;
        float totalProgress = 0;

        foreach (var obj in objectives)
        {
            totalRequired += obj.requiredKills;
            totalProgress += Mathf.Min(obj.currentKills, obj.requiredKills);
        }

        return totalProgress / totalRequired;
    }

    /// <summary>
    /// Registers a kill toward quest objectives.
    /// </summary>
    public void RegisterKill(EnemyType type)
    {
        if (state != QuestState.Active)
            return;

        foreach (var obj in objectives)
        {
            obj.RegisterKill(type);
        }

        if (IsCompleted)
            MarkCompleted();
    }

    public void ResetProgress()
    {
        foreach (var obj in objectives)
            obj.currentKills = 0;

        //state = QuestState.NotAssigned;
    }

    public void Assign()
    {
        ResetProgress();
        state = QuestState.Active;
        assignedTime = DateTime.UtcNow;
    }

    public void MarkCompleted()
    {
        state = QuestState.Completed;
        Debug.Log($"✅ Quest Completed: {questName}");
    }

    public void MarkFailed()
    {
        state = QuestState.Failed;
        Debug.Log($"❌ Quest Failed: {questName}");
    }

    /// <summary>
    /// Determines if this quest should be visible in GUI (only Active or Completed).
    /// </summary>
    public bool ShouldShowInGUI()
    {
        return state == QuestState.Active || state == QuestState.Completed;
    }
}
