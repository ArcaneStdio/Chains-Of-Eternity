using System.Collections.Generic;

[System.Serializable]
public class QuestDataDTO
{
    public string id;
    public string name;
    public string description;
    public string recommendedLevel;
    public int expReward;
    public int rarity;// where 0=Common,1=Rare,2=Epic,3=Legendary
    public int tokenReward;
    public List<QuestObjective> objectives;
}

public enum QuestRarity
{
    Common,
    Rare,
    Epic,
    Legendary
}
public enum QuestState
{
    NotAssigned,
    Active,
    Completed,
    Failed
}