using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    [SerializeField] private HeroData heroData;
    public static ProgressManager Instance { get; private set; }
    public int PlayerExperience { get; private set; } = 0;

    public int statPointsAvailable { get; private set; } = 5;
    public int statPointsTotal { get; private set; } = 5;

    // Base stats
    public int constituionStats { get; private set; } = 1;
    public int strengthStats { get; private set; } = 1;
    public int dexterityStats { get; private set; } = 1;
    public int intelligenceStats { get; private set; } = 1;
    public int staminaStats { get; private set; } = 1;
    public int agilityStats { get; private set; } = 1;

    // Delta stats
    public int deltaConstitution { get; private set; } = 0;
    public int deltaStrength { get; private set; } = 0;
    public int deltaDexterity { get; private set; } = 0;
    public int deltaIntelligence { get; private set; } = 0;
    public int deltaStamina { get; private set; } = 0;
    public int deltaAgility { get; private set; } = 0;

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

    public void Start()
    {
        strengthStats = heroData.statPointsAssigned.strength;
        constituionStats = heroData.statPointsAssigned.constitution;
        dexterityStats = heroData.statPointsAssigned.dexterity;
        intelligenceStats = heroData.statPointsAssigned.intelligence;
        staminaStats = heroData.statPointsAssigned.stamina;
        agilityStats = heroData.statPointsAssigned.agility;
        statPointsAvailable = heroData.statPointsAssigned.remainingPoints;
        PlayerExperience = heroData.statPointsAssigned.experience;

        //Debug.Log("statsPointsAvailable: " + statPointsAvailable);
        //Debug.Log("statsPointsTotal: " + statPointsTotal);
        //Debug.Log("PlayerExperience: " + PlayerExperience);
        //Debug.Log("Hero Level: " + heroData.level);
        //Debug.Log("Hero Constitution: " + constituionStats);
        //Debug.Log("Hero Strength: " + strengthStats);
        //Debug.Log("Hero Dexterity: " + dexterityStats);
        //Debug.Log("Hero Intelligence: " + intelligenceStats);
        //Debug.Log("Hero Stamina: " + staminaStats);
        //Debug.Log("Hero Agility: " + agilityStats);
        //Debug.Log("Delta Constitution: " + deltaConstitution);
        //Debug.Log("Delta Strength: " + deltaStrength);
        //Debug.Log("Delta Dexterity: " + deltaDexterity);
        //Debug.Log("Delta Intelligence: " + deltaIntelligence);
        //Debug.Log("Delta Stamina: " + deltaStamina);
        //Debug.Log("Delta Agility: " + deltaAgility);
    }

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

    #region Stat Modification Methods

    private bool TrySpendStatPoint()
    {
        if (statPointsAvailable > 0)
        {
            statPointsAvailable--;
            Debug.Log("Spent a stat point. Remaining: " + statPointsAvailable);
            return true;
        }
        Debug.LogWarning("No stat points available.");
        return false;
    }

    private void RefundStatPoint()
    {
        statPointsAvailable++;
    }

    public void IncreaseConstitution()
    {
        if (TrySpendStatPoint())
        {
            deltaConstitution++;
            Debug.Log("Delta Constitution increased: " + deltaConstitution);
        }
    }

    public void DecreaseConstitution()
    {
        if (constituionStats + deltaConstitution - 1 >= 1)
        {
            deltaConstitution--;
            RefundStatPoint();
            Debug.Log("Delta Constitution decreased: " + deltaConstitution);
        }
        else
        {
            Debug.LogWarning("Constitution cannot be decreased below 1.");
        }
    }

    public void IncreaseStrength()
    {
        if (TrySpendStatPoint())
        {
            deltaStrength++;
            Debug.Log("Delta Strength increased: " + deltaStrength);
        }
    }

    public void DecreaseStrength()
    {
        if (strengthStats + deltaStrength - 1 >= 1)
        {
            deltaStrength--;
            RefundStatPoint();
            Debug.Log("Delta Strength decreased: " + deltaStrength);
        }
        else
        {
            Debug.LogWarning("Strength cannot be decreased below 1.");
        }
    }

    public void IncreaseDexterity()
    {
        if (TrySpendStatPoint())
        {
            deltaDexterity++;
            Debug.Log("Delta Dexterity increased: " + deltaDexterity);
        }
    }

    public void DecreaseDexterity()
    {
        if (dexterityStats + deltaDexterity - 1 >= 1)
        {
            deltaDexterity--;
            RefundStatPoint();
            Debug.Log("Delta Dexterity decreased: " + deltaDexterity);
        }
        else
        {
            Debug.LogWarning("Dexterity cannot be decreased below 1.");
        }
    }

    public void IncreaseIntelligence()
    {
        if (TrySpendStatPoint())
        {
            deltaIntelligence++;
            Debug.Log("Delta Intelligence increased: " + deltaIntelligence);
        }
    }

    public void DecreaseIntelligence()
    {
        if (intelligenceStats + deltaIntelligence - 1 >= 1)
        {
            deltaIntelligence--;
            RefundStatPoint();
            Debug.Log("Delta Intelligence decreased: " + deltaIntelligence);
        }
        else
        {
            Debug.LogWarning("Intelligence cannot be decreased below 1.");
        }
    }

    public void IncreaseStamina()
    {
        if (TrySpendStatPoint())
        {
            deltaStamina++;
            Debug.Log("Delta Stamina increased: " + deltaStamina);
        }
    }

    public void DecreaseStamina()
    {
        if (staminaStats + deltaStamina - 1 >= 1)
        {
            deltaStamina--;
            RefundStatPoint();
            Debug.Log("Delta Stamina decreased: " + deltaStamina);
        }
        else
        {
            Debug.LogWarning("Stamina cannot be decreased below 1.");
        }
    }

    public void IncreaseAgility()
    {
        if (TrySpendStatPoint())
        {
            deltaAgility++;
            Debug.Log("Delta Agility increased: " + deltaAgility);
        }
    }

    public void DecreaseAgility()
    {
        if (agilityStats + deltaAgility - 1 >= 1)
        {
            deltaAgility--;
            RefundStatPoint();
            Debug.Log("Delta Agility decreased: " + deltaAgility);
        }
        else
        {
            Debug.LogWarning("Agility cannot be decreased below 1.");
        }
    }

    public void Confirm()
    {
        // Apply deltas to actual stats
        constituionStats += deltaConstitution;
        strengthStats += deltaStrength;
        dexterityStats += deltaDexterity;
        intelligenceStats += deltaIntelligence;
        staminaStats += deltaStamina;
        agilityStats += deltaAgility;



        // Apply effects to heroData (examples)

        heroData.statPointsAssigned.strength = strengthStats;
        heroData.statPointsAssigned.constitution = constituionStats;
        heroData.statPointsAssigned.dexterity = dexterityStats;
        heroData.statPointsAssigned.intelligence = intelligenceStats;
        heroData.statPointsAssigned.stamina = staminaStats;
        heroData.statPointsAssigned.agility = agilityStats;
        heroData.statPointsAssigned.remainingPoints = statPointsAvailable;
        heroData.statPointsAssigned.experience = PlayerExperience;

        // Logic to change the actual player stats of hero data
        heroData.offensiveStats.damage += deltaStrength;
        heroData.defensiveStats.maxHealth += deltaConstitution * 10;




        // Reset deltas
        deltaConstitution = 0;
        deltaStrength = 0;
        deltaDexterity = 0;
        deltaIntelligence = 0;
        deltaStamina = 0;
        deltaAgility = 0;

        Debug.Log("Stats confirmed.");
    }
    #endregion
}
