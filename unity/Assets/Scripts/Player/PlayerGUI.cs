using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class playerstatsGUI : MonoBehaviour
{
    public GameObject Player;
    public GameObject HealthCanvas;
    private Slider HealthSlider;
    private Slider StaminaSlider;
    private Slider ManaSlider;
    // public TextMeshProUGUI HealthText;
    // public TextMeshProUGUI AttackText;
    // public TextMeshProUGUI DefenseText;
    // public TextMeshProUGUI EnergyText;
    // public TextMeshProUGUI ManaText;
    [SerializeField] float maxHealth, maxEnergy, maxMana;

    private PlayerStats playerstats;

    void Awake()
    {
        HealthSlider = HealthCanvas.transform.Find("HealthBar").GetComponent<Slider>();
        StaminaSlider = HealthCanvas.transform.Find("EnergyBar").GetComponent<Slider>();
        ManaSlider = HealthCanvas.transform.Find("ManaBar").GetComponent<Slider>();
        StartCoroutine(StartAfter2s());
    }
    // Update is called once per frame
    void Start()
    {
        if (playerstats != null)
        {
            playerstats = playerstats.GetComponent<PlayerStats>();
            maxEnergy = playerstats.heroData.specialStats.maxEnergy;
            maxMana = playerstats.heroData.specialStats.maxMana;
            maxHealth = playerstats.heroData.defensiveStats.maxHealth;
        }


        //HealthText.text = playerstats.heroData.defensiveStats.maxHealth.ToString();
        //AttackText.text = playerstats.heroData.offensiveStats.damage.ToString();
        //DefenseText.text = playerstats.heroData.defensiveStats.defense.ToString();
        //EnergyText.text = playerstats.heroData.specialStats.maxEnergy.ToString();
        //ManaText.text = playerstats.heroData.specialStats.maxMana.ToString();
    }
    private IEnumerator StartAfter2s()
    {

        yield return new WaitForSeconds(2);
        playerstats = GetComponent<PlayerStats>();
        maxHealth = playerstats.heroData.defensiveStats.maxHealth;
        maxEnergy = playerstats.heroData.specialStats.maxEnergy;
        maxMana = playerstats.heroData.specialStats.maxMana;
    }
    void Update()
    {
        HealthSlider.value = playerstats.currentHealth / maxHealth;
        StaminaSlider.value = playerstats.currentEnergy / maxEnergy;
        ManaSlider.value = playerstats.currentMana / maxMana;
        //HealthText.text = playerstats.heroData.defensiveStats.maxHealth.ToString();
        //AttackText.text = playerstats.heroData.offensiveStats.damage.ToString();
        //DefenseText.text = playerstats.heroData.defensiveStats.defense.ToString();
        //EnergyText.text = playerstats.heroData.specialStats.maxEnergy.ToString();
        //ManaText.text = playerstats.heroData.specialStats.maxMana.ToString();

    }
}
