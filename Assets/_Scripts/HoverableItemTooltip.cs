using UnityEngine;
using UnityEngine.EventSystems;
using System;

[Serializable]
public class HoverableItemTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public Item item;

    void Awake()
    {
        EventRouter.RegisterWithHub(this);
    }
    
    void OnDestroy()
    {
        EventRouter.UnregisterFromHub(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventRouter.OnHoverableItemTooltipEnter.Invoke(gameObject, item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        EventRouter.OnHoverableItemTooltipExit.Invoke(gameObject);
    }
}
