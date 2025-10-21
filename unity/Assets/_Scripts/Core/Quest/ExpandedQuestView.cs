using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class ExpandedQuestView : MonoBehaviour
{
    [Header("UI Elements")]
    public Image backgroundImage; 
    public TextMeshProUGUI questNameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI objectivesText;
    public TextMeshProUGUI rewardsText;
    private GameObject LinkedSlot;
    public Button acceptButton;
    public Button closeButton;

    private Quest currentQuest;

    // You can tweak these colors in Inspector or set defaults here
    [Header("Rarity Colors")]
    public Color commonColor = Color.white;
    public Color rareColor = new Color(0.3f, 0.6f, 1f);
    public Color epicColor = new Color(0.6f, 0.2f, 0.8f);
    public Color legendaryColor = new Color(1f, 0.8f, 0.2f);

    private void Start()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(Hide);

        if (acceptButton != null)
            acceptButton.onClick.AddListener(OnAcceptClicked);

        Hide();
    }

    public void DisplayQuest(Quest quest, GameObject slot = null)
    {
        currentQuest = quest;
        LinkedSlot = slot;

        questNameText.text = quest.questName;
        descriptionText.text = quest.questDescription;
        rewardsText.text = $"XP: {quest.experienceReward}       |       Tokens: {quest.tokenReward}";

        StringBuilder sb = new StringBuilder();
        foreach (var obj in quest.objectives)
        {
            sb.AppendLine($"{obj.enemyType}: {obj.currentKills}/{obj.requiredKills}");
        }
        objectivesText.text = sb.ToString();

        // 🟦 Match rarity color
        if (backgroundImage != null)
            backgroundImage.color = GetColorForRarity(quest.rarity);
    }

    private Color GetColorForRarity(QuestRarity rarity)
    {
        switch (rarity)
        {
            case QuestRarity.Rare:
                return rareColor;
            case QuestRarity.Epic:
                return epicColor;
            case QuestRarity.Legendary:
                return legendaryColor;
            default:
                return commonColor;
        }
    }

    public void Show() => gameObject.SetActive(true);

    public void Hide() => gameObject.SetActive(false);

    private void OnAcceptClicked()
    {
        if (currentQuest != null)
        {
            var tracker = FindObjectOfType<PlayerQuestTracker>();
            if (tracker != null)
            {
                tracker.AssignQuest(currentQuest);
                Debug.Log($"🧾 Accepted quest: {currentQuest.questName}");
            }
        }
        Hide();
        //Hides the linked slot as well
        LinkedSlot.SetActive(false);
    }
}
