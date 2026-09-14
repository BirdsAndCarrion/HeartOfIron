using UnityEngine;
using HeartOfIron.Enums;
using System;
using TMPro;
using UnityEngine.UI;

[Serializable]
[SelectionBase]
public class ResourceBar : MonoBehaviour
{
    [Header("Core Settings")]
    [SerializeField] private Image bar;
    [SerializeField] private float resourceCurrent = 100;
    [SerializeField] private float resourceMax = 100;
    [Header("Text Settings")]
    [SerializeField] private DisplayType displayType = DisplayType.LongValue;
    [SerializeField] private TextMeshProUGUI resourceText;
    [Header("Gradient Settings")]
    [SerializeField] private bool useGradient = false;
    [SerializeField] private Gradient gradient;


    public void Setup(float resourceCurrent, float resourceMax, DisplayType displayType = DisplayType.LongValue, bool useGradient = false, Gradient gradient = null)
    {
        this.resourceCurrent = Mathf.Clamp(resourceCurrent, 0, resourceMax);
        this.resourceMax = Mathf.Max(resourceMax, 1); // Ensure max is at least 1 to avoid division by zero
        this.displayType = displayType;
        this.useGradient = useGradient;
        this.gradient = gradient;

        UpdateBarAndResourceText();
    }

    void Start()
    {
        UpdateBarAndResourceText();
    }

    public bool SetResourceCurrent(float value)
    {
        resourceCurrent = Mathf.Clamp(value, 0, resourceMax);
        UpdateBarAndResourceText();
        return resourceCurrent == value;
    }

    public bool SetResourceMax(float value)
    {
        resourceMax = Mathf.Max(value, 1); // Ensure max is at least 1 to avoid division by zero
        resourceCurrent = Mathf.Clamp(resourceCurrent, 0, resourceMax);
        UpdateBarAndResourceText();
        return resourceMax == value;
    }

    private void UpdateBarAndResourceText()
    {
        if (resourceMax <= 0f)
        {
            bar.fillAmount = 0f;
            return;
        }
        if (bar != null)
        {
            bar.fillAmount = resourceCurrent / resourceMax;
        }
        SetCurrentResourceValueText();
        UseGradient();
    }

    private void SetCurrentResourceValueText()
    {
        if (resourceText != null)
        {
            switch (displayType)
            {
                case DisplayType.LongValue:
                    resourceText.text = $"{resourceCurrent}/{resourceMax}";
                    break;
                case DisplayType.ShortValue:
                    resourceText.text = $"{resourceCurrent}";
                    break;
                case DisplayType.Percentage:
                    float percentage = resourceCurrent / resourceMax * 100f;
                    resourceText.text = $"{percentage:F1}%";
                    break;
                case DisplayType.None:
                    resourceText.text = string.Empty;
                    break;
                default:
                    resourceText.text = $"{resourceCurrent}/{resourceMax}";
                    break;
            }
        }
    }

    private void UseGradient()
    {
        if (!useGradient || bar == null || gradient == null)
        {
            return;
        }
        bar.color = gradient.Evaluate(bar.fillAmount);
    }
}
