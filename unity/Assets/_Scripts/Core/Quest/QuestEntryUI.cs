using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestEntryUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text questNameText;
    public TMP_Text rewardText;
    public TMP_Text descriptionText;
    public Button expandButton;
    public Button acceptButton;
    public GameObject expandedPanel;

    private Quest quest;
    private QuestBoardUI questBoardUI;
    private bool isExpanded = false;

    public void Setup(Quest questData, QuestBoardUI boardUI)
    {
        quest = questData;
        questBoardUI = boardUI;

        questNameText.text = quest.questName;
        rewardText.text = $"{quest.tokenReward} Tokens | {quest.experienceReward} XP";
        descriptionText.text = quest.questDescription;

        expandedPanel.SetActive(false);

        expandButton.onClick.AddListener(ToggleExpand);
        acceptButton.onClick.AddListener(() => questBoardUI.AcceptQuest(quest));
    }

    private void ToggleExpand()
    {
        isExpanded = !isExpanded;
        expandedPanel.SetActive(isExpanded);
    }
}
