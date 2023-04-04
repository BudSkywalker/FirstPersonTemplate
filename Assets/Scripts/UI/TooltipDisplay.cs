using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TooltipDisplay : MonoBehaviour
{
    [SerializeField]
    private Vector2 padding = new(20, 10);
    [SerializeField]
    private float mouseOffset = 25;
    [SerializeField]
    [Range(0f, 100f)]
    private float borderOffset;
    private RectTransform rectTransform;
    public static TooltipDisplay Instance { get; private set; }

    public TMP_Text Text { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.LogWarning("You have multiple Tooltip Displays in your scene. Please ensure there is only one!");
            Destroy(this);
        }

        Text = GetComponentInChildren<TMP_Text>();
        rectTransform = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        rectTransform.sizeDelta = Text.GetPreferredValues() + padding;
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        Vector2 sizeDelta = rectTransform.sizeDelta;
        float widthRadius = sizeDelta.x / 2;
        float heightRadius = sizeDelta.y / 2;
        Vector2 targetPosition = Mouse.current.position.value + Vector2.up * mouseOffset + Vector2.up * heightRadius;

        //Initial position setting
        rectTransform.position = targetPosition;

        //Testing for tooltip off the left of the screen
        if (rectTransform.position.x - widthRadius < borderOffset) rectTransform.position = new Vector2(borderOffset + widthRadius, targetPosition.y);
        //Testing for tooltip off the right of the screen
        else if (rectTransform.position.x + widthRadius > Screen.width - borderOffset) rectTransform.position = new Vector2(Screen.width - borderOffset - widthRadius, targetPosition.y);
        //Testing for tooltip off top of screen
        if (rectTransform.position.y + heightRadius > Screen.height - borderOffset) rectTransform.position = new Vector2(rectTransform.position.x, Screen.height - borderOffset - heightRadius * 4);
    }
}