using UnityEngine;
using HeartOfIron.Enums;

/// <summary>
/// Manages the main menu UI, including handling button clicks and showing/hiding panels.
/// </summary>
public class MainMenuUIManager : MonoBehaviour
{
    public static MainMenuUIManager Instance { get; private set; }
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _settingsPanel;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        GameManager.Instance.LoadGame();
        ShowMainMenu();
    }

    /// <summary>
    /// Handles the event when the start button is clicked and loads the garage scene.
    /// </summary>
    public void OnStartButtonClicked()
    {
        SceneLoader.Instance.LoadScene(SceneName.Garage);
    }

    /// <summary>
    /// Handles the event when the back button is clicked and shows the main menu.
    /// </summary>
    public void OnBackButtonClicked()
    {
        ShowMainMenu();
    }

    /// <summary>
    /// Handles the event when the settings button is clicked and shows the settings panel.
    /// </summary>
    public void OnSettingsButtonClicked()
    {
        ShowSettings();
    }

    /// <summary>
    /// Handles the event when the exit button is clicked and saves the game before exiting.
    /// </summary>
    public void OnExitButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveGame(); // Save the game before exiting
        }
        if (Application.isEditor)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
        else
        {
            Application.Quit();
        }
    }

    /// <summary>
    /// Shows the main menu panel and hides the settings panel.
    /// </summary>
    private void ShowMainMenu()
    {
        _mainMenuPanel.SetActive(true);
        _settingsPanel.SetActive(false);
    }

    /// <summary>
    /// Shows the settings panel and hides the main menu panel.
    /// </summary>
    private void ShowSettings()
    {
        _mainMenuPanel.SetActive(false);
        _settingsPanel.SetActive(true);
    }
}
