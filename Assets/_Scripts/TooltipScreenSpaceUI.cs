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

    public void DisplayTooltip(StringBuilder sb)
    {
        SetText(sb.ToString());
    }

    private void SetText(string text)
    {
        tooltipText.SetText(text);
        tooltipText.ForceMeshUpdate();
        Vector2 textSize = tooltipText.GetRenderedValues(false);
        Vector2 backgroundSize = new Vector2(textSize.x + PADDING * 2, textSize.y + PADDING * 2);
        backgroundRectTransform.sizeDelta = backgroundSize;
    }
}
