using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    [SerializeField]
    private HeroData heroData;
    public static ProgressManager Instance { get; private set; }

    public int PlayerExperience { get; private set; } = 0;
    public int statPointsAvailable { get; private set; } = 5;
    public int statPointsTotal { get; private set; } = 5;

    // --- Base Stats ---
    public int vitalityStats { get; private set; } = 1;
    public int strengthStats { get; private set; } = 1;
    public int dexterityStats { get; private set; } = 1;
    public int intelligenceStats { get; private set; } = 1;
    public int enduranceStats { get; private set; } = 1;
    public int focusStats { get; private set; } = 1;

    // --- Delta (Pending Allocation) Stats ---
    public int deltaVitality { get; private set; } = 0;
    public int deltaStrength { get; private set; } = 0;
    public int deltaDexterity { get; private set; } = 0;
    public int deltaIntelligence { get; private set; } = 0;
    public int deltaEndurance { get; private set; } = 0;
    public int deltaFocus { get; private set; } = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Initialize from HeroData
        strengthStats = heroData.statPointsAssigned.strength;
        vitalityStats = heroData.statPointsAssigned.vitality;
        dexterityStats = heroData.statPointsAssigned.dexterity;
        intelligenceStats = heroData.statPointsAssigned.intelligence;
        enduranceStats = heroData.statPointsAssigned.endurance;
        focusStats = heroData.statPointsAssigned.focus;

        statPointsAvailable = heroData.statPointsAssigned.remainingPoints;
        PlayerExperience = heroData.statPointsAssigned.experience;
    }

    // --- Experience & Leveling ---
    public void AddExperience(int amount)
    {
        Debug.Log($"Adding {amount} experience.");
        PlayerExperience += amount;
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        int experienceNeeded = heroData.level * 100;
        if (PlayerExperience >= experienceNeeded)
        {
            PlayerExperience -= experienceNeeded;
            heroData.level++;
            statPointsAvailable += 3;
            statPointsTotal += 3;
            Debug.Log($"Level Up! New Level: {heroData.level}");
            AudioManager.Instance.PlayLevelUp();
        }
    }

    // --- Stat Modification ---
    private bool TrySpendStatPoint()
    {
        if (statPointsAvailable > 0)
        {
            statPointsAvailable--;
            return true;
        }
        Debug.LogWarning("No stat points available.");
        return false;
    }

    private void RefundStatPoint() => statPointsAvailable++;

    #region Increase/Decrease Methods
    public void IncreaseVitality()
    {
        if (TrySpendStatPoint())
            deltaVitality++;
    }

    public void DecreaseVitality()
    {
        if (CanDecrease(vitalityStats, deltaVitality))
        {
            deltaVitality--;
            RefundStatPoint();
        }
    }

    public void IncreaseStrength()
    {
        if (TrySpendStatPoint())
            deltaStrength++;
    }

    public void DecreaseStrength()
    {
        if (CanDecrease(strengthStats, deltaStrength))
        {
            deltaStrength--;
            RefundStatPoint();
        }
    }

    public void IncreaseDexterity()
    {
        if (TrySpendStatPoint())
            deltaDexterity++;
    }

    public void DecreaseDexterity()
    {
        if (CanDecrease(dexterityStats, deltaDexterity))
        {
            deltaDexterity--;
            RefundStatPoint();
        }
    }

    public void IncreaseIntelligence()
    {
        if (TrySpendStatPoint())
            deltaIntelligence++;
    }

    public void DecreaseIntelligence()
    {
        if (CanDecrease(intelligenceStats, deltaIntelligence))
        {
            deltaIntelligence--;
            RefundStatPoint();
        }
    }

    public void IncreaseEndurance()
    {
        if (TrySpendStatPoint())
            deltaEndurance++;
    }

    public void DecreaseEndurance()
    {
        if (CanDecrease(enduranceStats, deltaEndurance))
        {
            deltaEndurance--;
            RefundStatPoint();
        }
    }

    public void IncreaseFocus()
    {
        if (TrySpendStatPoint())
            deltaFocus++;
    }

    public void DecreaseFocus()
    {
        if (CanDecrease(focusStats, deltaFocus))
        {
            deltaFocus--;
            RefundStatPoint();
        }
    }

    private bool CanDecrease(int baseStat, int delta)
    {
        if (baseStat + delta - 1 >= 1)
            return true;
        Debug.LogWarning("Stat cannot be decreased below 1.");
        return false;
    }
    #endregion

    // --- Confirm Allocation ---
    public void Confirm()
    {
        // Apply deltas
        vitalityStats += deltaVitality;
        strengthStats += deltaStrength;
        dexterityStats += deltaDexterity;
        intelligenceStats += deltaIntelligence;
        enduranceStats += deltaEndurance;
        focusStats += deltaFocus;

        // Update heroData persistent stats
        var s = heroData.statPointsAssigned;
        s.vitality = vitalityStats;
        s.strength = strengthStats;
        s.dexterity = dexterityStats;
        s.intelligence = intelligenceStats;
        s.endurance = enduranceStats;
        s.focus = focusStats;
        s.remainingPoints = statPointsAvailable;
        s.experience = PlayerExperience;

        // --- Apply stat scaling to heroData ---
        ApplyScaling();

        // Reset deltas
        deltaVitality =
            deltaStrength =
            deltaDexterity =
            deltaIntelligence =
            deltaEndurance =
            deltaFocus =
                0;

        Debug.Log("Stats confirmed.");
    }

    private void ApplyScaling()
    {
        var o = heroData.offensiveStats;
        var d = heroData.defensiveStats;
        var sp = heroData.specialStats;

        // Vitality
        d.maxHealth = 200 + vitalityStats * 25;
        d.defense = 5 + Mathf.RoundToInt(vitalityStats * 0.5f);
        d.healthRegeneration = 1 + Mathf.RoundToInt(vitalityStats * 0.2f);

        // Strength
        o.damage = 15 + strengthStats * 3;
        o.criticalDamage = 50 + strengthStats * 1;

        // Dexterity
        o.attackSpeed = 100 + dexterityStats * 2;
        o.criticalRate = 10 + dexterityStats * 1;

        // Intelligence
        sp.maxMana = 150 + intelligenceStats * 15;
        sp.manaRegeneration = 5 + Mathf.RoundToInt(intelligenceStats * 0.5f);

        // Endurance
        sp.maxEnergy = 100 + enduranceStats * 10;
        sp.energyRegeneration = 5 + Mathf.RoundToInt(enduranceStats * 0.5f);

        // Focus
        o.damage += Mathf.RoundToInt(focusStats * 0.5f);
        o.criticalRate += focusStats * 1;
        o.criticalDamage += focusStats * 1;
    }
}
