using TMPro;
using UnityEngine;

public class StatsFunctions : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI strengthText;
    [SerializeField] private TextMeshProUGUI dexterityText;
    [SerializeField] private TextMeshProUGUI constitutionText;
    [SerializeField] private TextMeshProUGUI intelligenceText;
    [SerializeField] private TextMeshProUGUI staminaText;
    [SerializeField] private TextMeshProUGUI agilityText;
    [SerializeField] private TextMeshProUGUI levelText;

    [SerializeField] private HeroData heroData;

    private void Start()
    {
        Debug.Log("Start called. Setting stat text fields.");

        strengthText.text = heroData.statPointsAssigned.strength.ToString();
        dexterityText.text = heroData.statPointsAssigned.dexterity.ToString();
        constitutionText.text = heroData.statPointsAssigned.constitution.ToString();
        intelligenceText.text = heroData.statPointsAssigned.intelligence.ToString();
        staminaText.text = heroData.statPointsAssigned.stamina.ToString();
        agilityText.text = heroData.statPointsAssigned.agility.ToString();
        levelText.text = "Level " + heroData.level.ToString();
    }

    private void Update()
    {
        // Update the level text if the hero's level changes
        if (heroData.level != int.Parse(levelText.text.Split(' ')[1]))
        {
            levelText.text = "Level " + heroData.level.ToString();
        }
    }

    public void increaseStrength()
    {
        Debug.Log("Increasing Strength");
        ProgressManager.Instance.IncreaseStrength();
        strengthText.text = (ProgressManager.Instance.strengthStats + ProgressManager.Instance.deltaStrength).ToString();
    }

    public void increaseDexterity()
    {
        ProgressManager.Instance.IncreaseDexterity();
        dexterityText.text = (ProgressManager.Instance.dexterityStats + ProgressManager.Instance.deltaDexterity).ToString();

    }

    public void increaseConstitution()
    {
        ProgressManager.Instance.IncreaseConstitution();
        constitutionText.text = (ProgressManager.Instance.constituionStats + ProgressManager.Instance.deltaConstitution).ToString();

    }

    public void increaseIntelligence()
    {
        ProgressManager.Instance.IncreaseIntelligence();
        intelligenceText.text = (ProgressManager.Instance.intelligenceStats + ProgressManager.Instance.deltaIntelligence).ToString();

    }

    public void increaseStamina()
    {
        ProgressManager.Instance.IncreaseStamina();
        staminaText.text = (ProgressManager.Instance.staminaStats + ProgressManager.Instance.deltaStamina).ToString();

    }

    public void increaseAgility()
    {
        ProgressManager.Instance.IncreaseAgility();
        agilityText.text = (ProgressManager.Instance.deltaAgility + ProgressManager.Instance.agilityStats).ToString();

    }

    public void decreaseStrength()
    {
        ProgressManager.Instance.DecreaseStrength();
        strengthText.text = (ProgressManager.Instance.strengthStats + ProgressManager.Instance.deltaStrength).ToString();
        //strengthText.text = ProgressManager.Instance.strengthStats.ToString();
    }

    public void decreaseDexterity()
    {
        ProgressManager.Instance.DecreaseDexterity();
        dexterityText.text = (ProgressManager.Instance.dexterityStats + ProgressManager.Instance.deltaDexterity).ToString();
    }

    public void decreaseConstitution()
    {
        ProgressManager.Instance.DecreaseConstitution();
        constitutionText.text = (ProgressManager.Instance.constituionStats + ProgressManager.Instance.deltaConstitution).ToString();
    }

    public void decreaseIntelligence()
    {
        ProgressManager.Instance.DecreaseIntelligence();
        intelligenceText.text = (ProgressManager.Instance.intelligenceStats + ProgressManager.Instance.deltaIntelligence).ToString();
    }

    public void decreaseStamina()
    {
        ProgressManager.Instance.DecreaseStamina();
        staminaText.text = (ProgressManager.Instance.staminaStats + ProgressManager.Instance.deltaStamina).ToString();
    }

    public void decreaseAgility()
    {
        ProgressManager.Instance.DecreaseAgility();
        agilityText.text = (ProgressManager.Instance.deltaAgility + ProgressManager.Instance.agilityStats).ToString();
    }

    public void confirm()
    {
        ProgressManager.Instance.Confirm();
        Debug.Log("Stats confirmed. Changes applied.");
    }
}
