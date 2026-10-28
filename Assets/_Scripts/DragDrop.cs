using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class DragDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{ 
    private ActionMarker actionMarker;
    public bool active = false;
    public bool interactable = true;
    private RectTransform rectTransform;
    private RectTransform canvasRectTransform;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetActionMarker(ActionMarker marker)
    {
        actionMarker = marker;
        active = true;
    }

    private void Update()
    {
        if (actionMarker == null && active)
        {
            Destroy(gameObject);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!interactable) return;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }
   
    public void OnDrag(PointerEventData eventData)
    {
        if (!interactable) return;
        float changeInY = eventData.delta.y / canvasRectTransform.localScale.y;
        if (rectTransform.anchoredPosition.y + changeInY < 0)
            changeInY = -rectTransform.anchoredPosition.y;
        else if (rectTransform.anchoredPosition.y + changeInY > rectTransform.parent.GetComponent<RectTransform>().rect.height)
            changeInY = rectTransform.parent.GetComponent<RectTransform>().rect.height - rectTransform.anchoredPosition.y;
        rectTransform.anchoredPosition += new Vector2(0, changeInY);
        int sibIndex = rectTransform.GetSiblingIndex();
        int nextSiblingIndex = sibIndex + 1;
        int prevSiblingIndex = sibIndex - 1;
        if (nextSiblingIndex < rectTransform.parent.childCount && rectTransform.anchoredPosition.y < rectTransform.parent.GetChild(nextSiblingIndex).GetComponent<RectTransform>().anchoredPosition.y)
        {
            rectTransform.SetSiblingIndex(nextSiblingIndex);
        }
        else if (prevSiblingIndex >= 0 && rectTransform.anchoredPosition.y > rectTransform.parent.GetChild(prevSiblingIndex).GetComponent<RectTransform>().anchoredPosition.y)
        {
            rectTransform.SetSiblingIndex(prevSiblingIndex);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!interactable) return;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        if (actionMarker != null)
        {
            actionMarker.SourceMech.RemoveActionMarker(actionMarker);
            actionMarker.SourceMech.InsertActionMarkerAt(actionMarker, rectTransform.GetSiblingIndex());
        }
    }
    
}
