using UnityEngine;
using UnityEngine.EventSystems;

public class HoverableMechControllerInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public MechController MechController;

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventRouter.OnHoverableMechControllerInteractionEnter.Invoke(gameObject, MechController);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        EventRouter.OnHoverableMechControllerInteractionExit.Invoke(gameObject, MechController);
    }
}
