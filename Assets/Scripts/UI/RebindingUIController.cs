using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class RebindingUIController : MonoBehaviour
{
    [SerializeField] private InputActionAsset actionAsset;
    [SerializeField] private TMP_Text actionText;
    [SerializeField] private Button rebindButton;
    
    private InputActionRebindingExtensions.RebindingOperation rebindOperation;
    private InputAction actionToRebind;
    private int bindingIndex;
    
    public void Initialize(InputAction action, int bindingIdx, string actionName)
    {
        actionToRebind = action;
        bindingIndex = bindingIdx;
        actionText.text = actionName + ": " + GetBindingName();
        
        rebindButton.onClick.AddListener(StartRebinding);
    }

    private string GetBindingName()
    {
        string bindingPath = actionToRebind.bindings[bindingIndex].effectivePath;

        // Remove <Keyboard>/ prefix
        if (bindingPath.StartsWith("<Keyboard>/"))
        {
            bindingPath = bindingPath.Substring("<Keyboard>/".Length);
        }
        return bindingPath;
    }
    
    public void UpdateBindingDisplay()
    {
        if (actionToRebind != null)
            actionText.text = actionText.text.Split(':')[0] + ": " + GetBindingName();
    }
    
    public void StartRebinding()
    {
        // Disable the action while rebinding
        actionToRebind.Disable();
        
        rebindOperation = actionToRebind.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => RebindComplete())
            .OnCancel(operation => RebindCancelled())
            .Start();
    }
    
    private void RebindComplete()
    {
        rebindOperation.Dispose();
        rebindOperation = null;
        actionToRebind.Enable();
        UpdateBindingDisplay();
        SaveBindings();
    }
    
    private void RebindCancelled()
    {
        rebindOperation.Dispose();
        rebindOperation = null;
        actionToRebind.Enable();
    }
    
    
    private void SaveBindings()
    {
        string rebinds = actionAsset.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("InputBindings", rebinds);
        PlayerPrefs.Save();
    }
    
    private void OnDestroy()
    {
        if (rebindOperation != null)
        {
            rebindOperation.Dispose();
            rebindOperation = null;
        }
    }
}