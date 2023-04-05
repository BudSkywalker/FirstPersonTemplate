using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Displays a tooltip after hovering over object for specified delay
/// </summary>
/// <seealso cref="TooltipDisplay"/>
public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// Tooltip message to display
    /// </summary>
    public string text;
    /// <summary>
    /// Time to wait before displaying tooltip
    /// </summary>
    [SerializeField]
    protected float hoverDelay = 3f;

    protected virtual void Start()
    {
        if (TooltipDisplay.Instance != null) return;

        Object prefab = Resources.Load<Object>("Tooltip Display");
        if (prefab != null)
        {
            Debug.Log("Creating Tooltip Display from Resources prefab.");

            Instantiate(prefab, FindObjectOfType<Canvas>().transform, false).name = "Tooltip Display";
        }
        else
        {
            Debug.LogWarning("Could not find tooltip display in scene, killing tooltip");
            Destroy(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(DisplayTooltip());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        TooltipDisplay.Instance.gameObject.SetActive(false);
    }

    protected virtual IEnumerator DisplayTooltip()
    {
        yield return new WaitForSecondsRealtime(hoverDelay);

        TooltipDisplay.Instance.UpdateText(text);
    }
}