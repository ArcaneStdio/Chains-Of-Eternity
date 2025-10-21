using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class QuestSlotUI : MonoBehaviour, IPointerClickHandler
{
    [Header("UI References")]
    public TextMeshProUGUI questNameText;
    public TextMeshProUGUI recommendedLevelText;
    public Image backgroundImage; // The UI Image component for rarity coloring

    private Quest linkedQuest;
    private QuestBoardUI questBoardUI;

    // Define color mapping for rarities
    private static readonly Color commonColor = new(0.8f, 0.8f, 0.8f); // light gray
    private static readonly Color rareColor = new(0.3f, 0.6f, 1f);      // blue
    private static readonly Color epicColor = new(0.6f, 0.2f, 0.9f);     // purple
    private static readonly Color legendaryColor = new(1f, 0.7f, 0.1f);  // gold

    public void Show() => gameObject.SetActive(true);

    public void Hide() => gameObject.SetActive(false);

    /// <summary>
    /// Sets up this slot with quest data.
    /// </summary>
    public void SetupSlot(Quest quest, QuestBoardUI board)
    {
        Show();
        linkedQuest = quest;
        questBoardUI = board;
        recommendedLevelText.text = quest.recommendedLevel != null ? $"Recommended Level: {quest.recommendedLevel}" : "Recommended Level: N/A";
        
        if (questNameText != null)
            questNameText.text = quest.questName;
        else
            questNameText.text = "Unknown Quest";

        UpdateRarityColor(quest.rarity);
    }

    /// <summary>
    /// Updates the slot background based on quest rarity.
    /// </summary>
    private void UpdateRarityColor(QuestRarity rarity)
    {
        if (backgroundImage == null) return;

        switch (rarity)
        {
            case QuestRarity.Common:
                backgroundImage.color = commonColor;
                break;
            case QuestRarity.Rare:
                backgroundImage.color = rareColor;
                break;
            case QuestRarity.Epic:
                backgroundImage.color = epicColor;
                break;
            case QuestRarity.Legendary:
                backgroundImage.color = legendaryColor;
                break;
        }
    }

    /// <summary>
    /// Clears the slot when no quest is assigned.
    /// </summary>
    public void ClearSlot()
    {
        linkedQuest = null;
        if (questNameText != null)
            questNameText.text = "—";
        if (recommendedLevelText != null)
            recommendedLevelText.text = "";
        if (backgroundImage != null)
            backgroundImage.color = commonColor;
    }

    /// <summary>
    /// Called when this UI element is clicked anywhere.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (linkedQuest != null && questBoardUI != null)
            questBoardUI.OnQuestClicked(linkedQuest, gameObject);
    }
}
