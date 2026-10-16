using UnityEngine;
using UnityEngine.EventSystems;
using System;

[Serializable, RequireComponent(typeof(TooltipScreenSpaceUI))]
public class HoverInformationTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string tooltipText;
    private TooltipScreenSpaceUI tooltipUI;

    private void Awake()
    {
        tooltipUI = GetComponent<TooltipScreenSpaceUI>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Show the tooltip with the specified text
        tooltipUI.DisplayTooltip(tooltipText); 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Hide the tooltip when the pointer exits
        tooltipUI.HideTooltip();
    }
}
