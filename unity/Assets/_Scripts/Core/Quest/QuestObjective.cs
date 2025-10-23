using UnityEngine;

[System.Serializable]
public class QuestObjective
{
    [Header("Enemy Kill Requirement")]
    public EnemyType enemyType;
    public int requiredKills;
    public int currentKills;    

    public bool IsComplete => currentKills >= requiredKills;

    public void RegisterKill(EnemyType type)
    {
        if (type == enemyType && currentKills < requiredKills)
        {
            currentKills++;
        }
    }
}
