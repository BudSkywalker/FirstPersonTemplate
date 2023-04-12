using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Displays tooltip of slider's handle value and updates as it is adjusted
/// </summary>
[RequireComponent(typeof(Slider))]
public class SliderValueDisplayTooltip : Tooltip, IPointerMoveHandler
{
    private Slider slider;

    protected override void Start()
    {
        base.Start();
        slider = GetComponent<Slider>();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        StartCoroutine(DisplayTooltip());
    }

    protected override IEnumerator DisplayTooltip()
    {
        TooltipDisplay.Instance.UpdateText(slider.value.ToString("F2"));
        TooltipDisplay.Instance.gameObject.SetActive(true);
        yield return new WaitForEndOfFrame();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SliderValueDisplayTooltip))]
[CanEditMultipleObjects]
public class SettingsFieldEditor : Editor
{
    public override void OnInspectorGUI()
    {
    }
}
#endif