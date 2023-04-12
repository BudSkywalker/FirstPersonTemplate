using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Menu
{
    /// <summary>
    /// Populates a panel with all keybinds in a certain <see cref="InputActionMap" /> and sets up their <see cref="KeyRebinder" />.
    /// If you want to control multiple maps in the same menu you should add multiple <see cref="PopulateKeybindsPanel" />
    /// scripts, one for each map to display in menu.
    /// </summary>
    public class PopulateKeybindsPanel : MonoBehaviour
    {
        /// <summary>
        /// Optional prefab for header of menu
        /// </summary>
        [SerializeField]
        private GameObject headerPrefab;
        /// <summary>
        /// Prefab controlled by a <see cref="KeyRebinder" /> to spawn
        /// </summary>
        [SerializeField]
        private GameObject rebindPrefab;
        /// <summary>
        /// What <see cref="InputActionAsset" /> do we use to get all the information from?
        /// </summary>
        public InputActionAsset inputActionAsset;
        /// <summary>
        /// The index of the target <see cref="InputActionMap" /> for this keybind set
        /// </summary>
        [HideInInspector]
        public int targetMapIndex;
        /// <summary>
        /// The index of the reference <see cref="InputActionMap" /> for this keybind set.
        /// Some keybinds in a set might not want to be made available for the player,
        /// so a separate map should be duplicated and the protected fields removed.
        /// All actions in this map will be made available for customization.
        /// </summary>
        [HideInInspector]
        public int referenceMapIndex;
        private PlayerInput playerInput;

        private void Awake()
        {
            playerInput = FindObjectOfType<PlayerInput>() ?? gameObject.AddComponent<PlayerInput>();
            playerInput.onControlsChanged += Repoplute;
        }

        private void OnEnable()
        {
            if (transform.childCount == 0) Repoplute();
        }

        private void Repoplute(PlayerInput obj)
        {
            Repoplute();
        }

        private void Repoplute()
        {
            if (headerPrefab is not null) Instantiate(headerPrefab, transform).GetComponentInChildren<TMP_Text>().text = playerInput.currentControlScheme + " Keybinds";
            foreach (Transform t in GetComponentsInChildren<Transform>().Where(go => go != transform)) Destroy(t.gameObject);

            foreach (InputAction action in inputActionAsset.actionMaps[referenceMapIndex].actions)
            {
                foreach (InputBinding binding in action.bindings)
                {
                    if (binding.groups.Contains(playerInput.currentControlScheme))
                    {
                        KeyRebinder rebinder = Instantiate(rebindPrefab, transform).GetComponent<KeyRebinder>();
                        rebinder.actionMap = inputActionAsset.actionMaps[targetMapIndex];
                        rebinder.action = rebinder.actionMap.FindAction(action.name);
                        rebinder.binding = rebinder.action.bindings[Array.IndexOf(action.bindings.ToArray(), binding)];
                    }
                }
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(PopulateKeybindsPanel))]
    [CanEditMultipleObjects]
    public class PopulateKeybindsPanelEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            GUILayoutOption[] options = { GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true) };
            SerializedProperty targetMapIndex = serializedObject.FindProperty("targetMapIndex");
            SerializedProperty referenceMapIndex = serializedObject.FindProperty("referenceMapIndex");
            string[] names = ((PopulateKeybindsPanel)target).inputActionAsset.actionMaps.Select(x => x.name).ToArray();

            targetMapIndex.intValue = EditorGUILayout.Popup("Target Input Map", targetMapIndex.intValue, names, options);
            referenceMapIndex.intValue = EditorGUILayout.Popup("Reference Input Map", referenceMapIndex.intValue, names, options);

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}