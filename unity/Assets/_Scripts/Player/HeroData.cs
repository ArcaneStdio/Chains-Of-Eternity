using System.Collections.Generic;
using UnityEngine;

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

    public void ApplyStatScaling(
        StatPointsAssigned stats,
        OffensiveStats offensive,
        DefensiveStats defensive,
        SpecialStats special
    )
    {
        // Vitality
        defensive.maxHealth += stats.vitality * 25;
        defensive.defense += Mathf.RoundToInt(stats.vitality * 0.5f);
        defensive.healthRegeneration += Mathf.RoundToInt(stats.vitality * 0.2f);

        // Strength
        offensive.damage += stats.strength * 3;
        offensive.criticalDamage += stats.strength * 1;

        // Dexterity
        offensive.attackSpeed += stats.dexterity * 2;
        offensive.criticalRate += stats.dexterity * 1;

        // Intelligence
        special.maxMana += stats.intelligence * 15;
        special.manaRegeneration += Mathf.RoundToInt(stats.intelligence * 0.5f);

        // Endurance
        special.maxEnergy += stats.endurance * 10;
        special.energyRegeneration += Mathf.RoundToInt(stats.endurance * 0.5f);

        // Focus
        offensive.damage += Mathf.RoundToInt(stats.focus * 0.5f);
        offensive.criticalRate += stats.focus * 1;
        offensive.criticalDamage += stats.focus * 1;
    }
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
    public int vitality = 1;
    public int strength = 1;
    public int dexterity = 1;
    public int intelligence = 1;
    public int endurance = 1;
    public int focus = 1;

    public int experience = 0;
    public int remainingPoints = 0;

    public void Reset()
    {
        vitality = 1;
        strength = 1;
        dexterity = 1;
        intelligence = 1;
        endurance = 1;
        focus = 1;
    }
}
