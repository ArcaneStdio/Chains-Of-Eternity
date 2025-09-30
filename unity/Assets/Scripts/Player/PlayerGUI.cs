using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerGUI : MonoBehaviour
{
    public GameObject Player;
    public Slider HealthSlider;
    public Slider StaminaSlider;
    public Slider ManaSlider;
    public TextMeshProUGUI HealthText;
    public TextMeshProUGUI AttackText;
    public TextMeshProUGUI DefenseText;
    public TextMeshProUGUI EnergyText;
    public TextMeshProUGUI ManaText;
    public float maxHealth, maxEnergy, maxMana;
    
    private Player player;
    // Update is called once per frame
    void Start()
    {
        player = Player.GetComponent<Player>();
        maxHealth = player.heroData.defensiveStats.maxHealth;
        maxEnergy = player.heroData.specialStats.maxEnergy;
        maxMana = player.heroData.specialStats.maxMana;
        //HealthText.text = player.heroData.defensiveStats.maxHealth.ToString();
        //AttackText.text = player.heroData.offensiveStats.damage.ToString();
        //DefenseText.text = player.heroData.defensiveStats.defense.ToString();
        //EnergyText.text = player.heroData.specialStats.maxEnergy.ToString();
        //ManaText.text = player.heroData.specialStats.maxMana.ToString();
    }
    void Update()
    {
        HealthSlider.value = player.currentHealth / maxHealth;
        StaminaSlider.value = player.currentEnergy / maxEnergy;
        ManaSlider.value = player.currentMana / maxMana;
        //HealthText.text = player.heroData.defensiveStats.maxHealth.ToString();
        //AttackText.text = player.heroData.offensiveStats.damage.ToString();
        //DefenseText.text = player.heroData.defensiveStats.defense.ToString();
        //EnergyText.text = player.heroData.specialStats.maxEnergy.ToString();
        //ManaText.text = player.heroData.specialStats.maxMana.ToString();

    }
}
