using UnityEngine;
using UnityEngine.UI;
using HeartOfIron.Enums;

/// <summary>
/// Manages the navigation bar in the UI.
/// Handles button clicks for different scenes and highlights the current location.
/// </summary>
public class NavBar : MonoBehaviour
{

    [SerializeField] private Button _garageButton;
    [SerializeField] private Button _messHallButton;
    [SerializeField] private Button _engiBayButton;
    [SerializeField] private Button _labButton;

    public void OnGarageButtonClicked()
    {
        ShowCurrentLocation(SceneName.Garage);
        SceneLoader.Instance.LoadScene(SceneName.Garage);
    }

    public void OnMessHallButtonClicked()
    {
        ShowCurrentLocation(SceneName.MessHall);
        SceneLoader.Instance.LoadScene(SceneName.MessHall);
    }

    public void OnEngiBayButtonClicked()
    {
        ShowCurrentLocation(SceneName.EngiBay);
        SceneLoader.Instance.LoadScene(SceneName.EngiBay);
    }

    public void OnLabButtonClicked()
    {
        ShowCurrentLocation(SceneName.Lab);
        SceneLoader.Instance.LoadScene(SceneName.Lab);
    }

    private void ShowCurrentLocation(SceneName sceneName)
    {
        // Reset all button colors to default
        _garageButton.GetComponent<Image>().color = new Color32(0xE5, 0x8E, 0x26, 0xFF);
        _messHallButton.GetComponent<Image>().color = new Color32(0xE5, 0x8E, 0x26, 0xFF);
        _engiBayButton.GetComponent<Image>().color = new Color32(0xE5, 0x8E, 0x26, 0xFF);
        _labButton.GetComponent<Image>().color = new Color32(0xE5, 0x8E, 0x26, 0xFF);

        // Highlight the current location button
        switch (sceneName)
        {
            case SceneName.Garage:
                _garageButton.GetComponent<Image>().color = new Color32(0xAE, 0x4D, 0x00, 0xFF);
                break;
            case SceneName.MessHall:
                _messHallButton.GetComponent<Image>().color = new Color32(0xAE, 0x4D, 0x00, 0xFF);
                break;
            case SceneName.EngiBay:
                _engiBayButton.GetComponent<Image>().color = new Color32(0xAE, 0x4D, 0x00, 0xFF);
                break;
            case SceneName.Lab:
                _labButton.GetComponent<Image>().color = new Color32(0xAE, 0x4D, 0x00, 0xFF);
                break;
            default:
                Debug.LogWarning("Unknown scene name: " + sceneName);
                break;
        }
    }
}
