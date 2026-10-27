using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Provides a runtime color picker UI for adjusting the color of a mech or its pilot.
/// Updates the color preview and the associated mech or pilot color in real-time.
/// This class is intended to interface with the UI elements for color selection and preview.
/// </summary>
public class RuntimeColorPicker : MonoBehaviour
{

    [Header("Color Picker UI Elements")]
    [SerializeField] private Slider _redSlider;
    [SerializeField] private Slider _greenSlider;
    [SerializeField] private Slider _blueSlider;
    [SerializeField] private Image _colorPreviewImage;
    private Mech _targetMech; // The mech whose color will be changed
    bool _adjustingPilotColor = false; // Flag to determine if we're adjusting the pilot's color
    private GameObject _buttonVisual;

    // TODO: Possible extension of adding update of visual representation of the mech in the garage scene when color changes, if applicable.

    public void SetTargetMech(GameObject buttonVisual, Mech mech, bool adjustPilotColor = false)
    {
        _buttonVisual = buttonVisual;
        _targetMech = mech;
        _adjustingPilotColor = adjustPilotColor;

        if (_targetMech != null)
        {
            Color initialColor = _adjustingPilotColor ? _targetMech.Pilot.Color : _targetMech.Color;
            _redSlider.value = initialColor.r;
            _greenSlider.value = initialColor.g;
            _blueSlider.value = initialColor.b;
            UpdateColor(initialColor);
        }
    }

    public void ClearTargetMech()
    {
        _targetMech = null;
        _adjustingPilotColor = false;
        _buttonVisual = null;
    }
    
    void Start()
    {

        _redSlider.onValueChanged.AddListener(OnSliderChanged);
        _greenSlider.onValueChanged.AddListener(OnSliderChanged);
        _blueSlider.onValueChanged.AddListener(OnSliderChanged);

        UpdateColor(new Color(_redSlider.value, _greenSlider.value, _blueSlider.value));
    }

    void OnSliderChanged(float value)
    {
        Color newColor = new Color(_redSlider.value, _greenSlider.value, _blueSlider.value);
        UpdateColor(newColor);
    }

    void UpdateColor(Color newColor)
    {
        if (_colorPreviewImage != null)
        {
            _colorPreviewImage.color = newColor;
        }
        if (_targetMech != null && _adjustingPilotColor)
        {
            _targetMech.Pilot.SetPilotColor(newColor);
        } else if (_targetMech != null)
        {
            _targetMech.SetMechColor(newColor);
        }
        if (_buttonVisual != null)
        {
            Image buttonImage = _buttonVisual.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = newColor;
            }
        }
    }
}
