using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MoveRebindUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private Button upButton;
    [SerializeField] private Button downButton;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private TextMeshProUGUI upText;
    [SerializeField] private TextMeshProUGUI downText;
    [SerializeField] private TextMeshProUGUI leftText;
    [SerializeField] private TextMeshProUGUI rightText;

    private InputAction moveAction;
    private const string rebindsKey = "moveRebinds";

    private void Awake()
    {
        inputActions = FindAnyObjectByType<InputActionAsset>();
        moveAction = inputActions.FindAction("Move");
        LoadRebinds();
        UpdateUI();

        upButton.onClick.AddListener(() => StartRebind("up"));
        downButton.onClick.AddListener(() => StartRebind("down"));
        leftButton.onClick.AddListener(() => StartRebind("left"));
        rightButton.onClick.AddListener(() => StartRebind("right"));
    }

    private void StartRebind(string direction)
    {
        // Find the binding index for that composite part
        int bindingIndex = -1;
        for (int i = 0; i < moveAction.bindings.Count; i++)
        {
            var binding = moveAction.bindings[i];
            if (binding.isPartOfComposite && binding.name == direction)
            {
                bindingIndex = i;
                break;
            }
        }

        if (bindingIndex == -1)
        {
            Debug.LogError($"No binding found for {direction}");
            return;
        }

        upButton.interactable = downButton.interactable =
        leftButton.interactable = rightButton.interactable = false;

        GetTextForDirection(direction).text = "Press any key...";

        moveAction.Disable();

        moveAction.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("<Mouse>/position")
            .WithControlsExcluding("<Mouse>/delta")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation =>
            {
                moveAction.Enable();
                operation.Dispose();
                SaveRebinds();
                UpdateUI();
                EnableButtons();
            })
            .Start();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y))
        {
            m_Canvas.enabled = !m_Canvas.enabled;
        }
    }


    private void EnableButtons()
    {
        upButton.interactable = downButton.interactable =
        leftButton.interactable = rightButton.interactable = true;
    }

    private TextMeshProUGUI GetTextForDirection(string dir)
    {
        return dir switch
        {
            "up" => upText,
            "down" => downText,
            "left" => leftText,
            "right" => rightText,
            _ => null
        };
    }

    private void UpdateUI()
    {
        string upPath = "", downPath = "", leftPath = "", rightPath = "";

        foreach (var binding in moveAction.bindings)
        {
            if (!binding.isPartOfComposite) continue;

            switch (binding.name)
            {
                case "up": upPath = binding.effectivePath; break;
                case "down": downPath = binding.effectivePath; break;
                case "left": leftPath = binding.effectivePath; break;
                case "right": rightPath = binding.effectivePath; break;
            }
        }

        upText.text = InputControlPath.ToHumanReadableString(
            upPath, InputControlPath.HumanReadableStringOptions.OmitDevice);

        downText.text = InputControlPath.ToHumanReadableString(
            downPath, InputControlPath.HumanReadableStringOptions.OmitDevice);

        leftText.text = InputControlPath.ToHumanReadableString(
            leftPath, InputControlPath.HumanReadableStringOptions.OmitDevice);

        rightText.text = InputControlPath.ToHumanReadableString(
            rightPath, InputControlPath.HumanReadableStringOptions.OmitDevice);
    }


    private void SaveRebinds()
    {
        string json = inputActions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(rebindsKey, json);
        PlayerPrefs.Save();
    }

    private void LoadRebinds()
    {
        if (PlayerPrefs.HasKey(rebindsKey))
        {
            string json = PlayerPrefs.GetString(rebindsKey);
            inputActions.LoadBindingOverridesFromJson(json);
        }
    }

    public void ResetToDefault()
    {
        moveAction.RemoveAllBindingOverrides();
        PlayerPrefs.DeleteKey(rebindsKey);
        UpdateUI();
    }
}
