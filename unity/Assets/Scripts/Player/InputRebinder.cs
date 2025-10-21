using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using System.Collections;
using TMPro;

public class InputRebinder : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions; // Reference to your Input Actions asset
    private const string rebindsKey = "inputRebinds";

    private void Awake()
    {
        inputActions = FindAnyObjectByType<InputActionAsset>();
        // Load saved bindings on start
        LoadRebinds();
    }

    public void StartRebind(string actionName, int bindingIndex)
    {
        InputAction action = inputActions.FindAction(actionName);
        if (action == null)
        {
            Debug.LogError($"Action '{actionName}' not found!");
            return;
        }

        if (bindingIndex >= action.bindings.Count)
        {
            Debug.LogError("Invalid binding index");
            return;
        }

        // Skip composite bindings like "WASD"
        if (action.bindings[bindingIndex].isComposite)
        {
            Debug.LogWarning("Cannot rebind a composite directly.");
            return;
        }

        Debug.Log($"Starting rebind for {action.name}...");

        action.Disable();

        var rebindOperation = action.PerformInteractiveRebinding(bindingIndex)
            .OnMatchWaitForAnother(0.1f) // To handle multi-part bindings like Shift+E
            .OnComplete(operation =>
            {
                action.Enable();
                operation.Dispose();
                SaveRebinds();
                Debug.Log($"{action.name} rebound to {action.bindings[bindingIndex].effectivePath}");
            })
            .Start();
    }

    public void ResetBindings()
    {
        inputActions.RemoveAllBindingOverrides();
        PlayerPrefs.DeleteKey(rebindsKey);
    }

    private void SaveRebinds()
    {
        string rebinds = inputActions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(rebindsKey, rebinds);
        PlayerPrefs.Save();
        Debug.Log("Saved rebinds: " + rebinds);
    }

    private void LoadRebinds()
    {
        if (PlayerPrefs.HasKey(rebindsKey))
        {
            string rebinds = PlayerPrefs.GetString(rebindsKey);
            inputActions.LoadBindingOverridesFromJson(rebinds);
            Debug.Log("Loaded rebinds");
        }
    }
}
