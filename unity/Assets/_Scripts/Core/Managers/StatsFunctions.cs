using TMPro;
using UnityEngine;

public class StatsFunctions : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField]
    private TextMeshProUGUI vitalityText;

    [SerializeField]
    private TextMeshProUGUI strengthText;

    [SerializeField]
    private TextMeshProUGUI dexterityText;

    [SerializeField]
    private TextMeshProUGUI intelligenceText;

    [SerializeField]
    private TextMeshProUGUI enduranceText;

    [SerializeField]
    private TextMeshProUGUI focusText;

    [SerializeField]
    private TextMeshProUGUI levelText;

    [SerializeField]
    private HeroData heroData;

    private ProgressManager pm => ProgressManager.Instance;

    private void Start()
    {
        Debug.Log("Initializing stat UI with HeroData values.");
        RefreshAllTexts();
    }

    private void Update()
    {
        // Update level text dynamically if hero levels up
        if (levelText.text != $"Level {heroData.level}")
            levelText.text = $"Level {heroData.level}";
    }

    // 🔄 Refresh all stat fields at once
    private void RefreshAllTexts()
    {
        vitalityText.text = heroData.statPointsAssigned.vitality.ToString();
        strengthText.text = heroData.statPointsAssigned.strength.ToString();
        dexterityText.text = heroData.statPointsAssigned.dexterity.ToString();
        intelligenceText.text = heroData.statPointsAssigned.intelligence.ToString();
        enduranceText.text = heroData.statPointsAssigned.endurance.ToString();
        focusText.text = heroData.statPointsAssigned.focus.ToString();
        levelText.text = $"Level {heroData.level}";
    }

    // === Increase methods ===
    public void IncreaseVitality()
    {
        pm.IncreaseVitality();
        vitalityText.text = (pm.vitalityStats + pm.deltaVitality).ToString();
    }

    public void IncreaseStrength()
    {
        pm.IncreaseStrength();
        strengthText.text = (pm.strengthStats + pm.deltaStrength).ToString();
    }

    public void IncreaseDexterity()
    {
        pm.IncreaseDexterity();
        dexterityText.text = (pm.dexterityStats + pm.deltaDexterity).ToString();
    }

    public void IncreaseIntelligence()
    {
        pm.IncreaseIntelligence();
        intelligenceText.text = (pm.intelligenceStats + pm.deltaIntelligence).ToString();
    }

    public void IncreaseEndurance()
    {
        pm.IncreaseEndurance();
        enduranceText.text = (pm.enduranceStats + pm.deltaEndurance).ToString();
    }

    public void IncreaseFocus()
    {
        pm.IncreaseFocus();
        focusText.text = (pm.focusStats + pm.deltaFocus).ToString();
    }

    // === Decrease methods ===
    public void DecreaseVitality()
    {
        pm.DecreaseVitality();
        vitalityText.text = (pm.vitalityStats + pm.deltaVitality).ToString();
    }

    public void DecreaseStrength()
    {
        pm.DecreaseStrength();
        strengthText.text = (pm.strengthStats + pm.deltaStrength).ToString();
    }

    public void DecreaseDexterity()
    {
        pm.DecreaseDexterity();
        dexterityText.text = (pm.dexterityStats + pm.deltaDexterity).ToString();
    }

    public void DecreaseIntelligence()
    {
        pm.DecreaseIntelligence();
        intelligenceText.text = (pm.intelligenceStats + pm.deltaIntelligence).ToString();
    }

    public void DecreaseEndurance()
    {
        pm.DecreaseEndurance();
        enduranceText.text = (pm.enduranceStats + pm.deltaEndurance).ToString();
    }

    public void DecreaseFocus()
    {
        pm.DecreaseFocus();
        focusText.text = (pm.focusStats + pm.deltaFocus).ToString();
    }

    // ✅ Confirm changes
    public void Confirm()
    {
        pm.Confirm();
        RefreshAllTexts();
        Debug.Log("Stats confirmed and UI updated.");
    }
}
