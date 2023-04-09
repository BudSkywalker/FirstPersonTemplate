using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Controls a key rebinder prefab giving it traditional "click to rebind" functionality
/// </summary>
public class KeyRebinder : MonoBehaviour
{
    /// <summary>
    /// The <see cref="InputAction"/> of the keybind to control
    /// </summary>
    [HideInInspector]
    public InputAction action;
    /// <summary>
    /// The <see cref="InputBinding"/> of the keybind to control
    /// </summary>
    [HideInInspector]
    public InputBinding binding;
    /// <summary>
    /// The <see cref="InputActionMap"/> of the keybind to control
    /// </summary>
    [HideInInspector]
    public InputActionMap actionMap;
    private Button button;
    private TMP_Text labelText, buttonText;

    private void Start()
    {
        labelText = GetComponentInChildren<TMP_Text>();
        button = GetComponentInChildren<Button>();
        buttonText = button.GetComponentsInChildren<TMP_Text>().First(x => x != labelText);
        button.onClick.AddListener(OnStartBind);

        labelText.text = binding.isPartOfComposite ? binding.name : action.name;
        buttonText.text = binding.ToDisplayString();
    }

    private void OnStartBind()
    {
        buttonText.text = "...";
        WhileBinding();
    }

    private void WhileBinding()
    {
        InputActionRebindingExtensions.RebindingOperation rebindAction =
            action.PerformInteractiveRebinding(Array.IndexOf(action.bindings.ToArray(), binding))
                .WithBindingGroup(binding.groups)
                .WithControlsExcluding("<Mouse>/position")
                .WithControlsExcluding("<Mouse>/delta")
                .WithControlsExcluding("<Keyboard>/escape")
                .WithControlsExcluding("<Keyboard>/anyKey")
                .OnMatchWaitForAnother(0.1f);

        rebindAction.OnComplete(_ =>
        {
            Settings.GetSettings().keybindSettings.AddOverride(action.id, binding.id, binding.effectivePath);
            OnStopBind(ref rebindAction);
            Settings.SaveSettings();
        });

        rebindAction.Start();
    }

    private void OnStopBind(ref InputActionRebindingExtensions.RebindingOperation rebindAction)
    {
        buttonText.text = binding.ToDisplayString();
        rebindAction.Dispose();
    }
}