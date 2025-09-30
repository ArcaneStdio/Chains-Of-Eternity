using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewHero", menuName = "BlockchainGame/HeroData")]
public class HeroData : ScriptableObject
{
    [Header("Basic Info")]
    public string playerName;
    public string playerID; // Could be wallet address as string
    public int level = 1;
    public bool isBanned;

    [Header("Race")]
    public string raceName;

    [Header("Equipped Items")]
    public List<string> equippedItems = new();

    [Header("Stats")]
    public OffensiveStats offensiveStats;
    public DefensiveStats defensiveStats;
    public SpecialStats specialStats;
    public StatPointsAssigned statPointsAssigned;
}

[System.Serializable]
public class OffensiveStats
{
    public int damage = 5;
    public int attackSpeed = 100;
    public int criticalRate = 10;
    public int criticalDamage = 50;
}

[System.Serializable]
public class DefensiveStats
{
    public int maxHealth = 100;
    public int defense = 5;
    public int healthRegeneration = 1;

    [Tooltip("0 - Stun, 1 - Fire, ...")]
    public List<int> resistances = new(); 
}

[System.Serializable]
public class SpecialStats
{
    public int maxEnergy = 100;
    public int energyRegeneration = 5;
    public int maxMana = 100;
    public int manaRegeneration = 5;
}

[System.Serializable]
public class StatPointsAssigned
{
    public int constitution = 1;
    public int strength = 1;
    public int dexterity = 1;
    public int intelligence = 1;
    public int stamina = 1;
    public int agility = 1;
    public int experience = 0; // Total experience points
    public int remainingPoints = 0;
    public void Reset()
    {
        constitution = 1;
        strength = 1;
        dexterity = 1;
        intelligence = 1;
        stamina = 1;
        agility = 1;
    }
}
