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
        // Debug.Log($"Pointer entered on GameObject: {gameObject.name}, Item: {item?.Name}");
        if (item == null)
            return;
        EventRouter.OnHoverableItemTooltipEnter.Invoke(gameObject, item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log($"Pointer exited on GameObject: {gameObject.name}, Item: {item?.Name}");
        if (item == null)
            return;
        EventRouter.OnHoverableItemTooltipExit.Invoke(gameObject);
    }
}
