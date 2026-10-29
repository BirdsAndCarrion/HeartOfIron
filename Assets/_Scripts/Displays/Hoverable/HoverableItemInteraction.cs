using UnityEngine;
using UnityEngine.EventSystems;

public class HoverableItemInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
        EventRouter.OnHoverableItemInteractionEnter.Invoke(item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log($"Pointer exited on GameObject: {gameObject.name}, Item: {item?.Name}");
        if (item == null)
            return;
        EventRouter.OnHoverableItemInteractionExit.Invoke();
    }
}
