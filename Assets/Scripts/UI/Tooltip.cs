using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private string text;
    [SerializeField]
    private float hoverDelay;

    private void Start()
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

    private IEnumerator DisplayTooltip()
    {
        yield return new WaitForSecondsRealtime(hoverDelay);

        TooltipDisplay.Instance.Text.text = text;
        TooltipDisplay.Instance.gameObject.SetActive(true);
    }
}