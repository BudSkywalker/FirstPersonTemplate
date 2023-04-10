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
    public delegate void OnUpdateKeybindLabels();

    public static event OnUpdateKeybindLabels onUpdateKeybindLabels;
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
    private InputActionRebindingExtensions.RebindingOperation rebindAction;
    private int index;
    private PlayerInput playerInput;

    private void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>() ?? gameObject.AddComponent<PlayerInput>();

        index = Array.IndexOf(action.bindings.ToArray(), binding);
        action = actionMap.FindAction(action.name);
        binding = action.bindings[index];
        labelText = GetComponentInChildren<TMP_Text>();
        button = GetComponentInChildren<Button>();
        buttonText = button.GetComponentsInChildren<TMP_Text>().First(x => x != labelText);
        button.onClick.AddListener(OnStartBind);

        labelText.text = binding.isPartOfComposite ? action.name + " " + binding.name : action.name;
        onUpdateKeybindLabels += UpdateButtonText;
        UpdateButtonText();
    }

    private void OnEnable()
    {
        //UpdateButtonText();
    }

    public static void UpdateKeybindLabels()
    {
        onUpdateKeybindLabels?.Invoke();
    }

    private void UpdateButtonText()
    {
        binding = action.bindings[index];
        buttonText.text = binding.ToDisplayString();
    }

    private void OnStartBind()
    {
        buttonText.text = "...";
        WhileBinding();
    }

    private void WhileBinding()
    {
        playerInput.DeactivateInput();
        rebindAction =
            action.PerformInteractiveRebinding(index)
                .WithBindingGroup(binding.groups)
                .WithControlsExcluding("<Mouse>/position")
                .WithControlsExcluding("<Mouse>/delta")
                .WithControlsExcluding("<Keyboard>/escape")
                .WithControlsExcluding("<Keyboard>/anyKey")
                .OnMatchWaitForAnother(0.1f);

        rebindAction.OnComplete(_ => OnStopBind(index));

        rebindAction.Start();
    }

    private void OnStopBind(int index)
    {
        rebindAction.Dispose();
        binding = action.bindings[index];
        buttonText.text = binding.ToDisplayString();
        Settings.GetSettings().keybindSettings.AddOverride(action.name, index, binding.effectivePath);
        Settings.SaveSettings();
        playerInput.ActivateInput();
    }
}