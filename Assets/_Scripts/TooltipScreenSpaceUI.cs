using UnityEngine;
using System;
using System.Text;
using TMPro;

[Serializable]
public class TooltipScreenSpaceUI : MonoBehaviour
{

    private readonly float PADDING = 5f;
    
    [SerializeField]
    private RectTransform backgroundRectTransform;
    [SerializeField]
    private TextMeshProUGUI tooltipText;

    private void Awake()
    {
        if (backgroundRectTransform == null)
            backgroundRectTransform = transform.Find("background").GetComponent<RectTransform>();
        
        if (tooltipText == null)
            tooltipText = transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
    }

    public void DisplayTooltip(string text)
    {
        SetText(text);
    }

    private void SetText(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            Debug.LogWarning("Tooltip text is null or empty.");
            backgroundRectTransform.sizeDelta = Vector2.zero;
            return;
        }
        gameObject.SetActive(true);
        // Debug.Log($"Setting tooltip text: {text}");
        tooltipText.SetText(text);
        tooltipText.ForceMeshUpdate();
        Vector2 textSize = tooltipText.GetPreferredValues(text);
        // Debug.Log($"Text size: {textSize}");
        Vector2 backgroundSize = new Vector2(textSize.x + PADDING * 2, textSize.y + PADDING * 2);
        // Debug.Log($"Background size: {backgroundSize}");
        backgroundRectTransform.sizeDelta = backgroundSize;
    }
}
